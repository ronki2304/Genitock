using Genitock.Entity;
using Genitock.Entity.Poloniex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genitock.Interface
{
    public interface IBroker
    {

        Double ReturnBalance(Currencies currency);

        Chart GetChartData(Pair pair, DateTime dtStart, DateTime dtEnd, Period period);
    }
}
