namespace Qiushui.Lian.Bot.Models
{
    public class SignUser : BaseModel
    {
        /// <summary>
        /// Q号
        /// </summary>
        public string QNumber { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 群组ID
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// rank
        /// </summary>
        public int Rank { get; set; }
    }
}
