namespace RemindSME.Desktop.Events
{
    public class NetworkCountChangeEvent
    {
        public NetworkCountChangeEvent(int count)
        {
            Count = count;
        }

        public int Count { get; }
    }
}
