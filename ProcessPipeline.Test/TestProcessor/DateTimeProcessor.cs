using System;
using System.Globalization;

namespace ch.wuerth.tobias.ProcessPipeline.Test.TestProcessor
{
    public class DateTimeProcessor : ProcessPipe<DateTime, String>
    {
        protected override String OnProcess(DateTime obj)
        {
            return obj.ToString("F", new CultureInfo("en-US"));
        }
    }
}