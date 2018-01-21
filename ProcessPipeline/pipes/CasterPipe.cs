namespace ch.wuerth.tobias.ProcessPipeline.pipes
{
    public class CasterPipe<TIn, TOut> : ProcessPipe<TIn, TOut> where TOut : class
    {
        public CasterPipe() : base(o => o as TOut) { }
    }
}