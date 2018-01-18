namespace ch.wuerth.tobias.ProcessPipeline
{
    public interface IPipeConnector<in T>
    {
        void Take(T obj);
    }
}