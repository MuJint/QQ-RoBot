using Qiushui.Lian.Bot.Business;
using Qiushui.Lian.Bot.Helper.ConfigModule;
using Qiushui.Lian.Bot.Models;
using Sora.EventArgs.SoraEvent;
using Group = Sora.Entities.Group;

namespace Qiushui.Lian.Bot.ChatModule.LianModule
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
                        await service.SignIn(EventArgs);
                        break;
                    case KeywordCommand.Search:
                        await service.SearchRank(EventArgs);
                        break;
                    case KeywordCommand.Fenlai:
                        await service.Fenlai(EventArgs);
                        break;
                    case KeywordCommand.Skill:
                        await service.Skill(EventArgs);
                        break;
                    case KeywordCommand.RankList:
                        await service.RankList(EventArgs);
                        break;
                    case KeywordCommand.SpecialEvent:
                        await service.SpecialEvent(EventArgs);
                        break;
                    case KeywordCommand.LogsRecord:
                        await service.LogsRecord(EventArgs);
                        break;
                    case KeywordCommand.Giving:
                        await service.Giving(EventArgs);
                        break;
                    case KeywordCommand.Morning:
                        await service.Morning(EventArgs);
                        break;
                    case KeywordCommand.Night:
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

        
        #endregion
    }
}
