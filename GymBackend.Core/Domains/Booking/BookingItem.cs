namespace GymBackend.Core.Domains.Booking
{
    public class Instructor
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Time
    {
        public string Format_12_Hour { get; set; }
        public string Format_24_Hour { get; set; }
    }

    public class BookingItem
    {
        public int Bookings_Count { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Instructor> Instructors { get; set; }
        public Time Starts_At { get; set; }
        public Time Ends_At { get; set; }
    }
}
