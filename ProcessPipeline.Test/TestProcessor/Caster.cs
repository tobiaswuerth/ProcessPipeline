namespace ch.wuerth.tobias.ProcessPipeline.Test.TestProcessor
{
    public class Caster<T> : ProcessPipe<dynamic, T>
    {
        protected override T OnProcess(dynamic obj)
        {
            return (T) obj;
        }
    }
}