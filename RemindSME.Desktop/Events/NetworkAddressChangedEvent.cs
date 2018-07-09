namespace RemindSME.Desktop.Events
{
    public class NetworkAddressChangedEvent
    {
        public bool IsWorkNetwork { get; }

        public NetworkAddressChangedEvent(bool isWorkNetwork)
        {
            IsWorkNetwork = isWorkNetwork;
        }
    }
}
