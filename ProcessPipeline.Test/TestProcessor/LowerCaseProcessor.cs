using System;

namespace ch.wuerth.tobias.ProcessPipeline.Test.TestProcessor
{
    public class LowerCaseProcessor : ProcessPipe<String, String>
    {
        public LowerCaseProcessor() : base(o => o.ToLower()) { }
    }
}