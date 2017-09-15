using Genitock.Genotick;
using Genitock.Entity.Poloniex;
using Genitock.Poloniex;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Genitock.Interface;
using Newtonsoft.Json;
using Genitock.Entity.Poloniex.Market;
using Genitock.Poloniex.Live;
using System.Threading;
using Genitock.Trading;

namespace Genitock
{ 
    class Program
    {
        const String WAMPURL = "wss://api.poloniex.com";
        static TradingEnvironment trading;
        static void Main(string[] args)
        {

            String mode = String.Empty;
            try
            {
                mode=  args.First(p => p.StartsWith("mode=")).Substring(5);
            }
            catch
            {
                Console.WriteLine("mode must be set");
                Console.WriteLine("possible value :");
                Console.WriteLine("CSV to generate data for genotick learning");
                Console.WriteLine("LIVE for trading");
                return;
            }

            //for the moment only poloniex but may add a new provider
            Console.WriteLine("Poloniex choosen");
			IBroker pw = new PoloniexWrapper();
			trading = new TradingEnvironment(pw);

            if (mode == "CSV")
            {
                //example : mode=CSV dtStart=01/01/1970 dtEnd=01/01/2090 Period=m5 Pair=BTC_ETH ExportPath=c:\temp
                DateTime dtstart, dtEnd;
                Period period;
                Pair pair;
                String ExportPath = String.Empty;
                try
                {
                    String sdt = args.FirstOrDefault(p => p.StartsWith("dtStart="));
                    if (String.IsNullOrEmpty(sdt))
                        dtstart = new DateTime(1970, 01, 01);
                    else
                        dtstart = Convert.ToDateTime(args.First(p => p.StartsWith("dtStart=")).Substring(8));

                    sdt = String.Empty;
                    sdt = args.FirstOrDefault(p => p.StartsWith("dtEnd="));
                    if (String.IsNullOrEmpty(sdt))
                        dtEnd = DateTime.MaxValue;
                    else
                        dtEnd = Convert.ToDateTime(args.FirstOrDefault(p => p.StartsWith("dtEnd=")).Substring(6));

                    period = (Period)Enum.Parse(typeof(Period), args.First(p => p.StartsWith("Period=")).Substring(7));
                    pair = (Pair)Enum.Parse(typeof(Pair), args.First(p => p.StartsWith("Pair=")).Substring(5));

                    ExportPath = args.First(p => p.StartsWith("ExportPath=")).Substring(11);
                    try
                    {
                        if (!Directory.Exists(ExportPath))
                        {
                            Directory.CreateDirectory(ExportPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error with the ExportedPath value : ");
                        Console.WriteLine(ex.ToString());
                        return;
                    }
                }

                catch
                {
                    Console.WriteLine("Invalid or missing parameters ");
                    Console.WriteLine("required parameter : ExportPath, Period, Pair");
                    Console.WriteLine();
                    Console.WriteLine("Period possible value : m5 for 5 minuts");
                    Console.WriteLine("m15 for 15 minuts");
                    Console.WriteLine("m30 for 30 minuts");
                    Console.WriteLine("h2 for 2 hours");
                    Console.WriteLine("h4 for 4 hours");
                    Console.WriteLine("d1 for 1 day");
                    Console.WriteLine();
                    Console.WriteLine("Pair must be written by {reference_Currency}_{convertedCurrency} for example BTC_ETH");


                    Console.WriteLine("Optional parameter dtStart,dtEnd");
                    Console.WriteLine("the date must be written as it is showed on systray");

                    Console.ReadLine();
                    return;
                }
                ExportData(pair, dtstart, dtEnd, period, ExportPath);
            }

            if (mode=="LIVE")
            {
				//TODO need to set the timer
				//TODO need to synchronize before
				runtime();

            }
        }

        /// <summary>
        /// Export data for genotick training
        /// </summary>
        /// <param name="pair">Pair.</param>
        /// <param name="dtstart">Dtstart.</param>
        /// <param name="dtstop">Dtstop.</param>
        /// <param name="period">Period.</param>
        /// <param name="ExportPath">Export path.</param>
        private static void ExportData(Pair pair, DateTime dtstart, DateTime dtstop, Period period, String ExportPath)
        {
            var chart = trading.GetChartData(pair, dtstart, dtstop, period);

            //write the file
            InputData.WriteToCSVFile(chart, ExportPath);
        }

        static void runtime()
        {
           //TODO check if it is in order
            //Ticker.onTick+= (source, e) => { Console.WriteLine($"{e.Rate}");};
            //Console.WriteLine("ca marche");
            //Console.ReadLine();
            //trading.Buy();
            Console.ReadLine();
            return;

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
            var operation =GenotickExec.SetPrediction();




            if (operation == Operation.buy && trading.SourceWallet.amount >Convert.ToDouble(ConfigurationManager.AppSettings["Minimum_trade"]))
                trading.Buy();
            if (operation== Operation.sell && trading.TargetWallet.amount > Convert.ToDouble(ConfigurationManager.AppSettings["Minimum_trade"]))
                trading.Sell();
            
        }
    }
}

