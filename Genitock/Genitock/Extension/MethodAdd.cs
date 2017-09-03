using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genitock.Extension
{
    public static  class MethodAdd
    { 
        public static Int64  getUnixTime(this DateTime dt)
        {
            return (Int64)(dt - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
