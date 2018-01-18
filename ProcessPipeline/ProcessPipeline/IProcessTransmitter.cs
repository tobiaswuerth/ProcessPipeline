namespace ch.wuerth.tobias.ProcessPipeline
{
    public interface IProcessTransmitter<in T>
    {
        void Transmit(T obj);
    }
}