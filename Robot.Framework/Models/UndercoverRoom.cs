namespace Robot.Framework.Models
{
    /// <summary>
    /// 谁是卧底
    /// </summary>
    public class UndercoverRoom : BaseModel
    {
        /// <summary>
        /// 创建人
        /// </summary>
        public long CreateUid { get; set; }
        /// <summary>
        /// 词库ID
        /// </summary>
        public int UndercoverLexiconId { get; set; }
        /// <summary>
        /// 群组ID
        /// </summary>
        public long GroupId { get; set; }
        /// <summary>
        /// 是否已开始游戏
        /// </summary>
        public bool IsStart { get; set; }
    }
}
