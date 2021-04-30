using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Qiushui.Framework.Models
{
    public class BaseModel
    {
        [Key]
        public int ID { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime LastModifyTime { get; set; } = DateTime.Now;
        public Status Status { get; set; } = Status.Valid;
    }

    public enum Status
    {
        [Description("启用")]
        Valid = 1,
        [Description("禁用")]
        InValid = 0,
        [Description("封禁")]
        Banned = 3
    }
}
