using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using Genitock.Entity.Poloniex;
using Genitock.Genotick;
using Genitock.Interface;
using Genitock.Poloniex;
using Genitock.Poloniex.Live;
using Genitock.Trading;

namespace Genitock.Service
{
    public class RuntimeService:ServiceBase
    {
        TradingEnvironment trading;
        protected override void OnStart(string[] args)
        {
			//for the moment only poloniex but may add a new provider
			Console.WriteLine("Poloniex choosen");
			IBroker pw = new PoloniexWrapper();
			ITicker it = new PoloniexTicker();
			trading = new TradingEnvironment(pw, it);
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            base.OnStop();
        }


        void GenitockProcess()
        {
			//load config file
			String sconfigfilePath = Path.Combine(ConfigurationManager.AppSettings["genotick_Path"]
				, ConfigurationManager.AppSettings["genotick_configfileName"]);

			if (!File.Exists(sconfigfilePath))
			{
				Console.WriteLine("Config File not found please update genitock config file");
				return;
			}


			//if we are already up to date do nothing
			if (GenotickConfig.NextEndingPoint > DateTime.UtcNow)
				return;

			//backup data
			InputData.BackupData();



			foreach (Pair pair in GenotickConfig.CurrenciesDataFileName)
			{
				//getting the last data
				//for moment assume that all csv have the same date
				//retrieve missing candle
				var chart = trading.GetChartData(pair, GenotickConfig.StartingPoint, DateTime.MaxValue, GenotickConfig.PoloniexPeriod);

				InputData.AppendChartDataFile(chart);
			}
			//reverse data
			GenotickExec.ReverseData();

			//update genotick config file
			GenotickConfig.SaveConfig();
			//call genotick to analyze
			var operation = GenotickExec.SetPrediction();




			if (operation == Operation.buy && trading.SourceWallet.amount > Convert.ToDouble(ConfigurationManager.AppSettings["Minimum_trade"]))
				trading.Buy();
			if (operation == Operation.sell && trading.TargetWallet.amount > Convert.ToDouble(ConfigurationManager.AppSettings["Minimum_trade"]))
				trading.Sell();
        }
    
    }
}
