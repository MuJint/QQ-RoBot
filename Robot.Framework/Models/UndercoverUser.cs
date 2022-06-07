namespace Robot.Framework.Models
{
    /// <summary>
    /// 谁是卧底玩家
    /// </summary>
    public class UndercoverUser : BaseModel
    {
        /// <summary>
        /// 房间号
        /// </summary>
        public int RoomId { get; set; }
        /// <summary>
        /// 是否淘汰
        /// </summary>
        public bool IsOut { get; set; } = false;
        /// <summary>
        /// Uid
        /// </summary>
        public long Uid { get; set; }
        /// <summary>
        /// nick
        /// </summary>
        public string Nick { get; set; }
        /// <summary>
        /// 是否卧底
        /// </summary>
        public bool IsUndercover { get; set; } = false;
    }
}
