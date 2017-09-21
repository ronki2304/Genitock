using GenotickResultAnalyzer.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;



namespace GenotickResultAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnBrowseTick_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtpathTick.Text = openFileDialog.FileName;
        }



        private void btnBrowseProfit_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtpathProfit.Text = openFileDialog.FileName;
        }

     

        private void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            //verification que le SL est bien numerique
            Double SL, fees;
            Boolean OK=Double.TryParse(txtSL.Text, out SL);
            if (!OK)
            {
                System.Windows.MessageBox.Show("Stop loss must be a numeric");
                return;
            }
            Double.TryParse(txtSL.Text, out fees);
            if (!OK)
            {
                System.Windows.MessageBox.Show("fees must be a numeric");
                return;
            }


            if (!File.Exists(txtpathTick.Text))
            {
                System.Windows.MessageBox.Show("Tick file invalid");
                return;
            }

            if (!File.Exists(txtpathProfit.Text))
            {
                System.Windows.MessageBox.Show("Profil file invalid");
                return;
            }

            //recuperation du nom de fichier uniquemet avec l'extension pour filtrer le fichier de profit
            FileInfo fi = new FileInfo(txtpathTick.Text);


            List<GenotickData> data = new List<GenotickData>();
            //lecture de l'historique des données
            List<String> AllTicks = File.ReadAllLines(txtpathTick.Text).ToList();
            //recuperation des lignes uniquement pour la paire analysée et exclusion du reverse
            List<String> AllPrediction = File.ReadAllLines(txtpathProfit.Text).ToList().Where(p => p.Contains(fi.Name) && !p.Contains("reverse")).ToList();
            Boolean chk = chkReinvest.IsChecked.HasValue ? chkReinvest.IsChecked.Value : false;
            TradingSystem sys = new TradingSystem(Convert.ToDouble(txtFees.Text),chk);

            for (int i=1;i<AllTicks.Count;i++)
            {
                GenotickData tick = new GenotickData();
                String[] charting = AllTicks[i].Split(',');
                String[] prediction = AllPrediction[i-1].Split(',');

                tick.keyChart = DateTime.ParseExact(charting[0], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                tick.Open = Convert.ToDouble(charting[1], CultureInfo.InvariantCulture);
                tick.High = Convert.ToDouble(charting[2], CultureInfo.InvariantCulture);
                tick.Low = Convert.ToDouble(charting[3], CultureInfo.InvariantCulture);
                tick.Close = Convert.ToDouble(charting[4], CultureInfo.InvariantCulture);

                tick.keyPrediction = DateTime.ParseExact(prediction[0], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                tick.prediction = (Prediction)Enum.Parse(typeof(Prediction), prediction[2]);

                //systeme de trading
                sys.Createposition(tick.Open, tick.keyChart, tick.prediction);
                sys.StopLoss(tick.Low, tick.keyChart);
                sys.close(tick.Open, tick.keyChart, tick.prediction);
            }
            String filename = String.IsNullOrWhiteSpace(txtOutput.Text) ? "result.csv" : txtOutput.Text + ".csv";

            
            File.WriteAllLines("result.csv", sys.ExportData());
        }
    }
}
