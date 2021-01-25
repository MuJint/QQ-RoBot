using System;
using Newtonsoft.Json;
using Sora.Enumeration.ApiEnum;
using Sora.Server.Converter;

namespace Sora.Server.Params.ApiParams
{
    /// <summary>
    /// API请求类
    /// </summary>
    internal sealed class ApiRequest
    {
        /// <summary>
        /// API请求类型
        /// </summary>
        [JsonProperty(PropertyName = "action")]
        [JsonConverter(typeof(EnumDescriptionConverter))]
        internal APIType ApiType { get; set; }

        /// <summary>
        /// 请求标识符
        /// 会自动生成初始值不需要设置
        /// </summary>
        [JsonProperty(PropertyName = "echo")]
        internal Guid Echo { get; set; } = Guid.NewGuid();

        /// <summary>
        /// API参数对象
        /// 不需要参数时不需要设置
        /// </summary>
        [JsonProperty(PropertyName = "params")]
        internal object ApiParams { get; set; } = new { };
    }
}
