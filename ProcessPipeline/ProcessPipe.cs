using System;

namespace ch.wuerth.tobias.ProcessPipeline
{
    public class ProcessPipe<TIn, TOut>
    {
        private readonly Func<TIn, TOut> _onProcess;

        protected ProcessPipe(Func<TIn, TOut> onProcess)
        {
            _onProcess = onProcess;
        }

        public TOut Process(TIn obj)
        {
            return _onProcess.Invoke(obj);
        }

        public ProcessPipe<TIn, TAnother> Connect<TAnother>(ProcessPipe<TOut, TAnother> processPipe)
        {
            TAnother NewProcess(TIn o)
            {
                return processPipe._onProcess(_onProcess(o));
            }

            return new ProcessPipe<TIn, TAnother>(NewProcess);
        }
    }
}