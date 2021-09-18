using JiebaNet.Segmenter;
using JiebaNet.Segmenter.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Framework.Interface;
using Robot.Framework.Models;
using System;
using System.Collections.Generic;
using System.DrawingCore.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit.Sdk;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace TestProject
{
    [TestClass]
    public class UnitTest1 : BaseUt
    {
        readonly ISignUserServices signUserServices;
        readonly ISignLogsServices signLogsServices;
        readonly ILianChatServices lianChatServices;
        readonly ILianKeyWordsServices lianKeyWordsServices; 
        readonly ISpeakerServices speakerServices;


        public UnitTest1()
        {
            signUserServices = GetInstance<ISignUserServices>();
            signLogsServices = GetInstance<ISignLogsServices>();
            lianChatServices = GetInstance<ILianChatServices>();
            lianKeyWordsServices = GetInstance<ILianKeyWordsServices>();
            speakerServices = GetInstance<ISpeakerServices>();
        }

        [TestMethod]
        public void UTCounterByJieba()
        {
            var s = "在数学和计算机科学之中，算法（algorithm）为任何良定义的具体计算步骤的一个序列，常用于计算、数据处理和自动推理。精确而言，算法是一个表示为有限长列表的有效方法。算法应包含清晰定义的指令用于计算函数。";
            var seg = new JiebaSegmenter();
            var freqs = new Counter<string>(seg.Cut(s));
            var wordKeys = new List<string>();
            var ints = new List<int>();
            var filterFreqs = freqs.Count >= 20 ? freqs?.MostCommon(20) : freqs?.MostCommon(freqs.Count - 1);
            foreach (var pair in filterFreqs)
            {
                wordKeys.Add(pair.Key);
                ints.Add(pair.Value);
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
            var WordCloudGen = new WordCloudSharp.WordCloud(300, 300, true);
            //var images = WordCloudGen.Draw(wordKeys, ints);
            var images = WordCloudGen.Draw(wordKeys, ints);
            //var images = WordCloudGen.Draw(@"C:\Users\v-jinlv\Desktop\background.jpg", wordKeys, ints);
            images.Save($"D:\\{Guid.NewGuid()}.png", ImageFormat.Png);
        }


        [TestMethod]
        public void UTGroupWC()
        {
            try
            {
                // && s.GroupId == 566040141
                var speakerLists = speakerServices.Query(s => s.Uid == 1069430666);
                if (speakerLists.Any())
                {
                    var builder = string.Join(",", speakerLists.Select(s => s.RawText))
                                    .Replace(",", "");
                    var seg = new JiebaSegmenter();
                    var freqs = new Counter<string>(seg.Cut(builder));
                    var filterFreqs = freqs.Count >= 20 ? freqs?.MostCommon(20) : freqs?.MostCommon(freqs.Count - 1);
                    var WordCloudGen = new WordCloudSharp.WordCloud(300, 300, true);
                    var images = WordCloudGen
                        .Draw(filterFreqs.Select(s => s.Key).ToList(), filterFreqs.Select(s => s.Value).ToList());
                    var imgName = $"{Environment.CurrentDirectory}\\Images\\{Guid.NewGuid()}.png";
                    images.Save(imgName, ImageFormat.Png);
                    //delete img
                     //Task.Delay(10);
                    //File.Delete(imgName);
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            var list = new List<string>() { "22", "33" }.ToArray();
            Console.WriteLine(string.Join(",", list).Replace(",", ""));

            foreach (ContentEnum item in Enum.GetValues(typeof(ContentEnum)))
            {
                var t = GetT(item).Name is nameof(ContentEnum);
                string strName = Enum.GetName(typeof(ContentEnum), item);//获取名称
                string strVaule = item.ToString();//获取值
            }
        }

        private Type GetT<T>(T t) => typeof(T);

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
                var t = lianKeyWordsServices.Query(t => t.Status == Status.Valid);
                var t1 = signUserServices.Query(t => t.Status == Status.Valid);
                var t2 = lianChatServices.Query(t => t.Status == Status.Valid);
                var t3 = signUserServices.Query(t => t.QNumber.Equals("1069430666"));
                signLogsServices.DeleteById(t => t.ID > 0);
                // await signLogsServices.DeleteById(2);
                //var t = await signLogsServices.Query(t => t.ID > 0);
            }
            catch (Exception c)
            {

            }
        }

        public class Input
        {
            var lianAss = typeof(ILianInterface);
            var lianAss2 = typeof(ILianInterface).GetMethods().ToArray();
            foreach (var item in lianAss.GetMethods())
            {
                var s = item.GetCustomAttribute(typeof(KeyWordAttribute));
                if (s != null)
                {
                }
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

        abstract class CmdBase<T>
        {
            protected abstract string Name { get; }
            protected string _traceId;
            private string _age;
            public CmdBase([CallerMemberName] string age = null)
            {
                _age = age;
                _traceId = Guid.NewGuid().ToString();
            }

            protected abstract string GenerateS(string G);
            public virtual void Voice() => Console.WriteLine("the voice of water");
        }

        class Test : CmdBase<Input>
        {
            protected override string Name => "Test name";
            public Test(string age) : base(age)
            {

            }

            protected override string GenerateS(string G) => "generate s by override ";
            public override void Voice()
            {
                Console.WriteLine($"{_traceId} the voice of {nameof(Test)}");
            }
        }

        interface IA
        {
            string GenerateS(string G);
            abstract void Voice();
        }
        class B : IA
        {
            public string GenerateS(string G) => "generate s by interface";
            public void Voice()
            {
                throw new NotImplementedException();
            }
        }
    }
}
