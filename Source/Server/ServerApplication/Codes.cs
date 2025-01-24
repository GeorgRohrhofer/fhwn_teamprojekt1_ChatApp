using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    public enum Codes
    {
        SUCCESS = 0,
        FAILED = 1,
        USERNAMEALREADYTAKEN = 2,
        EMAILALREADYTAKEN = 3,
        EMPTYSYMBOL = 4,
        INVALIDEMAIL = 5,
        PASSWORDTOOSHORT = 6,
        NOTINGROUP = 7,
        BANNED = 8,
    }
}
