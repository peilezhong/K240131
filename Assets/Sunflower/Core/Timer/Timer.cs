using System;
using Sunflower.Core;

namespace Sunflower.Common
{
    public class Timer: Singleton<Timer>, ISingletonAwake, ISingletonUpdate
    {
        private int timeZone;

        private DateTime dt1970;
        private DateTime dt;

        public int TimeZone
        {
            get
            {
                return this.timeZone;
            }
            set
            {
                this.timeZone = value;
                this.dt = dt1970.AddHours(TimeZone);
            }
        }
        
        public long ServerMinusClientTime { private get; set; }

        public long FrameTime { get; private set; }
        public void Awake()
        {
            this.dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this.dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this.FrameTime = this.ClientNow();
        }

        public void Update()
        {
            this.FrameTime = this.ClientNow();
        }

        public long ClientNow()
        {
            return (DateTime.UtcNow.Ticks - this.dt1970.Ticks) / 10000;
        }

        public long ServerNow()
        {
            return this.ClientNow() + this.ServerMinusClientTime;
        }

        public long ClientFrameTime()
        {
            return this.FrameTime;
        }

        public long ServerFrameTime()
        {
            return this.FrameTime + this.ServerMinusClientTime;
        }

        public long DateTime2ClientTime(DateTime d)
        {
            return (d.Ticks - dt.Ticks) / 10000;
        }
    }
}