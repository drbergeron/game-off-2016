using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShakeotDay.Core.Models
{
    public enum GameTypeEnum
    {
        ShakeOfTheDay = 0
    }

    public enum GameWinType
    {
        loss = -1,
        three=1, //start @ 1 so losing games are 0
        four,
        five
    }
    public class Game
    {
        public long Id { get; set; }
        public GameTypeEnum TypeId { get; set; }
        public long UserId { get; set; }
        public int Year { get; set; }
        public int Day { get; set; }
        public int RollsTaken { get; set; }
        public bool isClosed { get; set; }
        public GameWinType isWinningGame { get; set; }
        public int winAmount { get; set; }
        public int AppliedToAccount { get; set; }
    }
}
