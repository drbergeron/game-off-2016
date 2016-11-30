using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShakeotDay.Core.Models
{
    public class DiceRoll
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int RollValue { get; set; }
        public long GameId { get; set; }
        public long GameRollNumber { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
