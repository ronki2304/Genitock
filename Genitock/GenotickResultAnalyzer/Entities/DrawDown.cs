using System;
namespace GenotickResultAnalyzer.Entities
{
    internal class DrawDown
    {
		Double HighestProfit;
        Double Profit;
		Double MaxDrawDown;
		Int32 MaxDrawDownInterval;

        Boolean DrawDownEnable;
        Int32 DrawDownEnableTime; //nombre de trade interdit depuis que DrawDownEnable est à true
        public DrawDown(Double maxdrawdown, Int32 interval)
        {
            HighestProfit = 0;
            Profit = 0;
            MaxDrawDownInterval = interval;
            MaxDrawDown = maxdrawdown;
            DrawDownEnable = false;
            DrawDownEnableTime = 0;
        }

        public void compute (Double profit)
        {
            Profit += profit;

            if (Profit >= HighestProfit)
            {
                HighestProfit = Profit;
            }
            if (HighestProfit * MaxDrawDown > Profit)
                DrawDownEnable = true;
        }

        public Boolean AutorizeTrade()
        {
			if (DrawDownEnable)
			{

				if (DrawDownEnableTime >= MaxDrawDownInterval)
				{
					DrawDownEnableTime = 0;
					HighestProfit = Profit;
                    DrawDownEnable = false;
					return true;
				}
				else
				{
					DrawDownEnableTime++;
					return false;
				}
			}
            return true;

		}
    }
}
