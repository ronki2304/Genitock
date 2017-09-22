using System;
using System.Collections.Generic;
using System.Linq;
using GenotickResultAnalyzer.Entities;
using GenotickResultAnalyzer.Interface;

namespace GenotickResultAnalyzer.Clipping
{
    public class ExponentialMovingAverage:Iclipping
    {
        List<Int32> LGenotickPrediction;
		Int32 nbOccurrence;
        Double Alpha;
        Int32 Coeff;


        public ExponentialMovingAverage(Int32 period, Double alpha, int coeff)
        {
			nbOccurrence = period;
			LGenotickPrediction = new List<Int32>();
            Alpha = alpha;
            Coeff = coeff;
        }

        public Prediction Next(Prediction Genotickprediction, Prediction trend)
        {
			var toadd = Genotickprediction == trend ? ((Int32)Genotickprediction) * Coeff : ((Int32)Genotickprediction);
			LGenotickPrediction.Add(toadd);

			if (LGenotickPrediction.Count > nbOccurrence)
				LGenotickPrediction.RemoveAt(0);
			else
				return Prediction.OUT;

            var avg=LGenotickPrediction.Select(p=> Convert.ToDouble((int)p)).Aggregate((ema, nextpred)=>Alpha*nextpred+(1-Alpha)*ema);

			if (avg > ((int)Prediction.UP) / 10)
				return Prediction.UP;

			if (avg < ((int)Prediction.DOWN) / 10)
				return Prediction.DOWN;

			return Prediction.OUT;
            
        }
    }
}
