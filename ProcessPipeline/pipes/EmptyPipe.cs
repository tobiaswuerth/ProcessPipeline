using System;

namespace ch.wuerth.tobias.ProcessPipeline.pipes
{
    public class EmptyPipe<T> : ProcessPipe<T, T>
    {
        public EmptyPipe() : base(o => o) { }

        public EmptyPipe(Action flowAction) : base(o =>
        {
            flowAction?.Invoke();
            return o;
        }) { }
    }
}