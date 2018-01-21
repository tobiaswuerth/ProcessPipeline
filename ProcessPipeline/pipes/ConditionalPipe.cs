using System;

namespace ch.wuerth.tobias.ProcessPipeline.pipes
{
    public class ConditionalPipe<TIn, TOut> : ProcessPipe<TIn, TOut> where TOut : class
    {
        public ConditionalPipe(Func<TIn, Boolean> condition, ProcessPipe<TIn, TOut> onTrue, ProcessPipe<TIn, TOut> onFalse) :
            base(o => condition(o) ? onTrue?.Process(o) : onFalse?.Process(o)) { }
    }
}