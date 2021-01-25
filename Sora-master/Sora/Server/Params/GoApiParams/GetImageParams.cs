using Newtonsoft.Json;

namespace Sora.Server.Params.GoApiParams
{
    /// <summary>
    /// 获取图片信息
    /// </summary>
    internal struct GetImageParams
    {
        [JsonProperty(PropertyName = "file")]
        internal string FileName { get; set; }
    }
}
