using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenotickResultAnalyzer.Entities
{
    public enum Prediction
    {
        UP=1,
        OUT=0,
        DOWN=-1
    }

    public enum Position
    {
        InMarket,
        OutMarket
    }
}
