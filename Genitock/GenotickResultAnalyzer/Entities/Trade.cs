using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenotickResultAnalyzer.Entities
{
    public class Trade
    {
        public Double Initialamount { get; set; }
        public Double FinalAmount {get;set;}
        public DateTime Openkey { get; set; }
        public DateTime Closekey { get; set; }
        public Double openRate { get; set; }
        public Double closeRate { get; set; }
        public Double profit { get; set; }
        public String comment { get; set; }
        public Trade(double amount)
        {
            Initialamount = amount;
        }

    }
}
