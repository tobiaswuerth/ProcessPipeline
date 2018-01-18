using System;

namespace ch.wuerth.tobias.ProcessPipeline
{
    public abstract class ProcessSegment<TFrom, TTo> : IProcessReceiver<TFrom>
    {
        public delegate void ProcessFinishedEvent(TTo to);

        public delegate void ProcessStartedEvent(TFrom from);

        public IProcessReceiver<TTo> Next { get; private set; }

        public void Receive(TFrom obj)
        {
            OnProcessStarted?.Invoke(obj);
            TTo processed = OnProcess(obj);
            OnProcessFinished?.Invoke(processed);
            Next?.Receive(processed);
        }

        public void Append(IProcessReceiver<TTo> next)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public event ProcessFinishedEvent OnProcessFinished;
        public event ProcessStartedEvent OnProcessStarted;

        protected abstract TTo OnProcess(TFrom obj);
    }
}