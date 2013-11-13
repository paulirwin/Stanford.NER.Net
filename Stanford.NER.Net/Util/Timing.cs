using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public class Timing
    {
        private long start;
        private static long startTime = DateTime.UtcNow.CurrentTimeMillis();
        private static readonly NumberFormatInfo nf = new NumberFormatInfo() { NumberDecimalDigits = 1, NumberDecimalSeparator = "." };
        public Timing()
        {
            this.Start();
        }

        public virtual void Start()
        {
            start = DateTime.UtcNow.CurrentTimeMillis();
        }

        public virtual long Report()
        {
            return DateTime.UtcNow.CurrentTimeMillis() - start;
        }

        public virtual long Report(string str, TextWriter stream)
        {
            long elapsed = this.Report();
            stream.WriteLine(str + @" Time elapsed: " + (elapsed) + @" ms");
            return elapsed;
        }

        public virtual long Report(string str)
        {
            return this.Report(str, Console.Error);
        }

        public virtual string ToSecondsString()
        {
            return ToSecondsString(Report());
        }

        public static string ToSecondsString(long elapsed)
        {
            return (((double)elapsed) / 1000).ToString(nf);
        }

        public virtual long Restart()
        {
            long elapsed = this.Report();
            this.Start();
            return elapsed;
        }

        public virtual long Restart(string str, TextWriter stream)
        {
            long elapsed = this.Report(str, stream);
            this.Start();
            return elapsed;
        }

        public virtual long Restart(string str)
        {
            return this.Restart(str, Console.Error);
        }

        public virtual long Stop()
        {
            long elapsed = this.Report();
            this.start = 0;
            return elapsed;
        }

        public virtual long Stop(string str, TextWriter stream)
        {
            this.Report(str, stream);
            return this.Stop();
        }

        public virtual long Stop(string str)
        {
            return Stop(str, Console.Error);
        }

        public static void StartTime()
        {
            startTime = DateTime.UtcNow.CurrentTimeMillis();
        }

        public static long EndTime()
        {
            return DateTime.UtcNow.CurrentTimeMillis() - startTime;
        }

        public static long EndTime(string str, TextWriter stream)
        {
            long elapsed = EndTime();
            stream.WriteLine(str + @" Time elapsed: " + (elapsed) + @" ms");
            return elapsed;
        }

        public static long EndTime(string str)
        {
            return EndTime(str, Console.Error);
        }

        public virtual void Doing(string str)
        {
            Console.Error.Write(str);
            Console.Error.Write(@" ... ");
            Console.Error.Flush();
            Start();
        }

        public virtual void Done()
        {
            Console.Error.WriteLine(@"done [" + ToSecondsString() + @" sec].");
        }

        public virtual void Done(string msg)
        {
            Console.Error.WriteLine(msg + @" done [" + ToSecondsString() + @" sec].");
        }

        public static void StartDoing(string str)
        {
            Console.Error.Write(str);
            Console.Error.Write(@" ... ");
            Console.Error.Flush();
            StartTime();
        }

        public static void EndDoing()
        {
            long elapsed = DateTime.UtcNow.CurrentTimeMillis() - startTime;
            Console.Error.WriteLine(@"done [" + (((double)elapsed) / 1000).ToString(nf) + @" sec].");
        }

        public static void EndDoing(string msg)
        {
            long elapsed = DateTime.UtcNow.CurrentTimeMillis() - startTime;
            Console.Error.WriteLine(msg + @" done [" + (((double)elapsed) / 1000).ToString(nf) + @" sec].");
        }

        public static long Tick()
        {
            long elapsed = DateTime.UtcNow.CurrentTimeMillis() - startTime;
            StartTime();
            return elapsed;
        }

        public static long Tick(string str, TextWriter stream)
        {
            long elapsed = Tick();
            stream.WriteLine(str + @" Time elapsed: " + (elapsed) + @" ms");
            return elapsed;
        }

        public static long Tick(string str)
        {
            return Tick(str, Console.Error);
        }

        public override string ToString()
        {
            return @"Timing[start=" + startTime + @"]";
        }
    }
}
