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
                }
            }
        }
        #endregion

        #region Private Func

        
        #endregion
    }
}
