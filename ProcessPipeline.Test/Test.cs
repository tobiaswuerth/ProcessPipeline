using System;
using ch.wuerth.tobias.ProcessPipeline.pipes;
using ch.wuerth.tobias.ProcessPipeline.Test.TestProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ch.wuerth.tobias.ProcessPipeline.Test
{
    [ TestClass ]
    public class Test
    {
        [ TestMethod ]
        public void TestProcessFlow()
        {
            ProcessPipe<DateTime, Object> pipeline = new DateTimeProcessor().Connect(new LowerCaseProcessor())
                .Connect(new CasterPipe<String, Object>());

            Object output = pipeline.Process(new DateTime(2033, 8, 27, 18, 53, 22));

            Assert.AreEqual(output, "saturday, august 27, 2033 6:53:22 pm");
        }

        [ TestMethod ]
        public void TestConditionalTrue()
        {
            Int32 counter = 0;
            ProcessPipe<String, String> pipe = new ConditionalPipe<String, String>(String.IsNullOrEmpty
                , new EmptyPipe<String>(() => counter++)
                , new EmptyPipe<String>(() => throw new AssertFailedException("run although condition not met")));

            pipe.Process("");
            Assert.AreEqual(counter, 1);
        }

        [ TestMethod ]
        public void TestConditionalFalse()
        {
            Int32 counter = 0;
            ProcessPipe<String, String> pipe = new ConditionalPipe<String, String>(String.IsNullOrEmpty
                , new EmptyPipe<String>(() => throw new AssertFailedException("run although condition not met"))
                , new EmptyPipe<String>(() => counter++));

            pipe.Process("a");
            Assert.AreEqual(counter, 1);
        }
    }
}