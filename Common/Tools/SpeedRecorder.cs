using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Tools
{
    /// <summary>
    /// 提供计算速率、流量控制的功能。
    /// </summary>
    public class SpeedRecorder
    {
        /// <summary>
        /// 根据当前字节发送速率和数据帧大小，返回一个适宜的新大小。
        /// </summary>
        /// <returns></returns>
        public static int UpdateFrameSize(int speed, int size)
        {
            if (speed / size > 80)
            {
                size = (int)(size * Math.Sqrt(2));
            }
            else if (speed / size < 40)
            {
                size = (int)(size / Math.Sqrt(2));
            }
            return size;
        }

        /// <summary>
        /// 获取按毫秒计的速率。
        /// </summary>
        public long SpeedPerMilisecond
        {
            get
            {
                RemoveExpiredRecords();
                if (Records.Last == null)
                {
                    return 0;
                }
                else
                {
                    int time = (DateTime.Now - Records.First.Value.Item2).Milliseconds;
                    long value = Records.Last.Value.Item1 - Records.First.Value.Item1;

                    if (time == 0) return value;
                    else return value / time;
                }
            }
        }

        /// <summary>
        /// 获取按秒计的速率。
        /// </summary>
        public long SpeedPerSecond
        {
            get
            {
                RemoveExpiredRecords();
                if (Records.Last == null)
                {
                    return 0;
                }
                else
                {
                    int time = (DateTime.Now - Records.First.Value.Item2).Seconds;
                    long value = Records.Last.Value.Item1 - Records.First.Value.Item1;

                    if (time == 0) return value;
                    else return value / time;
                }
            }
        }

        /// <summary>
        /// 记录当前时间下的值。
        /// </summary>
        /// <param name="value"></param>
        public void Record(long value)
        {
            Records.AddLast(Tuple.Create(value, DateTime.Now));
            RemoveExpiredRecords();
        }

        /// <summary>
        /// 以默认的时间窗口大小创建新的实例。
        /// </summary>
        public SpeedRecorder()
            :this(TimeSpan.FromSeconds(3))
        {

        }

        /// <summary>
        /// 以给定的时间窗口大小创建新的实例。
        /// </summary>
        public SpeedRecorder(TimeSpan window)
        {
            Window = window;
        }

        /// <summary>
        /// 时间窗口大小。
        /// </summary>
        protected TimeSpan Window { get; }

        /// <summary>
        /// 记录列表。
        /// </summary>
        protected LinkedList<Tuple<long, DateTime>> Records { get; } = new LinkedList<Tuple<long, DateTime>>();

        /// <summary>
        /// 从<code>Records</code>头部移除已超出时间窗口的节点。
        /// </summary>
        protected void RemoveExpiredRecords()
        {
            while (Records.First != null && DateTime.Now - Records.First.Value.Item2 > Window)
            {
                Records.RemoveFirst();
            }
        }
    }
}
