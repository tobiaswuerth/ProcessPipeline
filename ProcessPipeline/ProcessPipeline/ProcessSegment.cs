using System;

namespace ch.wuerth.tobias.ProcessPipeline
{
    public abstract class ProcessSegment<TFrom, TTo> : IProcessReceiver<TFrom>
    {
        public IProcessReceiver<TTo> Next { get; private set; }

        public void Receive(TFrom obj)
        {
            TTo processed = OnProcess(obj);
            Next?.Receive(processed);
        }

        public void Append(IProcessReceiver<TTo> next)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
        }

        protected abstract TTo OnProcess(TFrom obj);
    }
}