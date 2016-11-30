using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShakeotDay.Core.Models;

namespace ShakeotDay.API.Models
{
    public class GameResult
    {
        public bool isWinningGame { get; set; }
        public GameWinType winType { get; set; }
    }
}
