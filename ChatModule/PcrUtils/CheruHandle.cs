using Qiushui.Lian.Bot.Models;
using Sora.EventArgs.SoraEvent;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qiushui.Bot.ChatModule.PcrUtils
{
    /// <summary>
    /// 切噜语转换
    /// 参考自HoshinoBot
    /// 对原算法有所改进
    /// </summary>
    internal class CheruHandle
    {
        #region 字符集常量
        //切噜字符集
        const string CHERU_SET = "切卟叮咧哔唎啪啰啵嘭噜噼巴拉蹦铃";
        #endregion

        #region 属性
        public object                Sender         { private set; get; }
        public GroupMessageEventArgs CheruEventArgs { private set; get; }
        #endregion

        #region 构造函数
        public CheruHandle(object sender, GroupMessageEventArgs e)
        {
            this.CheruEventArgs = e;
            this.Sender         = sender;
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 获取到群消息
        /// </summary>
        public void GetChat(RegexCommand cmdType)
        {
            if (CheruEventArgs == null || Sender == null) return;
            //检查参数
            switch (cmdType)
            {
                case RegexCommand.CheruDecode:
                    if (CheruEventArgs.Message.RawText.Length > 3)
                        CheruToString(CheruEventArgs.Message.RawText.Substring(3));
                    break;
                case RegexCommand.CheruEncode:
                    if (CheruEventArgs.Message.RawText.Length > 4)
                        StringToCheru(CheruEventArgs.Message.RawText.Substring(4));
                    break;
            }
        }

        /// <summary>
        /// 将切噜语解码为原句
        /// </summary>
        /// <param name="cheru">切噜语</param>
        public async void CheruToString(string cheru)
        {
            Regex         isCheru     = new Regex(@"切[切卟叮咧哔唎啪啰啵嘭噜噼巴拉蹦铃]+");
            StringBuilder textBuilder = new StringBuilder();
            foreach (string cheruWord in Regex.Split(cheru,@"\b"))
            {
                textBuilder.Append(isCheru.IsMatch(cheruWord) ? CheruToWord(cheruWord) : cheruWord);
            }
            await CheruEventArgs.SourceGroup.SendGroupMessage($"切噜的意思是:{textBuilder}");
        }

        /// <summary>
        /// 将原句编码为切噜语
        /// </summary>
        /// <param name="text">原语句</param>
        public async void StringToCheru(string text)
        {
            Regex         isCHN        = new Regex(@"[\u4e00-\u9fa5]");
            StringBuilder cheruBuilder = new StringBuilder();
            foreach (string word in Regex.Split(text,@"\b"))
            {
                cheruBuilder.Append(isCHN.IsMatch(word) ? WordToCheru(word) : word);
            }
            await CheruEventArgs.SourceGroup.SendGroupMessage($"切噜～{cheruBuilder}");
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 将中文词语为切噜词
        /// </summary>
        /// <param name="word">原词语</param>
        private static string WordToCheru(string word)
        {
            byte[] cheruBytes = Encoding.GetEncoding("GB18030").GetBytes(word);
            //切噜语翻译
            StringBuilder res = new StringBuilder();
            //开始翻译(不是
            res.Append("切");
            //将字符byte拆分高低四位并与字符集对应
            foreach (byte cheruByte in cheruBytes)
            {
                res.Append(CHERU_SET[cheruByte        & 0x0F]);
                res.Append(CHERU_SET[(cheruByte >> 4) & 0x0F]);
            }
            return res.ToString();
        }

        /// <summary>
        /// 将切噜词翻译为中文
        /// </summary>
        /// <param name="cheru">切噜词</param>
        private static string CheruToWord(string cheru)
        {
            if (cheru.Length < 2 && !cheru.StartsWith("切")) return cheru;
            string cheruContent = cheru.Substring(1);

            //转换为正常语句
            List<byte> wordBytes = new List<byte>();
            for (int i = 0; i < cheruContent.Length; i+= 2)
            {
                if (i + 1 < cheruContent.Length)
                {
                    //将index作为高低四位合并为八位
                    byte wordByte = (byte) (CHERU_SET.IndexOf(cheruContent[i]) + (CHERU_SET.IndexOf(cheruContent[i + 1]) << 4));
                    wordBytes.Add(wordByte);
                }
            }
            //剩下的单字符
            Regex isPunctuation = new Regex(@"\b");//跳过标点符号
            if (cheruContent.Length % 2 == 1 && !isPunctuation.IsMatch(cheruContent[^1].ToString()))
                wordBytes.Add((byte) CHERU_SET[CHERU_SET.IndexOf(cheruContent[^1])]);
            return Encoding.GetEncoding("GB18030").GetString(wordBytes.ToArray());
        }
        #endregion
    }
}
