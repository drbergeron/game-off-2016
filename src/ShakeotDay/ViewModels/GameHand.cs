using ShakeotDay.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShakeotDay.ViewModels
{
    public class GameHand
    {
        public GameHand(Game gameIn, DiceHand handIn, int ShakeIn)
        {
            CurrentGame = gameIn;
            CurrentHand = handIn;
            ShakeOfTheDay = ShakeIn;
        }
        public Game CurrentGame {get;set;}
        public DiceHand CurrentHand { get; set; }
        public int ShakeOfTheDay { get; set; }
    }
}
