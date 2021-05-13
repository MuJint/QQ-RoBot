namespace Qiushui.Framework.Models
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
        public MsgType MsgType { get; set; } = MsgType.Txt;
    }
    public enum MsgType
    {
        Img = 1,
        Txt = 2
    }
}
