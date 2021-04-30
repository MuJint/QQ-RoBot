using Newtonsoft.Json;
using Qiushui.Common;
using Sora.Entities.CQCodes;
using Sora.Tool;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sora.EventArgs.SoraEvent;

namespace Qiushui.Bot
{
    /// <summary>
    /// ModuleService
    /// <para>模块实现</para>
    /// </summary>
    public class ModuleService : BaseServiceObject, IModuleInterface
    {
        readonly ILianInterface _lianService;
        public ModuleService()
        {
            _lianService = GetInstance<ILianInterface>();
        }
        public async ValueTask HsoHandle(object sender, GroupMessageEventArgs e)
        {
            Config config = new(e.LoginUid);
            config.LoadUserConfig(out UserConfig userConfig);
            //检查色图文件夹大小
            if (IOUtils.GetHsoSize() >= userConfig.HsoConfig.SizeLimit * 1024 * 1024)
            {
                ConsoleLog.Warning("Hso", "色图文件夹超出大小限制，将清空文件夹");
                Directory.Delete(IOUtils.GetHsoPath(), true);
            }
            await GeneaterHso(userConfig.HsoConfig, e.SourceGroup);
        }

        public async ValueTask LianHandle(object sender, GroupMessageEventArgs e, KeywordCommand command)
        {
            if (e == null || sender == null) return;

            Config config = new(e.LoginUid);
            config.LoadUserConfig(out UserConfig userConfig);
            //var _lianService = new DealInstruction(userConfig);
            //判断指令类型并分发
            if (command is KeywordCommand keywordCmdType)
            {
                switch (keywordCmdType)
                {
                    case KeywordCommand.Sign:
                        if (IsTrigger(e.Message.RawText, KeywordCommand.Sign.GetDescription()))
                            await _lianService.SignIn(e, userConfig);
                        break;
                    case KeywordCommand.Search:
                        if (IsTrigger(e.Message.RawText, KeywordCommand.Search.GetDescription()))
                            await _lianService.SearchRank(e, userConfig);
                        break;
                    case KeywordCommand.Fenlai:
                        await _lianService.Fenlai(e, userConfig);
                        break;
                    case KeywordCommand.Skill:
                        await _lianService.Skill(e, userConfig);
                        break;
                    case KeywordCommand.RankList:
                        if (IsTrigger(e.Message.RawText, KeywordCommand.RankList.GetDescription()))
                            await _lianService.RankList(e);
                        break;
                    case KeywordCommand.SpecialEvent:
                        if (IsTrigger(e.Message.RawText, KeywordCommand.SpecialEvent.GetDescription()))
                            await _lianService.SpecialEvent(e);
                        break;
                    case KeywordCommand.LogsRecord:
                        await _lianService.LogsRecord(e);
                        break;
                    case KeywordCommand.Giving:
                        await _lianService.Giving(e);
                        break;
                    case KeywordCommand.Morning:
                        if (IsTrigger(e.Message.RawText, KeywordCommand.Morning.GetDescription()))
                            await _lianService.Morning(e, userConfig);
                        break;
                    case KeywordCommand.Night:
                        if (IsTrigger(e.Message.RawText, KeywordCommand.Night.GetDescription()))
                            await _lianService.Night(e, userConfig);
                        break;
                    case KeywordCommand.BonusPoint:
                        await _lianService.BonusPoint(e, userConfig);
                        break;
                    case KeywordCommand.DeductPoint:
                        await _lianService.DeductPoint(e, userConfig);
                        break;
                    case KeywordCommand.AllBonusPoint:
                        await _lianService.AllBonusPoint(e, userConfig);
                        break;
                    case KeywordCommand.AllDeductPoint:
                        await _lianService.AllDeductPoint(e, userConfig);
                        break;
                    case KeywordCommand.RedTea:
                        await _lianService.RedTea(e);
                        break;
                    case KeywordCommand.Raffle:
                        await _lianService.Raffle(e, userConfig);
                        break;
                    case KeywordCommand.Rob:
                        await _lianService.Rob(e, userConfig);
                        break;
                    case KeywordCommand.Rescur:
                        await _lianService.Rescur(e, userConfig);
                        break;
                    case KeywordCommand.Lian:
                        if (IsTrigger(e.Message.RawText, KeywordCommand.Lian.GetDescription()))
                            await _lianService.Lian(e);
                        break;
                    case KeywordCommand.AddKeys:
                        await _lianService.AddKeys(e);
                        break;
                    case KeywordCommand.AddThesaurus:
                        await _lianService.AddThesaurus(e);
                        break;
                }
            }
        }


        #region Func

        /// <summary>
        /// <para>从色图源获取色图</para>
        /// <para>不会支持R18的哦</para>
        /// </summary>
        /// <param name="hso">hso配置实例</param>
        private static async Task GeneaterHso(Hso hso, Sora.Entities.Group group)
        {
            string localPicPath;
            ConsoleLog.Debug("源", hso.Source);
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
                ConsoleLog.Debug("发送图片", localPicPath);
                await group.SendGroupMessage(hso.CardImage
                                                   ? CQCode.CQCardImage(localPicPath)
                                                   : CQCode.CQImage(localPicPath));
                return;
            }
            //网络部分
            try
            {
                ConsoleLog.Info("NET", "尝试获取色图");
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
                        ConsoleLog.Error("Hso", "发生了未知错误");
                        return;
                }
                //向服务器发送请求
                var json = await HttpHelper.HttpGetAsync($"{serverUrl}?apikey={apiKey}&r18={hso.R18}");
                var result = JsonConvert.DeserializeObject<HsoResponseModel>(json);
                if (result != null && result.Code == 0)
                {
                    await group.SendGroupMessage(GetResult(result));
                    await group.SendGroupMessage(CQCode.CQImage(result.Data.First().Url));
                }
                else
                    await group.SendGroupMessage("发生了未知错误");
                ConsoleLog.Debug("Get Json", json);
            }
            catch (Exception e)
            {
                //网络错误
                await group.SendGroupMessage("网络错误，暂时没有请求到~");
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


        private static bool IsTrigger(string rawText, string keyWord) => rawText.Equals(keyWord);
        #endregion
    }
}
