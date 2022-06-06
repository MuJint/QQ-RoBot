namespace Robot.Framework.Models
{
    /// <summary>
    /// 谁是卧底词库
    /// </summary>
    public class UndercoverLexicon : BaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// 卧底
        /// </summary>
        public string UndercoverWord { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nick { get; set; } = "于心";

        /// <summary>
        /// Uid
        /// </summary>
        public long Uid { get; set; } = 1069430666;
    }
}
