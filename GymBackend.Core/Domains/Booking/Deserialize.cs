namespace GymBackend.Core.Domains.Booking
{
    public class Deserialize
    {
        public class TimetableRoot
        {
            public List<BookingItem> Data { get; set; }
        }

        public class BookedRoot
        {
            public List<BookedItem> Data { get; set; }
        }
    }
}
