using Newtonsoft.Json;

namespace Sora.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 合并转发/合并转发节点
    /// </summary>
    public struct Forward
    {
        #region 属性
        /// <summary>
        /// 转发消息ID
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string MessageId { get; internal set; }
        #endregion
    }
}
