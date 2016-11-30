using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShakeotDay.Core.Models
{
    public class Wallet
    {
        public Wallet() { }

        public long Id { get; set; }
        public long UserId { get; set; }
        public long WalletValue { get; set; }
        public int TimesBoughtIn { get; set; }
    }
}
