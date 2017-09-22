using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenotickResultAnalyzer.Interface;

namespace GenotickResultAnalyzer.Entities
{
    public class TradingSystem
    {
        Position position;
        Double _StopLoss;
        Double Fees;
        Boolean Reinvest;
        Double amount;
        Trade currentTrade;
        List<Trade> trades;
        Iclipping Predictor;
        DrawDown DrawDownSystem;


        public TradingSystem(Double fees, Boolean reinvest, Iclipping predictor,Double maxdrawdown, Int32 maxdrawdownInterval)
        {
            Fees = fees;
            Reinvest = reinvest;
            position = Position.OutMarket;
            amount = 1;
            trades = new List<Trade>();
            Predictor = predictor;
            DrawDownSystem = new DrawDown(maxdrawdown, maxdrawdownInterval);
          
        }

        public void Createposition(Double openRate, DateTime key, Prediction prediction, Prediction trend)
        {
            if (position == Position.OutMarket && Predictor.Next(prediction,trend) == Prediction.UP)
            {
                if (DrawDownSystem.AutorizeTrade())
                {
                    currentTrade = new Trade(amount);
                    currentTrade.Openkey = key;
                    currentTrade.openRate = openRate;
                    _StopLoss = 0.9 * openRate;
                    position = Position.InMarket;
                }
                else
                {
                    position = Position.VirtualInMarket;
                }
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
            DrawDownSystem.compute(currentTrade.profit);
        }
        public void close(Double CloseRate, DateTime key, Prediction prediction, Prediction trend)
        {
            Prediction bet = Predictor.Next(prediction, trend);
            if (position == Position.InMarket &&  bet!= Prediction.UP)
            {
                currentTrade.closeRate = CloseRate;
                currentTrade.Closekey = key;
                position = Position.OutMarket;
                ComputeProfit();
                currentTrade.comment = "On close";

                trades.Add(currentTrade);
                
                currentTrade = null;
            }
            if (position == Position.VirtualInMarket && bet != Prediction.UP)
            {
                position = Position.OutMarket;
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
