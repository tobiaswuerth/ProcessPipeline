using System;

namespace ch.wuerth.tobias.ProcessPipeline
{
    public abstract class ProcessPipe<TFrom, TTo> : Procedure, IPipeConnector<TFrom>
    {
        public delegate void ProcessFinishedEvent(TTo to);

        public delegate void ProcessStartedEvent(TFrom from);

        public IPipeConnector<TTo> Next { get; private set; }

        public void Take(TFrom obj)
        {
            OnProcessStarted?.Invoke(obj);
            TTo processed = OnProcess(obj);
            OnProcessFinished?.Invoke(processed);
            Next?.Take(processed);
        }

        public void Connect(IPipeConnector<TTo> next)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public event ProcessFinishedEvent OnProcessFinished;
        public event ProcessStartedEvent OnProcessStarted;

        protected abstract TTo OnProcess(TFrom obj);

        public override void Process(dynamic obj)
        {
            if (obj is TFrom oFrom)
            {
                Take(oFrom);
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}