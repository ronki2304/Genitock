using System;
using GenotickResultAnalyzer.Entities;

namespace GenotickResultAnalyzer.Interface
{
    public interface Iclipping
    {
        Prediction Next(Prediction Genotickprediction);
    }
}
