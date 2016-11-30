using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShakeotDay.Core.Models
{
    public class DiceHand
    {
        

        public DiceHand()
        {
            Hand = new List<Dice>(5);
        }

        public DiceHand(List<Dice> diceIn, int roll)
        {
            Hand = diceIn;
            RollNumber = roll;
        }

        public List<Dice> Hand { get; set; }
        //used when coming back from ajax call
        public int? RollNumber { get; set; }

        /// <summary>
        /// Used to set the dice in the hand being held from roll to roll
        /// </summary>
        /// <param name="positionArrayIn"></param>
        public void holdDice(int[] positionArrayIn)
        {
            if(positionArrayIn.Length > 5)
            {
                throw new Exception("Invalid number of holds requested on hand.");
            }

            for(int i =0; i< positionArrayIn.Length; ++i)
            {
                Hand[i].holding = true;
            }
        }
        
        public void AddDieToHand(Dice dieIn)
        {
            Hand.Add(dieIn);
        }

        public void AddDiceToHand(List<Dice> diceIn)
        {
            Hand.AddRange(diceIn);
        }

        public void RollNonHeldDice()
        {
            foreach(var d in Hand)
            {
                if (!d.holding)
                {
                    d.Roll();
                }
            }
        }

        public void ClearHand()
        {
            Hand = new List<Dice>(5);
        }

        public void Fill(Random rndIn)
        {
            if (Hand.Count != 5)
            {
                var initCount = Hand.Count;
                for (int i = 0; i < 5 - initCount; ++i)
                {
                    Hand.Add(new Dice(rndIn.Next(1, 7)));
                }
            }
        }
    }
}
