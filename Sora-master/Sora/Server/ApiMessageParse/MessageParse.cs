using System;
using System.Collections.Generic;
using Sora.Entities.CQCodes;
using Sora.Entities.CQCodes.CQCodeModel;
using Sora.Enumeration;
using Sora.Tool;

namespace Sora.Server.ApiMessageParse
{
    internal static class MessageParse
    {
        /// <summary>
        /// 处理接收到的消息段
        /// </summary>
        /// <param name="message">消息段</param>
        /// <returns>消息段列表</returns>
        internal static CQCode ParseMessageElement(ApiMessage message)
        {
            if (message?.RawData == null || message.RawData.Count == 0) return null;
            try
            {
                switch (message.MsgType)
                {
                    case CQFunction.Text:
                        return new CQCode(CQFunction.Text, message.RawData.ToObject<Text>());
                    case CQFunction.Face:
                        return new CQCode(CQFunction.Face, message.RawData.ToObject<Face>());
                    case CQFunction.Image:
                        return new CQCode(CQFunction.Image, message.RawData.ToObject<Image>());
                    case CQFunction.Record:
                        return new CQCode(CQFunction.Record, message.RawData.ToObject<Record>());
                    case CQFunction.At:
                        return new CQCode(CQFunction.At, message.RawData.ToObject<At>());
                    case CQFunction.Share:
                        return new CQCode(CQFunction.Share, message.RawData.ToObject<Share>());
                    case CQFunction.Reply:
                        return new CQCode(CQFunction.Reply, message.RawData.ToObject<Reply>());
                    case CQFunction.Forward:
                        return new CQCode(CQFunction.Forward, message.RawData.ToObject<Forward>());
                    case CQFunction.Xml:
                        return new CQCode(CQFunction.Xml, message.RawData.ToObject<Code>());
                    case CQFunction.Json:
                        return new CQCode(CQFunction.Json, message.RawData.ToObject<Code>());
                    default:
                        return new CQCode(CQFunction.Unknown, message.RawData);
                }
            }
            catch (Exception e)
            {
                ConsoleLog.Error("Sora",ConsoleLog.ErrorLogBuilder(e));
                ConsoleLog.Error("Sora",$"Json CQ码转换错误 未知CQ码格式，出错CQ码{message.MsgType},请向框架开发者反应此问题");
                return new CQCode(CQFunction.Unknown, message.RawData);
            }
        }

        /// <summary>
        /// 处理消息段数组
        /// </summary>
        /// <param name="messages">消息段数组</param>
        internal static List<CQCode> ParseMessageList(List<ApiMessage> messages)
        {
            ConsoleLog.Debug("Sora","Parsing msg list");
            if (messages == null || messages.Count == 0) return new List<CQCode>();
            List<CQCode> retMsg = new List<CQCode>();
            foreach (ApiMessage message in messages)
            {
                retMsg.Add(ParseMessageElement(message));
            }
            ConsoleLog.Debug("Sora",$"Get msg len={retMsg.Count}");
            return retMsg;
        }
    }
}
