using System;
using System.Linq;

namespace ch.wuerth.tobias.ProcessPipeline
{
    public class ConditionalPipe : Pipe
    {
        private readonly IsTrueEvaluation _isTrueEvaluation;

        public delegate Boolean IsTrueEvaluation(dynamic obj);

        public ConditionalPipe(IsTrueEvaluation isTrueEvaluation)
        {
            _isTrueEvaluation = isTrueEvaluation;
        }

        public override Type GetTypeFrom()
        {
            return Previous.FirstOrDefault()?.GetTypeTo() ?? typeof(Object);
        }

        public override Type GetTypeTo()
        {
            return GetTypeFrom();
        }

        public override void Process(dynamic obj)
        {
            if (GetTypeFrom().IsInstanceOfType(obj))
            {
                if (_isTrueEvaluation(obj))
                {
                    Next.ForEach(x => x.Process(obj));
                }
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}