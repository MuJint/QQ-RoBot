using System.ComponentModel;

namespace Sora.Enumeration.EventParamsType
{
    /// <summary>
    /// 荣誉类型
    /// </summary>
    public enum HonorType
    {
        /// <summary>
        /// 龙王
        /// </summary>
        [Description("talkative")]
        Talkative,
        /// <summary>
        /// 群聊之火
        /// </summary>
        [Description("performer")]
        Performer,
        /// <summary>
        /// 快乐源泉
        /// </summary>
        [Description("emotion")]
        Emotion
    }
}
