using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShakeotDay.Core.Models
{
    public class Dice 
    {
        private Random roll;
 
        /// <summary>
        /// used by the model binder to create a dice
        /// </summary>
       public Dice()
        {
            roll = new Random((int)DateTime.Now.Ticks);
            Task.Delay(20).Wait();
        }

        /// <summary>
        /// Use this method if creating multiple dice at once, create random number generator outside of this class and pass it in.
        /// 
        /// Will result and more evenly distributed dice rolls.
        /// </summary>
        /// <param name="rnd"></param>
        public Dice(Random rnd)
        {
            roll = rnd;
            dieValue = roll.Next(1, 7); 
        }
        /// <summary>
        /// Use an external RNG to pass in a rnd.Next() value to set as initial value
        /// </summary>
        /// <param name="valIn"></param>
        public Dice(int valIn)
        {
            roll = new Random();
            Task.Delay(20).Wait();
            dieValue = valIn;
        }

        public int dieValue { get; set; }
        public bool holding { get; set; }

        public int Roll()
        {
            dieValue = roll.Next(1, 7);
            return dieValue;
        }
    }
}
