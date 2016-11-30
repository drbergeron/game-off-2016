using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShakeotDay.Core.Models
{
    public class GameType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RollsPerGame { get; set; }
        public int MaxPlaysPerDay { get; set; }
    }
}
