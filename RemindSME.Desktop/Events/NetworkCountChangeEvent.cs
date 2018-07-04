namespace RemindSME.Desktop.Events
{
    public class NetworkCountChangeEvent
    {
        public int Count { get; }

        public NetworkCountChangeEvent(int count)
        {
            Count = count;
        }
    }
}
