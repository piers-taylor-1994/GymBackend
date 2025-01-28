using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymBackend.Core.Domains.Workouts
{
    public class Swimming
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public int Lengths { get; set; }
        public int TimeSwimming { get; set; }
        public bool Review { get; set; }
        public string? Explanation { get; set; }
        public Distances Distances => new(Lengths, TimeSwimming);

        

        //public Swimming(Guid userId, DateTime date, int lengths, int timeSwimming, bool review, string explanation)
        //{
        //    UserId = userId;
        //    Date = date;
        //    Lengths = lengths;
        //    TimeSwimming = timeSwimming;
        //    Review = review;
        //    Explanation = explanation;

        //}
    }
    
    public class Distances
    {
        public int Meters { get; set; }
        public double Kilometers { get; set; }
        public double Yards { get; set; }
        public double Miles { get; set; }
        public double Mph {  get; set; }
        public double Kph {  get; set; }

        public Distances(int lengths, int timeSwimming)
        {
            Meters = lengths * 25;
            Kilometers = Meters * 0.001;
            Yards = Meters * 1.094;
            Miles = Math.Round(Kilometers / 1.609, 2);

            double perHour = (double)60 / timeSwimming;
            
            Mph = Math.Round(Miles * perHour, 2);
            

            Kph = Math.Round(Kilometers * perHour, 2);
            
        }
    }
}
