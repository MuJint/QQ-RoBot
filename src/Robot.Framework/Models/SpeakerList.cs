using System.Collections.Generic;

namespace Robot.Framework.Models
{
    /// <summary>
    /// 记录表
    /// </summary>
    public class SpeakerList : BaseModel
    {
        public long Uid { get; set; }
        public long MsgId { get; set; }
        public long GroupId { get; set; }
        public string RawText { get; set; }

        /// <summary>
        /// 消息段
        /// </summary>
        public ICollection<SpeakerMessageBody> MessageBodies { get; set; } = new HashSet<SpeakerMessageBody>();
    }

    public class SpeakerMessageBody
    {
        public string Json { get; set; }
        public string Text { get; set; }
        public MsgType MsgType { get; set; } = MsgType.Txt;
        /// <summary>
        /// IMG / VOICE ==
        /// </summary>
        public string Operator { get; set; }
    }

    public enum MsgType
    {
        Img,
        Txt,
        Face,
        Record,
        Video,
        Music,
        At,
    }
}
