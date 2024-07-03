using GymBackend.Core.Contracts.Booking;
using GymBackend.Core.Domains.Booking;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace GymBackend.Service.Booking
{
    public class BookingService : IBookingService
    {
        private readonly HttpClient httpClient;

        public BookingService(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        private readonly string auth = "Bearer v4.local.uk5TgNXsHIaK9lQ7dxNagw5YH-aShddDrzF_-VojO8fAYKiyhoZsEJP5vGrEl6cyfCkJVNcVmnBHXae2TAbA2nkHk3sIgVHSW4PazLADp8a55w07R-5agYwY_FBZ2wPe0hc_6Gdmqda_iI1pXJWYO40VRg-5bWPKrMq8mGM--RbABGzBeVy2JaIut4QQ-OvXy5vx0lJQDuwOxjI1";
        private readonly string origin = "https://bookings.better.org.uk";

        public async Task<List<BookingItem>> GetTimetable()
        {
            var date = DateTime.UtcNow;
            var requestMessages = new List<HttpRequestMessage>();
            var bookings = new List<BookingItem>();

            // This is for every wednesday for x weeks
            for (var i = 0; i < 3; i++)
            {
                int daysToAdd = ((int)DayOfWeek.Wednesday - (int)DateTime.Today.DayOfWeek + 7) % 7;
                var nextWednesday = DateTime.Today.AddDays(daysToAdd + i * 7);
                var newRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("https://better-admin.org.uk/api/activities/venue/the-hive-leisure-centre/activity/fitness-classes-c/timetable?date=" + nextWednesday.ToString("yyyy-MM-dd")));

                newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Bearer v4.local.uk5TgNXsHIaK9lQ7dxNagw5YH-aShddDrzF_-VojO8fAYKiyhoZsEJP5vGrEl6cyfCkJVNcVmnBHXae2TAbA2nkHk3sIgVHSW4PazLADp8a55w07R-5agYwY_FBZ2wPe0hc_6Gdmqda_iI1pXJWYO40VRg-5bWPKrMq8mGM--RbABGzBeVy2JaIut4QQ-OvXy5vx0lJQDuwOxjI1");
                newRequest.Headers.Add("Origin", "https://bookings.better.org.uk");

                requestMessages.Add(newRequest);
            }

            //This is for the next 7 days
            //for (var i = 0; i < 8; i++)
            //{
            //    var newRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("https://better-admin.org.uk/api/activities/venue/the-hive-leisure-centre/activity/fitness-classes-c/timetable?date=" + date.AddDays(i).ToString("yyyy-MM-dd")));
            //    newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Bearer v4.local.uk5TgNXsHIaK9lQ7dxNagw5YH-aShddDrzF_-VojO8fAYKiyhoZsEJP5vGrEl6cyfCkJVNcVmnBHXae2TAbA2nkHk3sIgVHSW4PazLADp8a55w07R-5agYwY_FBZ2wPe0hc_6Gdmqda_iI1pXJWYO40VRg-5bWPKrMq8mGM--RbABGzBeVy2JaIut4QQ-OvXy5vx0lJQDuwOxjI1");
            //    newRequest.Headers.Add("Origin", "https://bookings.better.org.uk");

            //    requestMessages.Add(newRequest);
            //}

            foreach (var request in requestMessages)
            {
                var response = await httpClient.SendAsync(request, CancellationToken.None);
                string responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Deserialize.TimetableRoot>(responseString);
                if (result != null && result.Data != null) bookings = bookings.Concat(result.Data.Where(r => r.Spaces > 0 && r.Name.Contains("yoga", StringComparison.CurrentCultureIgnoreCase))).ToList();
            }

            return bookings;
        }

        public async Task<string> CreateBookingAsync(Guid userId, int bookingId)
        {
            bool check1; bool check2;
            try
            {
                if (userId != new Guid("9F15FA88-844E-480C-9440-C7290EE31115")) throw new Exception("Not authorised to call this function");

                check1 = await AddToBasket(bookingId);
                check2 = await Checkout();
            }
            catch (Exception ex)
            {
                throw new Exception("Booking error", ex);
            }

            if (check1 && check2) return "Successfully booked!";
            return "Booking failed";
        }

        public async Task<List<int>> GetBookedClassesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://better-admin.org.uk/api/my-account/bookings?filter=future"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth);
            request.Headers.Add("Origin", origin);

            var response = await httpClient.SendAsync(request, CancellationToken.None);
            string responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Deserialize.BookedRoot>(responseString);

            return result?.Data.Select(r => r.Activity_Id).ToList() ?? new List<int>();
        }

        private async Task<bool> AddToBasket(int bookingId)
        {
            var url = new Uri("https://better-admin.org.uk/api/activities/cart/add");

            string body = "{\"items\":[{\"id\":\"" + bookingId + "\",\"type\":\"activity\",\"pricing_option_id\":1,\"apply_benefit\":true,\"activity_restriction_ids\":[]}],\"membership_user_id\":849218,\"selected_user_id\":null}";

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth);
            request.Headers.Add("Origin", origin);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request, CancellationToken.None);

            return response.IsSuccessStatusCode;
        }

        private async Task<bool> Checkout()
        {
            var url = new Uri("https://better-admin.org.uk/api/checkout/complete");

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth);
            request.Headers.Add("Origin", origin);
            request.Content = new StringContent("{\"completed_waivers\":[],\"payments\":[],\"selected_user_id\":null,\"source\":\"activity-booking\",\"terms\":[1]}", Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request, CancellationToken.None);

            return response.IsSuccessStatusCode;
        }
    }
}
