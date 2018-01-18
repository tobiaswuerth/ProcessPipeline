using System;

namespace ch.wuerth.tobias.ProcessPipeline.Test.TestProcessor
{
    public class LowerCaseProcessor : ProcessSegment<String, String>
    {
        protected override String OnProcess(String obj)
        {
            return obj?.ToLower();
        }
    }
}