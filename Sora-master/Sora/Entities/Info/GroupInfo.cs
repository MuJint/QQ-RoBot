namespace Sora.Entities.Info
{
    /// <summary>
    /// 群信息
    /// </summary>
    public struct GroupInfo
    {
        #region 属性
        /// <summary>
        /// 群名称
        /// </summary>
        public string GroupName { get; internal set; }

        /// <summary>
        /// 成员数
        /// </summary>
        public int MemberCount { get; internal set; }

        /// <summary>
        /// 最大成员数（群容量）
        /// </summary>
        public int MaxMemberCount { get; internal set; }

        /// <summary>
        /// 群组ID
        /// </summary>
        public long GroupId { get; internal set; }
        #endregion
    }
}
