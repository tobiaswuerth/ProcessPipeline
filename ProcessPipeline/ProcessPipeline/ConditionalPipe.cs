using System;
using System.Linq;

namespace ch.wuerth.tobias.ProcessPipeline
{
    public class ConditionalPipe : Pipe
    {
        public delegate Boolean IsTrueEvaluation(dynamic obj);

        private readonly IsTrueEvaluation _isTrueEvaluation;

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
            return Next.FirstOrDefault()?.GetTypeFrom() ?? typeof(Object);
        }

        public override void Process(dynamic obj)
        {
            if (GetTypeFrom().IsAssignableFrom(obj) && _isTrueEvaluation(obj))
            {
                Next.ForEach(x => x.Process(obj));
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}