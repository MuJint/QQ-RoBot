using Qiushui.Bot.Business;
using Qiushui.Bot.Helper.ConfigModule;
using Qiushui.Bot.Models;
using Sora.EventArgs.SoraEvent;
using Group = Sora.Entities.Group;

namespace Qiushui.Bot.ChatModule.LianModule
{
    internal class LianHandle
    {
        #region 属性
        public object Sender { private set; get; }
        public Group QQGroup { private set; get; }
        public GroupMessageEventArgs EventArgs { private set; get; }
        #endregion

        #region 构造函数
        public LianHandle(object sender, GroupMessageEventArgs e)
        {
            this.EventArgs = e;
            this.Sender = sender;
            this.QQGroup = e.SourceGroup;
        }
        #endregion

        #region 指令响应
        /// <summary>
        /// 指令响应
        /// </summary>
        /// <param name="cmdType"></param>
        public async void Chat(object cmdType)
        {
            if (EventArgs == null || Sender == null) return;

            Config config = new Config(EventArgs.LoginUid);
            config.LoadUserConfig(out UserConfig userConfig);
            var service = new DealInstruction(userConfig);
            //判断指令类型并分发
            if (cmdType is KeywordCommand keywordCmdType)
            {
                switch (keywordCmdType)
                {
                    case KeywordCommand.Sign:
                        if (IsTrigger(EventArgs.Message.RawText, KeywordCommand.Sign.GetDescription()))
                            await service.SignIn(EventArgs);
                        break;
                    case KeywordCommand.Search:
                        if (IsTrigger(EventArgs.Message.RawText, KeywordCommand.Search.GetDescription()))
                            await service.SearchRank(EventArgs);
                        break;
                    case KeywordCommand.Fenlai:
                        await service.Fenlai(EventArgs);
                        break;
                    case KeywordCommand.Skill:
                        await service.Skill(EventArgs);
                        break;
                    case KeywordCommand.RankList:
                        if (IsTrigger(EventArgs.Message.RawText, KeywordCommand.RankList.GetDescription()))
                            await service.RankList(EventArgs);
                        break;
                    case KeywordCommand.SpecialEvent:
                        if (IsTrigger(EventArgs.Message.RawText, KeywordCommand.SpecialEvent.GetDescription()))
                            await service.SpecialEvent(EventArgs);
                        break;
                    case KeywordCommand.LogsRecord:
                        await service.LogsRecord(EventArgs);
                        break;
                    case KeywordCommand.Giving:
                        await service.Giving(EventArgs);
                        break;
                    case KeywordCommand.Morning:
                        if (IsTrigger(EventArgs.Message.RawText, KeywordCommand.Morning.GetDescription()))
                            await service.Morning(EventArgs);
                        break;
                    case KeywordCommand.Night:
                        if (IsTrigger(EventArgs.Message.RawText, KeywordCommand.Night.GetDescription()))
                            await service.Night(EventArgs);
                        break;
                    case KeywordCommand.BonusPoint:
                        await service.BonusPoint(EventArgs);
                        break;
                    case KeywordCommand.DeductPoint:
                        await service.DeductPoint(EventArgs);
                        break;
                    case KeywordCommand.AllBonusPoint:
                        await service.AllBonusPoint(EventArgs);
                        break;
                    case KeywordCommand.AllDeductPoint:
                        await service.AllDeductPoint(EventArgs);
                        break;
                    case KeywordCommand.RedTea:
                        await service.RedTea(EventArgs);
                        break;
                    case KeywordCommand.Raffle:
                        await service.Raffle(EventArgs);
                        break;
                    case KeywordCommand.Rob:
                        await service.Rob(EventArgs);
                        break;
                    case KeywordCommand.Rescur:
                        await service.Rescur(EventArgs);
                        break;
                    case KeywordCommand.Lian:
                        if (IsTrigger(EventArgs.Message.RawText, KeywordCommand.Lian.GetDescription()))
                            await service.Lian(EventArgs);
                        break;
                    case KeywordCommand.AddKeys:
                        await service.AddKeys(EventArgs);
                        break;
                    case KeywordCommand.AddThesaurus:
                        await service.AddThesaurus(EventArgs);
                        break;
                }
            }
        }
        #endregion

        #region Private Func

        private bool IsTrigger(string rawText, string keyWord) => rawText.Equals(keyWord);

        #endregion
    }
}
