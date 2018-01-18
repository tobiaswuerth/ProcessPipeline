namespace ch.wuerth.tobias.ProcessPipeline
{
    public interface IProcessReceiver<in T>
    {
        void Receive(T obj);
    }
}