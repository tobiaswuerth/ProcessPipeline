using System;
using System.Linq;
using ch.wuerth.tobias.ProcessPipeline.Test.TestProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ch.wuerth.tobias.ProcessPipeline.Test
{
    [ TestClass ]
    public class Test
    {
        [ TestMethod ]
        public void TestLinkingGood()
        {
            Pipe pipe1 = new DateTimeProcessor();
            Pipe pipe2 = new LowerCaseProcessor();

            pipe1.Connect(pipe2); // [DT|String] <-> [String|String]

            Assert.IsTrue(pipe1.Next.Contains(pipe2));
            Assert.IsTrue(pipe2.Previous.Contains(pipe1));
        }

        [ TestMethod ]
        public void TestLinkingGoodFluent()
        {
            Pipe pipe1 = new DateTimeProcessor();
            Pipe pipe3 = pipe1.Connect(new LowerCaseProcessor()).Connect(new Caster<Object>()); // [DT|String] <-> [String|String] <-> [String|Object]

            Assert.IsTrue(pipe1.Next.Contains(pipe3.Previous.First()));
            Assert.IsTrue(pipe3.Previous.First().Previous.Contains(pipe1));
            Assert.AreEqual(pipe1.Next.First(), pipe3.Previous.First());
        }

        [ TestMethod ]
        public void TestLinkingBad()
        {
            Pipe pipe1 = new DateTimeProcessor();
            Pipe pipe2 = new LowerCaseProcessor();
            try
            {
                pipe2.Connect(pipe1); // [String|String] <-> [DT|String]
                Assert.Fail("connecting pipe with invalid connector interfaces");
            }
            catch (Exception)
            {
                // good
            }
        }

        [ TestMethod ]
        public void TestEventTrigger()
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

            pipe1.Process(dt);
        }

        [ TestMethod ]
        public void TestProcessingFlow()
        {
            ProcessPipe<DateTime, String> pipe1 = new DateTimeProcessor();
            ProcessPipe<String, String> pipe2 = new LowerCaseProcessor();
            pipe1.Connect(pipe2);

            DateTime dt = new DateTime(2027, 11, 15, 7, 45, 8);
            pipe2.OnProcessFinished += result => Assert.AreEqual(result, "monday, november 15, 2027 7:45:08 am");

            pipe1.Process(dt);
        }

        [ TestMethod ]
        public void TestProcessingArgumentValidation()
        {
            ProcessPipe<DateTime, String> pipe1 = new DateTimeProcessor();
            ProcessPipe<String, String> pipe2 = new LowerCaseProcessor();
            pipe1.Connect(pipe2);

            Pipe p = pipe1;

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

        [ TestMethod ]
        public void TestMultiLinking()
        {
            Pipe pipe1 = new DateTimeProcessor();
            Pipe pipe21 = new LowerCaseProcessor();
            Pipe pipe22 = new Caster<Object>();

            pipe1.Connect(pipe21);
            pipe1.Connect(pipe22);

            Assert.IsTrue(pipe1.Next.Contains(pipe21));
            Assert.IsTrue(pipe1.Next.Contains(pipe22));
            Assert.IsTrue(pipe21.Previous.Count.Equals(1));
            Assert.IsTrue(pipe22.Previous.Count.Equals(1));
            Assert.AreEqual(pipe21.Previous.First(), pipe1);
            Assert.AreEqual(pipe22.Previous.First(), pipe1);
        }

        [ TestMethod ]
        public void TestConditionalPipe()
        {
            Pipe pipe1 = new LowerCaseProcessor();
            Pipe pipe2 = new ConditionalPipe(o => !String.IsNullOrEmpty(o));
            LowerCaseProcessor pipe3 = new LowerCaseProcessor();
            pipe1.Connect(pipe2).Connect(pipe3);
            pipe3.OnProcessStarted += from => { };
        }

        [ TestMethod ]
        public void TestConditionalPipeNot()
        {
            Pipe pipe1 = new LowerCaseProcessor();
            Pipe pipe2 = new ConditionalPipe(o => !String.IsNullOrEmpty(o));
            LowerCaseProcessor pipe3 = new LowerCaseProcessor();
            pipe1.Connect(pipe2).Connect(pipe3);
            Boolean run = false;
            pipe3.OnProcessStarted += from => run = true;
            pipe1.Process("a");
            Assert.IsTrue(run);
        }
    }
}