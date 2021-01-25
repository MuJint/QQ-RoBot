using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Sora.Enumeration;

namespace Sora.Server.Converter
{
    /// <summary>
    /// 用于Enum和Description特性互相转换的Json转换器
    /// </summary>
    internal class EnumDescriptionConverter : JsonConverter
    {
        //控制执行条件（当属性的值为枚举类型时才使用转换器）
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Enum);
        }

        /// <summary>
        /// 序列化时执行的转换
        /// 获取枚举的描述值
        /// </summary>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                writer.WriteValue("");
                return;
            }
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString()!);
            if (fieldInfo == null)
            {
                writer.WriteValue("");
                return;
            }
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            writer.WriteValue(attributes.Length > 0 ? attributes[0].Description : "");
        }

        /// <summary>
        /// 序列化时执行的转换
        /// 通过Description获取枚举值
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            FieldInfo[] fields    = objectType.GetFields();
            string      readValue = reader.Value?.ToString() ?? string.Empty;
            foreach (FieldInfo field in fields)
            {
                object[] objects = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (objects.Any(item => (item as DescriptionAttribute)?.Description ==
                                        readValue))
                {
                    return Convert.ChangeType(field.GetValue(-1),objectType);
                }
            }
            return CQFunction.Unknown;
        }
    }
}
