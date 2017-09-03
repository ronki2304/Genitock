using Genitock.Entity.Poloniex;
using Genitock.Genotick;
using Genitock.Poloniex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography; 
using System.Text;
using System.Threading.Tasks; 

namespace Genitock
{ 
    class Program
    {
        const String WAMPURL = "wss://api.poloniex.com";

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
            if (mode == "CSV")
            {
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
                    Console.WriteLine("the date must be written as it is showed on systray")




                    Console.ReadLine();
                    return;
                }
                ExportData(pair, dtstart, dtEnd, period, ExportPath);
            }

        }

        private static void ExportData(Pair pair, DateTime dtstart, DateTime dtstop, Period period, String ExportPath)
        {
            PoloniexWrapper wrapper = new PoloniexWrapper();

            var chart = wrapper.GetChartData(pair, dtstart, dtstop, period);

            //write the file
            InputData.SaveToCSV(pair, chart, ExportPath);
        }

    }
}
