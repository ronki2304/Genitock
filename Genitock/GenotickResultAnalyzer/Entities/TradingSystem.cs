using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenotickResultAnalyzer.Entities
{
    public class TradingSystem
    {
        public Position position { get; set; }
        public Double _StopLoss { get; set; }
        private Double Fees;
        private Boolean Reinvest;
        private Double amount;
        public Trade currentTrade;
        public List<Trade> trades;

        public TradingSystem(Double fees, Boolean reinvest)
        {
            Fees = fees;
            Reinvest = reinvest;
            position = Position.OutMarket;
            amount = 1;
            trades = new List<Trade>();
        }

        public void Createposition(Double openRate, DateTime key, Prediction prediction)
        {
            if (position == Position.OutMarket && prediction == Prediction.UP)
            {
                currentTrade = new Trade(amount);
                currentTrade.Openkey = key;
                currentTrade.openRate = openRate;
                _StopLoss = 0.9 * openRate;
                position = Position.InMarket;
            }

        }

        public Boolean StopLoss(Double Low, DateTime key)
        {
            if (position == Position.InMarket && Low <= _StopLoss)
            {
                currentTrade.closeRate = _StopLoss;
                currentTrade.Closekey = key;

                position = Position.OutMarket;
                _StopLoss = 100;
                ComputeProfit();
                currentTrade.comment = "Stop Loss";
                trades.Add(currentTrade);
                currentTrade = null;
                return true;
            }
            return false;
        }

        private void ComputeProfit()
        {
            Double convertedamount = currentTrade.Initialamount / currentTrade.openRate;
            //on retire les frais
            convertedamount = convertedamount - convertedamount * Fees/100;

            //retour à la monnaie initiale
            Double resultamount = convertedamount * currentTrade.closeRate;
            //passage des frais
            resultamount = resultamount - resultamount * Fees / 100;

            currentTrade.FinalAmount = resultamount;

            //si on réinvestis les gains sinon cela reste 1 par defaut
            if (Reinvest)
                amount = currentTrade.FinalAmount;

            currentTrade.profit = currentTrade.FinalAmount - currentTrade.Initialamount;
        }
        public void close(Double CloseRate, DateTime key, Prediction prediction)
        {
            if (position == Position.InMarket && prediction != Prediction.UP)
            {
                currentTrade.closeRate = CloseRate;
                currentTrade.Closekey = key;
                position = Position.OutMarket;
                ComputeProfit();
                currentTrade.comment = "On close";

                trades.Add(currentTrade);
                
                currentTrade = null;
            }
        }

        public List<String> ExportData()
        {
            List<String> export = new List<string>();
            export.Add("date achat; cours achat; date cloture;cours cloture;profit; Somme post trade;comment");
            export.AddRange(trades.Select(p => $"{p.Openkey.ToString("yyyyMMddHHmmss")};{p.openRate};{p.Closekey.ToString("yyyyMMddHHmmss")};{p.closeRate};{p.profit}; {p.FinalAmount};{p.comment}").ToList());
            return export;
        }
    }
}
