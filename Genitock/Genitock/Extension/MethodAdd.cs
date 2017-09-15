using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Genitock.Entity.Poloniex;

namespace Genitock.Extension
{
    public static class Methodextension
    {
        public static Int64 getUnixMilliTime(this DateTime dt)
        {
            return (Int64)(dt - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

     
    }
}
