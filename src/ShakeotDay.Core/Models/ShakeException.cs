using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShakeotDay.Core.Models
{
    public class ShakeException
    {
        private ShakeError _err;

        public ShakeException() { }

        public ShakeException(ShakeError errIn, string DescIn)
        {
            _err = errIn;
            ErrorMessage = DescIn;
        }

       public string ErrorType { get { return  _err.ToString(); } }
       public int ErrorNumber { get { return (int)_err; } }
        
        public string ErrorMessage { get; set; }
    }

    public enum ShakeError
    {
        Other = -1,
        AlreadyPlayedToday,
        NoMoreRollsAllowed        
    }
}
