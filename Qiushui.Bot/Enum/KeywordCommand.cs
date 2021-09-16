using System.ComponentModel;

namespace QQ.RoBot
{
    /// <summary>
    /// 关键字触发
    /// 关键字在Description中以空格分隔多个关键字，仅在初始化时读取
    /// </summary>
    public enum KeywordCommand
    {
        /// <summary>
        /// 昏睡红茶
        /// </summary>
        [Description("优质睡眠 昏睡红茶 昏睡套餐 健康睡眠")]
        RedTea,
        /// <summary>
        /// 来点色图！
        /// </summary>
        [Description("来点色图 来点涩图 我要看色图 色图 涩图")]
        Hso,
        /// <summary>
        /// 随机禁言
        /// </summary>
        [Description("随机禁言")]
        RandomBan,
        /// <summary>
        /// 签到
        /// </summary>
        [Description("签到")]
        Sign,
        /// <summary>
        /// 查询
        /// </summary>
        [Description("查询")]
        Search,
        /// <summary>
        /// 早安
        /// </summary>
        [Description("早安")]
        Morning,
        /// <summary>
        /// 晚安
        /// </summary>
        [Description("晚安")]
        Night,
        /// <summary>
        /// 莲
        /// </summary>
        [Description("莲")]
        Lian,
        /// <summary>
        /// 分来
        /// </summary>
        [Description("分来")]
        Fenlai,
        /// <summary>
        /// 排行榜
        /// </summary>
        [Description("排行榜")]
        RankList,
        /// <summary>
        /// 特殊事件
        /// </summary>
        [Description("特殊事件")]
        SpecialEvent,
        /// <summary>
        /// 活跃榜
        /// </summary>
        [Description("活跃榜")]
        AliveList,
        /// <summary>
        /// 技能
        /// </summary>
        [Description("技能 菜单 功能 指令")]
        Skill,
        /// <summary>
        /// 抽奖
        /// </summary>
        [Description("抽奖")]
        Raffle,
        /// <summary>
        /// 打劫
        /// </summary>
        [Description("打劫")]
        Rob,
        /// <summary>
        /// 劫狱
        /// </summary>
        [Description("救援 劫狱")]
        Rescur,
        /// <summary>
        /// 赠送
        /// </summary>
        [Description("赠送")]
        Giving,
        /// <summary>
        /// 关键词
        /// </summary>
        [Description("关键词")]
        KeyWords,
        /// <summary>
        /// 加分
        /// </summary>
        [Description("加分")]
        BonusPoint,
        /// <summary>
        /// 扣分
        /// </summary>
        [Description("扣分")]
        DeductPoint,
        /// <summary>
        /// 全体加分
        /// </summary>
        [Description("全体加分")]
        AllBonusPoint,
        /// <summary>
        /// 全体扣分
        /// </summary>
        [Description("全体扣分")]
        AllDeductPoint,
        /// <summary>
        /// 个人记录
        /// </summary>
        [Description("积分记录 个人积分")]      
        LogsRecord,
        /// <summary>
        /// 添加数据密码
        /// </summary>
        [Description("添加数据密码")]
        AddKeys,
        /// <summary>
        /// 添加词库
        /// </summary>
        [Description("添加词库")]
        AddThesaurus,
        /// <summary>
        /// 骰子
        /// </summary>
        [Description("骰子 扔骰子 掷骰子 色子")]
        RollDice,
        /// <summary>
        /// 词云
        /// </summary>
        [Description("词云")]
        WordCloud,
        /// <summary>
        /// 发言榜
        /// </summary>
        [Description("发言榜")]
        NonsenseKing,
    }
}
