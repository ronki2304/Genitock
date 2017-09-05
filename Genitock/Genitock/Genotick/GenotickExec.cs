using Genitock.Entity.Poloniex;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genitock.Genotick
{
    class GenotickExec
    {
        public static Operation SetPrediction()
        {
            Operation prediction=Operation.Out;
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "java",
                    Arguments = String.Concat("-jar ", Path.Combine(GenotickConfig._GenotikPath, "genotick.jar "), "input=file:", GenotickConfig.ConfigPath, " "),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = GenotickConfig._GenotikPath
                }
            };

            proc.Start();

            proc.OutputDataReceived += ((sender, e) =>
            {
                string consoleLine = e.Data;
                //handle data
                Console.WriteLine(consoleLine);
            });

            proc.BeginOutputReadLine();
            proc.WaitForExit();

            /*while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                // do something with line
                Console.WriteLine(line);
                if (line.Contains(" for the next trade:") &&
                    line.Contains(GenotickConfig.CurrenciesDataFileName.ToString()))
                {
                    switch (line.Substring(line.IndexOf(':')).Trim())
                    {
                        case "UP":
                            prediction = Operation.buy;
                            break;
                        case "DOWN":
                            prediction = Operation.buy;
                            break;
                        case "OUT":
                        default:
                            prediction = Operation.Out;
                            break;
                    }
                }
            }*/
            return prediction;
        }

        public static void ReverseData()
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "java",
                    Arguments = String.Concat("-jar ", Path.Combine(GenotickConfig._GenotikPath, "genotick.jar "), "reverse=", GenotickConfig.DataDirectory),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = GenotickConfig._GenotikPath
                }
            };
            proc.Start();
            proc.WaitForExit();

        }
    }
}
