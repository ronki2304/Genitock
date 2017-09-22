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

		List<Prediction> LGenotickPrediction;
		Int32 nbOccurrence;

        public LinearRegression(Int32 period)
        {
			nbOccurrence = period;
			LGenotickPrediction = new List<Prediction>();
        }

        public Prediction Next(Prediction Genotickprediction, Prediction trend)
        {
			LGenotickPrediction.Add(Genotickprediction);

			if (LGenotickPrediction.Count > nbOccurrence)
				LGenotickPrediction.RemoveAt(0);
			else
				return Prediction.OUT;

            var points = new List<Point>();

            for (int i = 0;i< LGenotickPrediction.Count();i++)
            {
                points.Add(new Point(){ X=i, Y=(Int32)LGenotickPrediction[i]});
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
