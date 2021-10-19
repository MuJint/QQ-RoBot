﻿using Newtonsoft.Json;
using Robot.Common;
using Sora.Entities.Segment;
using Sora.EventArgs.SoraEvent;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukariToolBox.FormatLog;

namespace QQ.RoBot
{
    /// <summary>
    /// hso实现
    /// </summary>
    public class HsoService : BaseServiceObject, IHsoInterface
    {
        #region Property

        #endregion

        #region Public Func

        #region Hso
        /// <summary>
        /// hso
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async ValueTask Hso(GroupMessageEventArgs e)
        {
            Config config = new(e.LoginUid);
            config.LoadUserConfig(out UserConfig userConfig);
            //检查色图文件夹大小
            if (IOUtils.GetHsoSize() >= userConfig.HsoConfig.SizeLimit * 1024 * 1024)
            {
                Log.Warning("Hso", "色图文件夹超出大小限制，将清空文件夹");
                Directory.Delete(IOUtils.GetHsoPath(), true);
            }
            await GeneaterHso(userConfig.HsoConfig, e.SourceGroup);
        }
        #endregion

        #endregion

        #region Private Func

        #region Hso
        /// <summary>
        /// <para>从色图源获取色图</para>
        /// <para>不会支持R18的哦</para>
        /// </summary>
        /// <param name="hso">hso配置实例</param>
        private static async Task GeneaterHso(Hso hso, Sora.Entities.Group group)
        {
            string localPicPath;
            Log.Debug("源", hso.Source);
            //本地模式
            if (hso.Source == SetuSourceType.Local)
            {
                string[] picNames = Directory.GetFiles(IOUtils.GetHsoPath());
                if (picNames.Length == 0)
                {
                    await group.SendGroupMessage("机器人管理者没有在服务器上塞色图\r\n你去找他要啦!");
                    return;
                }
                Random randFile = new();
                localPicPath = $"{picNames[randFile.Next(0, picNames.Length - 1)]}";
                Log.Debug("发送图片", localPicPath);
                await group.SendGroupMessage(hso.CardImage
                                                   ? SegmentBuilder.CardImage(localPicPath)
                                                   : SegmentBuilder.Image(localPicPath));
                return;
            }
            //网络部分
            try
            {
                Log.Info("NET", "尝试获取色图");
                string apiKey;
                string serverUrl;
                //源切换
                switch (hso.Source)
                {
                    case SetuSourceType.Mix:
                        Random randSource = new();
                        if (randSource.Next(1, 100) > 50)
                        {
                            serverUrl = "https://api.lolicon.app/setu/";
                            apiKey = hso.LoliconApiKey ?? string.Empty;
                        }
                        else
                        {
                            serverUrl = "https://api.yukari.one/setu/";
                            apiKey = hso.YukariApiKey ?? string.Empty;
                        }
                        break;
                    case SetuSourceType.Yukari:
                        serverUrl = "https://api.yukari.one/setu/";
                        apiKey = hso.YukariApiKey ?? string.Empty;
                        break;
                    case SetuSourceType.Lolicon:
                        serverUrl = "https://api.yukari.one/setu/";
                        apiKey = hso.LoliconApiKey ?? string.Empty;
                        break;
                    default:
                        await group.SendGroupMessage("发生了未知错误");
                        Log.Error("Hso", "发生了未知错误");
                        return;
                }
                //向服务器发送请求
                var json = await HttpHelper.HttpGetAsync($"{serverUrl}?apikey={apiKey}&r18={hso.R18}");
                var result = JsonConvert.DeserializeObject<HsoResponseModel>(json);
                if (result != null && result.Code == 0)
                {
                    await group.SendGroupMessage(GetResult(result));
                    await group.SendGroupMessage(SegmentBuilder.Image(result.Data.First().Url));
                }
                else
                    await group.SendGroupMessage("发生了未知错误");
                Log.Debug("Get Json", json);
            }
            catch (Exception e)
            {
                //网络错误
                await group.SendGroupMessage("网络错误，暂时没有请求到~");
                Log.Error("网络发生错误", Log.ErrorLogBuilder(e.InnerException));
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

        #endregion
    }
}
