using System;
using Qiushui.Bot.Models;
using Sora.Entities;
using Sora.Entities.CQCodes;
using Sora.EventArgs.SoraEvent;

namespace Qiushui.Bot.ChatModule
{
    internal class Surprise
    {
        #region 属性
        public  object                Sender       { private set; get; }
        public  GroupMessageEventArgs MFKEventArgs { private set; get; }
        private Group                 QQGroup      { set;         get; }
        #endregion

        #region 构造函数
        public Surprise(object sender, GroupMessageEventArgs eventArgs)
        {
            this.MFKEventArgs = eventArgs;
            this.Sender       = sender;
            this.QQGroup      = MFKEventArgs.SourceGroup;
        }
        #endregion

        #region 消息响应函数
        /// <summary>
        /// 消息接收函数
        /// </summary>
        public async void GetChat(object cmdType)
        {
            if (MFKEventArgs == null || Sender == null) return;

            //判断指令类型并分发
            if (cmdType is KeywordCommand keywordCmdType)
            {
                switch (keywordCmdType)
                {
                    //昏睡套餐
                    case KeywordCommand.RedTea:
                        RedTea();
                        break;
                    //随机禁言
                    case KeywordCommand.RandomBan:
                        RandomBan();
                        break;
                }
            }

            if (cmdType is RegexCommand regexCommandType)
            {
                switch (regexCommandType)
                {
                    //骰子
                    case RegexCommand.Dice:
                        RandomNumber();
                        break;
                }
            }
        }
        #endregion

        #region 私有方法
        private async void RandomNumber()
        {
            Random randomGen = new Random();
            await QQGroup.SendGroupMessage(CQCode.CQAt(MFKEventArgs.Sender.Id), "丢出了\r\n", randomGen.Next(0, 100));
        }

        private async void RandomBan()
        {
            Random banTime = new Random();
            await MFKEventArgs.SourceGroup.EnableGroupMemberMute(MFKEventArgs.Sender.Id,
                                                           banTime.Next(1, 10) * 60);
        }

        private async void RedTea()
        {
            await MFKEventArgs.SourceGroup.EnableGroupMemberMute(MFKEventArgs.Sender.Id,
                                                                 28800);
        }
        #endregion
    }
}
