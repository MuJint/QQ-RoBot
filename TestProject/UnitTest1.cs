using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qiushui.Bot.Framework.IServices;
using Qiushui.Bot.Framework.Services;
using System;
using Xunit.Sdk;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        readonly ISignUserServices signUserServices = new SignUserServices();
        readonly ISignLogsServices signLogsServices = new SignLogsServices();
        readonly ILianChatServices lianChatServices = new LianChatServices();
        readonly ILianKeyWordsServices lianKeyWordsServices = new LianKeyWordsServices();

        [TestMethod]
        public void TestMethod1()
        {
            foreach (ContentEnum item in Enum.GetValues(typeof(ContentEnum)))
            {
                string strName = Enum.GetName(typeof(ContentEnum), item);//获取名称
                string strVaule = item.ToString();//获取值
            }
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestMethod2Async()
        {
            try
            {
                //var readResults = OperateExcel.ExcelToDataTable(@"C:\Users\changqing\Desktop\chats.xlsx", "sheet1", true);
                //var listResults = OperateExcel.ToDataList<Input>(readResults);
                //foreach (var item in listResults)
                //{
                //    await lianChatServices.Insert(new Qiushui.Lian.Bot.Models.LianChat()
                //    {
                //        Content = item.Chats
                //    });
                //}
                //var t = await lianKeyWordsServices.Query(t => t.Status == Qiushui.Lian.Bot.Models.Status.Valid);
                //var t1 = await signUserServices.Query(t => t.Status == Qiushui.Lian.Bot.Models.Status.Valid);
                //var t2 = await lianChatServices.Query(t => t.Status == Qiushui.Lian.Bot.Models.Status.Valid);
                await signLogsServices.DeleteById(t => t.ID > 0);
               // await signLogsServices.DeleteById(2);
                var t = await signLogsServices.Query(t => t.ID > 0);
            }
            catch (Exception c)
            {

            }

        }

        public class Input
        {
            public string Chats { get; set; }
        }

        private enum ContentEnum
        {
            [Description("签到")]
            签到 = 1,
            [Description("查询")]
            查询 = 2,
            [Description("早安")]
            早安 = 3,
            [Description("晚安")]
            晚安 = 4,
            [Description("莲")]
            莲 = 5,
            [Description("分来")]
            分来 = 6,
            [Description("排行榜")]
            排行榜 = 7,
            [Description("特殊事件")]
            特殊事件 = 8,
            [Description("活跃榜")]
            活跃榜 = 9,
            [Description("技能")]
            技能 = 10,
            [Description("抽奖")]
            抽奖 = 11,
            [Description("打劫")]
            打劫 = 12,
            [Description("劫狱")]
            劫狱 = 13,
            [Description("赠送")]
            赠送 = 14,
            [Description("关键词")]
            关键词 = 15,
            [Description("加分")]
            加分 = 16,
            [Description("扣分")]
            扣分 = 17,
            [Description("全体加分")]
            全体加分 = 18,
            [Description("全体扣分")]
            全体扣分 = 19
        }

        /// <summary>
        /// 字符串相似度计算
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        public static void Levenshtein(String str1, String str2)
        {
            //计算两个字符串的长度。  
            int len1 = str1.Length;
            int len2 = str2.Length;
            //建立上面说的数组，比字符长度大一个空间  
            int[,] dif = new int[len1 + 1, len2 + 1];
            //赋初值，步骤B。  
            for (int a = 0; a <= len1; a++)
            {
                dif[a, 0] = a;
            }
            for (int a = 0; a <= len2; a++)
            {
                dif[0, a] = a;
            }
            //计算两个字符是否一样，计算左上的值  
            int temp;
            for (int i = 1; i <= len1; i++)
            {
                for (int j = 1; j <= len2; j++)
                {
                    if (str1[i - 1] == str2[j - 1])
                    {
                        temp = 0;
                    }
                    else
                    {
                        temp = 1;
                    }
                    //取三个值中最小的  
                    dif[i, j] = min(dif[i - 1, j - 1] + temp, dif[i, j - 1] + 1,
                            dif[i - 1, j] + 1);
                }
            }
            Console.WriteLine("字符串\"" + str1 + "\"与\"" + str2 + "\"的比较");
            //取数组右下角的值，同样不同位置代表不同字符串的比较  
            Console.WriteLine("差异步骤：" + dif[len1, len2]);
            //计算相似度  
            float similarity = 1 - (float)dif[len1, len2] / Math.Max(str1.Length, str2.Length);
            Console.WriteLine("相似度：" + similarity + " 越接近1越相似");
        }

        /// <summary>
        /// 得到最小值
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int min(params int[] num)
        {
            int min = Int32.MaxValue;
            foreach (var n in num)
            {
                if (min > n)
                {
                    min = n;
                }
            }
            return min;
        }

    }
}
