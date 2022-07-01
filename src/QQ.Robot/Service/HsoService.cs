using Robot.Common;
using Robot.Common.Interface;
using Sora.Entities;
using Sora.Entities.Segment;
using Sora.EventArgs.SoraEvent;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace QQ.RoBot
{
    /// <summary>
    /// hso实现
    /// </summary>
    public class HsoService : IHsoInterface
    {
        #region Property
        readonly ILogsInterface _logs;

        public HsoService(ILogsInterface logs)
        {
            _logs = logs;
        }

        #endregion

        #region Public Func

        #region Hso
        /// <summary>
        /// hso
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async ValueTask Hso(GroupMessageEventArgs e)
        {
            await GeneaterHso(e.SourceGroup);
        }
        #endregion

        #endregion

        #region Private Func

        #region Hso
        /// <summary>
        /// <para>从色图源获取色图</para>
        /// <para>不会支持R18的哦</para>
        /// </summary>
        private async Task GeneaterHso(Sora.Entities.Group group)
        {
            //网络部分
            try
            {
                _logs.Info("NET", "尝试获取色图");
                string serverUrl;
                //源切换
                serverUrl = "https://api.lolicon.app/setu/v2";
                //向服务器发送请求
                var json = await HttpHelper.HttpGetAsync($"{serverUrl}");
                var result = JsonSerializer.Deserialize<HsoResult>(json);
                if (result != null && result.Code == 0)
                {
                    await group.SendGroupMessage(GetResult(result));
                    var msg = new MessageBody()
                    {
                        SoraSegment.Image($"{result.Data.First()?.Url}")
                    };
                    await group.SendGroupMessage(msg);
                }
                else
                    await group.SendGroupMessage("发生了未知错误");
                _logs.Debug(new Exception(), json);
            }
            catch (Exception e)
            {
                //网络错误
                await group.SendGroupMessage("网络错误，暂时没有请求到~");
                _logs.Error(e, "");
                return;
            }
            //https://dotnet.microsoft.com/download/dotnet/current/runtime
        }

        private static string GetResult(HsoResult hsoResponse)
        {
            var strBulider = new StringBuilder();
            var tags = hsoResponse.Data.First().Tags;
            strBulider.Append($"[画师]：{hsoResponse.Data.First().Author}\r\n");
            strBulider.Append($"[标题]：{hsoResponse.Data.First().Title}\r\n");
            strBulider.Append($"[Pid]：{hsoResponse.Data.First().Pid}\r\n");
            strBulider.Append($"[画师ID]：{hsoResponse.Data.First().Uid}\r\n");
            strBulider.Append($"[标签]：{string.Join(",", tags)}\r\n");
            return strBulider.ToString();
        }

        #endregion

        #endregion
    }
}
