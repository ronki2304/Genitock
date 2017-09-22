using System;
using System.Collections.Generic;
using System.Linq;
using GenotickResultAnalyzer.Entities;
using GenotickResultAnalyzer.Interface;

namespace GenotickResultAnalyzer.Clipping
{
    public class SimpleMovingAverage : Iclipping
    {
        List<Int32> LGenotickPrediction;
        Int32 nbOccurrence;
        Int32 Coeff;
        public SimpleMovingAverage(int period, int coeff)
        {
            nbOccurrence = period;
            LGenotickPrediction = new List<int>();
            Coeff = coeff;
        }

        public Prediction Next(Prediction Genotickprediction, Prediction trend)
        {
            //on ajoute un coef si la prediction est dans le sens de la tendance
            var toadd = Genotickprediction == trend ? ((int)Genotickprediction) * Coeff: ((int)Genotickprediction);
            LGenotickPrediction.Add(toadd);

            if (LGenotickPrediction.Count > nbOccurrence)
                LGenotickPrediction.RemoveAt(0);
            else
                return Prediction.OUT;

            var avg = LGenotickPrediction.Average(p => p);
          

            if (avg > ((int)Prediction.UP)/10)
                return Prediction.UP;

            if (avg < ((int)Prediction.DOWN) / 10)
                return Prediction.DOWN;

            return Prediction.OUT;
        }
    }
}
