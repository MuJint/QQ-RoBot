namespace Qiushui.Lian.Bot.Models
{
    public class SignLogs : BaseModel
    {
        public string Uid { get; set; }
        public string LogContent { get; set; }
        public int ModifyRank { get; set; }
    }
}
