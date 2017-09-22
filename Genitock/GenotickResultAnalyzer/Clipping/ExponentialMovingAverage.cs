using System;
using System.Collections.Generic;
using System.Linq;
using GenotickResultAnalyzer.Entities;
using GenotickResultAnalyzer.Interface;

namespace GenotickResultAnalyzer.Clipping
{
    public class ExponentialMovingAverage:Iclipping
    {
		List<Prediction> LGenotickPrediction;
		Int32 nbOccurrence;
        Double Alpha;


        public ExponentialMovingAverage(Int32 period, Double alpha)
        {
			nbOccurrence = period;
			LGenotickPrediction = new List<Prediction>();
            Alpha = alpha;
        }

        public Prediction Next(Prediction Genotickprediction)
        {
			LGenotickPrediction.Add(Genotickprediction);

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
