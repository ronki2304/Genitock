using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenotickResultAnalyzer.Entities
{
    /// <summary>
    /// cette classe represente la prédiction de genotick ainsi que le cours sur lequel il a été fait
    /// </summary>
    public class GenotickData
    {
        public DateTime keyChart { get; set; }
        public DateTime keyPrediction { get; set; }
        public Double Open { get; set; }
        public Double Close { get; set; }
        public Double High { get; set; }
        public Double Low { get; set; }
        public Prediction prediction { get; set; }
    }
}
