using System;
using System.Collections.Generic;
using GenotickResultAnalyzer.Entities;
using GenotickResultAnalyzer.Interface;
using System.Linq;
using GenotickResultAnalyzer.Extension;

namespace GenotickResultAnalyzer.Clipping
{
    public class LinearRegression:Iclipping
    {
		struct Point
		{
			public double X { get; set; }
			public double Y { get; set; }
		}

        List<Int32> LGenotickPrediction;
		Int32 nbOccurrence;
        Int32 Coeff;

        public LinearRegression(Int32 period, int coeff)
        {
			nbOccurrence = period;
            LGenotickPrediction = new List<Int32>();
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

            var points = new List<Point>();

            for (int i = 0;i< LGenotickPrediction.Count();i++)
            {
                points.Add(new Point(){ X=i, Y=LGenotickPrediction[i]});
            }

			var a = points.Variance(p => p.X, p => p.Y) / points.Variance(p => p.X, p => p.X);
			var avg = points.Average(p => p.Y) - a * points.Average(p => p.X);

			if (avg > ((int)Prediction.UP) / 10)
				return Prediction.UP;

			if (avg < ((int)Prediction.DOWN) / 10)
				return Prediction.DOWN;

			return Prediction.OUT;
        }
    }
}
