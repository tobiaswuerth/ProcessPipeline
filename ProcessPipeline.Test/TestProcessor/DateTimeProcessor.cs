using System;
using System.Globalization;

namespace ch.wuerth.tobias.ProcessPipeline.Test.TestProcessor
{
    public class DateTimeProcessor : ProcessPipe<DateTime, String>
    {
        public DateTimeProcessor() : base(o => o.ToString("F", new CultureInfo("en-US"))) { }
    }
}