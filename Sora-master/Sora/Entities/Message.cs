using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sora.Entities.Base;
using Sora.Entities.CQCodes;
using Sora.Entities.CQCodes.CQCodeModel;
using Sora.Enumeration;

namespace Sora.Entities
{
    /// <summary>
    /// 消息实例
    /// </summary>
    public sealed class Message : BaseModel
    {
        #region 属性
        /// <summary>
        /// 消息ID
        /// </summary>
        public int MessageId { get; private set; }

        /// <summary>
        /// 纯文本信息
        /// </summary>
        public string RawText { get; private set; }

        /// <summary>
        /// 消息段列表
        /// </summary>
        public List<CQCode> MessageList { get; private set; }

        /// <summary>
        /// 消息时间戳
        /// </summary>
        public long Time { get; private set; }

        /// <summary>
        /// 消息字体id
        /// </summary>
        public int Font { get; private set; }
        #endregion

        #region 构造函数
        internal Message(Guid connectionGuid, int msgId, string text, List<CQCode> cqCodeList, long time, int font) : base(connectionGuid)
        {
            this.MessageId   = msgId;
            this.RawText     = text;
            this.MessageList = cqCodeList;
            this.Time        = time;
            this.Font        = font;
        }
        #endregion

        #region 消息管理方法
        /// <summary>
        /// 撤回本条消息
        /// </summary>
        public async ValueTask RecallMessage()
        {
            await base.SoraApi.RecallMessage(this.MessageId);
        }
        #endregion

        #region CQ码快捷方法
        /// <summary>
        /// 获取所有At的UID
        /// </summary>
        /// <returns>
        /// <para>At的uid列表</para>
        /// <para><see cref="List{T}"/>(T=<see cref="long"/>)</para>
        /// </returns>
        public List<long> GetAllAtList()
        {
            return MessageList.Where(cq => cq.Function == CQFunction.At)
                              .Select(cq => Convert.ToInt64(((At) cq.CQData).Traget ?? "-1"))
                              .ToList();
        }

        /// <summary>
        /// 获取语音URL
        /// 仅在消息为语音时有效
        /// </summary>
        /// <returns>语音文件url</returns>
        public string GetRecordUrl()
        {
            if (this.MessageList.Count != 1 || MessageList.First().Function != CQFunction.Record) return null;
            return ((Record)MessageList.First().CQData).Url;
        }

        /// <summary>
        /// 获取所有图片信息
        /// </summary>
        /// <returns>
        /// <para>图片信息结构体列表</para>
        /// <para><see cref="List{T}"/>(T=<see cref="Image"/>)</para>
        /// </returns>
        public List<Image> GetAllImage()
        {
            return MessageList.Where(cq => cq.Function == CQFunction.Image)
                              .Select(cq => (Image) cq.CQData)
                              .ToList();
        }
        #endregion

        #region 转换方法
        /// <summary>
        /// <para>转纯文本信息</para>
        /// <para>注意：CQ码会被转换为onebot的string消息段格式</para>
        /// </summary>
        public override string ToString()
        {
            return RawText;
        }
        #endregion
    }
}
