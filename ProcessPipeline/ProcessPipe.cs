using System;

namespace ch.wuerth.tobias.ProcessPipeline
{
    public abstract class ProcessPipe<TFrom, TTo> : Pipe
    {
        public delegate void ProcessFinishedEvent(TTo to);

        public delegate void ProcessStartedEvent(TFrom from);

        private void Handle(TFrom obj)
        {
            OnProcessStarted?.Invoke(obj);
            TTo processed = OnProcess(obj);
            OnProcessFinished?.Invoke(processed);
            Next?.ForEach(x => x.Process(processed));
        }

        public event ProcessFinishedEvent OnProcessFinished;
        public event ProcessStartedEvent OnProcessStarted;

        protected abstract TTo OnProcess(TFrom obj);

        public override Type GetTypeFrom()
        {
            return typeof(TFrom);
        }

        public override Type GetTypeTo()
        {
            return typeof(TTo);
        }

        public override void Process(dynamic obj)
        {
            if (typeof(TFrom).IsAssignableFrom(obj))
            {
                Handle(obj);
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}