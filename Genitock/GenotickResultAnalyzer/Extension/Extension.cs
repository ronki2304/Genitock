using System;
using System.Collections.Generic;
using System.Linq;

namespace GenotickResultAnalyzer.Extension
{
	public static class Extension
	{
		public static double Variance<T>(this IEnumerable<T> list, Func<T, double> selectA, Func<T, double> selectB)
		{
			return list.Average(p => selectA(p) * selectB(p)) - (list.Average(p => selectA(p)) * list.Average(p => selectB(p)));
		}
	}
}
