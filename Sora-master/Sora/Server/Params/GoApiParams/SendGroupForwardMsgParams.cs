using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sora.Server.Params.GoApiParams
{
    /// <summary>
    /// 发送合并转发(群)参数
    /// </summary>
    internal struct SendGroupForwardMsgParams
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        internal long GroupId { get; set; }

        /// <summary>
        /// 消息节点列表
        /// </summary>
        [JsonProperty(PropertyName = "messages")]
        internal List<object> NodeMsgList { get; set; }
    }
}
