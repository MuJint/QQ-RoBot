using Newtonsoft.Json;
using Qiushui.Bot.Helper;
using Qiushui.Bot.Helper.ConfigModule;
using Qiushui.Bot.Models;
using Sora.Entities.CQCodes;
using Sora.EventArgs.SoraEvent;
using Sora.Tool;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Group = Sora.Entities.Group;

namespace Qiushui.Bot.ChatModule.HsoModule
{
    internal class HsoHandle
    {
        #region 属性
        public object                  Sender       { private set; get; }
        public Group                   QQGroup      { private set; get; }
        public GroupMessageEventArgs HsoEventArgs { private set; get; }
        #endregion

        #region 构造函数
        public HsoHandle(object sender, GroupMessageEventArgs e)
        {
            this.HsoEventArgs = e;
            this.Sender       = sender;
            this.QQGroup      = e.SourceGroup;
        }
        #endregion

        #region 指令响应分发
        /// <summary>
        /// 用于处理传入指令
        /// </summary>
        public async void GetChat()
        {
            Config config = new Config(HsoEventArgs.LoginUid);
            config.LoadUserConfig(out UserConfig userConfig);
            //检查色图文件夹大小
            if (IOUtils.GetHsoSize() >= userConfig.HsoConfig.SizeLimit * 1024 * 1024)
            {
                ConsoleLog.Warning("Hso","色图文件夹超出大小限制，将清空文件夹");
                Directory.Delete(IOUtils.GetHsoPath(),true);
            }
            await GiveMeSetu(userConfig.HsoConfig);
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// <para>从色图源获取色图</para>
        /// <para>不会支持R18的哦</para>
        /// </summary>
        /// <param name="hso">hso配置实例</param>
        private async Task GiveMeSetu(Hso hso)
        {
            string  localPicPath;
            ConsoleLog.Debug("源",hso.Source);
            //本地模式
            if (hso.Source == SetuSourceType.Local)
            {
                string[] picNames = Directory.GetFiles(IOUtils.GetHsoPath());
                if (picNames.Length == 0)
                {
                    await QQGroup.SendGroupMessage("机器人管理者没有在服务器上塞色图\r\n你去找他要啦!");
                    return;
                }
                Random randFile = new Random();
                localPicPath = $"{picNames[randFile.Next(0, picNames.Length - 1)]}";
                ConsoleLog.Debug("发送图片",localPicPath);
                await QQGroup.SendGroupMessage(hso.CardImage
                                                   ? CQCode.CQCardImage(localPicPath)
                                                   : CQCode.CQImage(localPicPath));
                return;
            }
            //网络部分
            try
            {
                ConsoleLog.Info("NET", "尝试获取色图");
                await QQGroup.SendGroupMessage("正在尝试获取");
                string apiKey;
                string serverUrl;
                //源切换
                switch (hso.Source)
                {
                    case SetuSourceType.Mix:
                        Random randSource = new Random();
                        if (randSource.Next(1, 100) > 50)
                        {
                            serverUrl = "https://api.lolicon.app/setu/";
                            apiKey    = hso.LoliconApiKey ?? string.Empty;
                        }
                        else
                        {
                            serverUrl = "https://api.yukari.one/setu/";
                            apiKey    = hso.YukariApiKey ?? string.Empty;
                        }
                        break;
                    case SetuSourceType.Yukari:
                        serverUrl = "https://api.yukari.one/setu/";
                        apiKey    = hso.YukariApiKey ?? string.Empty;
                        break;
                    case SetuSourceType.Lolicon:
                        serverUrl = "https://api.yukari.one/setu/";
                        apiKey = hso.LoliconApiKey ?? string.Empty;
                        break;
                    default:
                        await QQGroup.SendGroupMessage("发生了未知错误");
                        ConsoleLog.Error("Hso","发生了未知错误");
                        return;
                }
                //向服务器发送请求
                var json = await HttpHelper.HttpGetAsync($"{serverUrl}?apikey={apiKey}&r18={hso.R18}");
                var result = JsonConvert.DeserializeObject<HsoResponseModel>(json);
                if (result != null && result.Code == 0)
                {
                    await QQGroup.SendGroupMessage(GetResult(result));
                    await QQGroup.SendGroupMessage(CQCode.CQImage(result.Data.First().Url));
                }
                else
                    await QQGroup.SendGroupMessage("发生了未知错误");
                ConsoleLog.Debug("Get Json", json);
            }
            catch (Exception e)
            {
                //网络错误
                await QQGroup.SendGroupMessage("网络错误，暂时没有请求到~");
                ConsoleLog.Error("网络发生错误", ConsoleLog.ErrorLogBuilder(e.InnerException));
                return;
            }
            //https://dotnet.microsoft.com/download/dotnet/current/runtime
        }

        private static string GetResult(HsoResponseModel hsoResponse)
        {
            var strBulider = new StringBuilder();
            var tags = hsoResponse.Data.First().Tags;
            strBulider.Append($"[画师]：{hsoResponse.Data.First().Author}\r\n");
            strBulider.Append($"[标题]：{hsoResponse.Data.First().Title}\r\n");
            strBulider.Append($"[Pid]：{hsoResponse.Data.First().Pid}\r\n");
            strBulider.Append($"[画师ID]：{hsoResponse.Data.First().Uid}\r\n");
            strBulider.Append($"[标签]：{string.Join(",", tags)}\r\n");
            return strBulider.ToString();
        }
        #endregion
    }
}
