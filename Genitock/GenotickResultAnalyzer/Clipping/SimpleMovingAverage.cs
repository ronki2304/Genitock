using System;
using System.Collections.Generic;
using System.Linq;
using GenotickResultAnalyzer.Entities;
using GenotickResultAnalyzer.Interface;

namespace GenotickResultAnalyzer.Clipping
{
    public class SimpleMovingAverage : Iclipping
    {
        List<Prediction> LGenotickPrediction;
        Int32 nbOccurrence;
        public SimpleMovingAverage(int period)
        {
            nbOccurrence = period;
            LGenotickPrediction = new List<Prediction>();
        }

        public Prediction Next(Prediction Genotickprediction)
        {
            LGenotickPrediction.Add(Genotickprediction);

            if (LGenotickPrediction.Count > nbOccurrence)
                LGenotickPrediction.RemoveAt(0);
            else
                return Prediction.OUT;

            var avg = LGenotickPrediction.Average(p => (int)p);

            if (avg > ((int)Prediction.UP)/10)
                return Prediction.UP;

            if (avg < ((int)Prediction.DOWN) / 10)
                return Prediction.DOWN;

            return Prediction.OUT;
        }
    }
}
