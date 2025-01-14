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
        public DateTime DateTime { get; set; }
        public int Lengths { get; set; }
        public int TimeSwimming { get; set; }
        public bool Review { get; set; }
        public string? Explanation { get; set; }
    }
}
