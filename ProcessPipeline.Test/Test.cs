using System;
using ch.wuerth.tobias.ProcessPipeline.Test.TestProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ch.wuerth.tobias.ProcessPipeline.Test
{
    [ TestClass ]
    public class Test
    {
        [ TestMethod ]
        public void Test1Linking()
        {
            ProcessPipe<DateTime, String> pipe1 = new DateTimeProcessor();
            ProcessPipe<String, String> pipe2 = new LowerCaseProcessor();

            pipe1.Connect(pipe2);

            Assert.AreEqual(pipe1.Next, pipe2);
        }

        [ TestMethod ]
        public void Test2EventTrigger()
        {
            ProcessPipe<DateTime, String> pipe1 = new DateTimeProcessor();
            ProcessPipe<String, String> pipe2 = new LowerCaseProcessor();
            pipe1.Connect(pipe2);

            DateTime dt = DateTime.Now;
            Int32 eventCounter = 0;
            pipe1.OnProcessStarted += _ =>
            {
                eventCounter++;
                Assert.AreEqual(eventCounter, 1);
            };
            pipe1.OnProcessFinished += _ =>
            {
                eventCounter++;
                Assert.AreEqual(eventCounter, 2);
            };
            pipe2.OnProcessStarted += _ =>
            {
                eventCounter++;
                Assert.AreEqual(eventCounter, 3);
            };
            pipe2.OnProcessFinished += _ =>
            {
                eventCounter++;
                Assert.AreEqual(eventCounter, 4);
            };

            pipe1.Take(dt);
        }

        [ TestMethod ]
        public void Test3Processing()
        {
            ProcessPipe<DateTime, String> pipe1 = new DateTimeProcessor();
            ProcessPipe<String, String> pipe2 = new LowerCaseProcessor();
            pipe1.Connect(pipe2);

            DateTime dt = new DateTime(2027, 11, 15, 7, 45, 8);
            pipe2.OnProcessFinished += result => Assert.AreEqual(result, "monday, november 15, 2027 7:45:08 am");

            pipe1.Take(dt);
        }

        [ TestMethod ]
        public void Test4Processing()
        {
            ProcessPipe<DateTime, String> pipe1 = new DateTimeProcessor();
            ProcessPipe<String, String> pipe2 = new LowerCaseProcessor();
            pipe1.Connect(pipe2);

            Procedure p = pipe1;

            DateTime dt = new DateTime(2027, 11, 15, 7, 45, 8);
            pipe2.OnProcessFinished += result => Assert.AreEqual(result, "monday, november 15, 2027 7:45:08 am");

            p.Process(dt);

            try
            {
                p.Process(1);
                Assert.Fail("processing invalid type without exception");
            }
            catch (Exception)
            {
                Console.WriteLine("Exception was thrown for invalid type, this is good");
                // good
            }
        }
    }
}