using System;
using Quobject.EngineIoClientDotNet.ComponentEmitter;

namespace RemindSME.Desktop.Helpers.Listeners
{
    public abstract class SocketListener : IListener
    {
        private static readonly Random Ids = new Random();
        private readonly int id;

        protected SocketListener()
        {
            id = Ids.Next();
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
