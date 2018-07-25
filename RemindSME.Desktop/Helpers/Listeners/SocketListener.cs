using System;
using Caliburn.Micro;
using Quobject.EngineIoClientDotNet.ComponentEmitter;

namespace RemindSME.Desktop.Helpers.Listeners
{
    public abstract class SocketListener : IListener
    {
        private static readonly Random Ids = new Random();

        protected readonly IEventAggregator EventAggregator;

        private readonly int id;

        protected SocketListener(IEventAggregator eventAggregator)
        {
            id = Ids.Next();
            EventAggregator = eventAggregator;
        }

        public abstract void Call(params object[] args);

        public int GetId()
        {
            return id;
        }

        public int CompareTo(IListener other)
        {
            return GetId().CompareTo(other?.GetId() ?? -1);
        }
    }
}
