using System;
using System.Collections.Generic;

namespace ch.wuerth.tobias.ProcessPipeline
{
    public abstract class Pipe
    {
        public List<Pipe> Previous { get; } = new List<Pipe>();
        public List<Pipe> Next { get; } = new List<Pipe>();

        public Pipe Connect(Pipe next)
        {
            if (null == next)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (!next.GetTypeFrom().IsAssignableFrom(GetTypeTo()))
            {
                throw new ArgumentException($"Cannot connect pipe with connector of type '{GetTypeTo()}' to pipe with connector of type '{next.GetTypeFrom()}'");
            }

            Next.Add(next);
            next.Previous.Add(this);
            return next;
        }

        public Pipe ConnectTo(Pipe previous)
        {
            if (null == previous)
            {
                throw new ArgumentNullException(nameof(previous));
            }

            if (!GetTypeFrom().IsAssignableFrom(previous.GetTypeTo()))
            {
                throw new ArgumentException($"Cannot connect pipe with connector of type '{GetTypeFrom()}' to pipe with connector of type '{previous.GetTypeTo()}'");
            }

            Previous.Add(previous);
            previous.Next.Add(this);
            return previous;
        }

        public abstract Type GetTypeFrom();
        public abstract Type GetTypeTo();
        public abstract void Process(dynamic obj);
    }
}