namespace RemindSME.Desktop.Events
{
    public class NetworkAddressChangedEvent
    {
        public NetworkAddressChangedEvent(bool isWorkNetwork)
        {
            IsWorkNetwork = isWorkNetwork;
        }

        public bool IsWorkNetwork { get; }
    }
}
