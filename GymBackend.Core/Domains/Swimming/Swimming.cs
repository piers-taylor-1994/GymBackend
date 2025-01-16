using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymBackend.Core.Domains.Swimming
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

        public Swimming(Guid userId, DateTime date, int lengths, int timeSwimming, bool review, string explanation)
        {
            UserId = userId;
            Date = date;
            Lengths = lengths;
            TimeSwimming = timeSwimming;
            Review = review;
            Explanation = explanation;

        }
    }
}
