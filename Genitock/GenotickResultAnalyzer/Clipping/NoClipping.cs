using System;
using GenotickResultAnalyzer.Entities;
using GenotickResultAnalyzer.Interface;

namespace GenotickResultAnalyzer.Clipping
{
    public class NoClipping : Iclipping
    {
        public NoClipping()
        {
        }

        public Prediction Next(Prediction Genotickprediction,Prediction trend)
        {
            return Genotickprediction;
        }
    }
}
