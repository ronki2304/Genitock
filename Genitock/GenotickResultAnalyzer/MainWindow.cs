using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GenotickResultAnalyzer.Clipping;
using GenotickResultAnalyzer.Entities;
using GenotickResultAnalyzer.Interface;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

	protected void OnBtnBrowseProfitClicked(object sender, EventArgs e)
	{
		FileChooserDialog dlg = new FileChooserDialog("Genotick profit"
													 , this
													  , FileChooserAction.Open
													  , "Cancel", ResponseType.Cancel
													  , "Open", ResponseType.Accept);

		if (dlg.Run() == (int)ResponseType.Accept)
            txtpathPrediction.Text = dlg.Filename;


		dlg.Destroy();
    }

    protected void OnBtnTickClicked(object sender, EventArgs e)
    {
		FileChooserDialog dlg = new FileChooserDialog("Tick data"
													, this
													 , FileChooserAction.Open
													 , "Cancel", ResponseType.Cancel
													 , "Open", ResponseType.Accept);

        if (dlg.Run() == (int)ResponseType.Accept)
            txtpathTick.Text = dlg.Filename;
          

        dlg.Destroy();
    }

    protected void OnBtnAnalyzeClicked(object sender, EventArgs e)
    {

        #region check
        //verification que le SL est bien numerique
        Double SL, fees;
        Int32 nbperiod;
		Boolean OK = Double.TryParse(txtSL.Text, out SL);
		if (!OK)
		{
            Dialog dialog = new Dialog("Error"
                                       , this
                                       , DialogFlags.Modal
                                       , "OK", ResponseType.Ok);

            var lbl = new Label("Stop loss must be a numeric");
            dialog.VBox.PackStart(lbl);
            lbl.Show();
            dialog.Run();
            dialog.Destroy();
			return;
		}
        Double.TryParse(txtFees.Text, out fees);
		if (!OK)
        {
			Dialog dialog = new Dialog("Error"
                                    , this
                                    , DialogFlags.Modal
                                    , "OK", ResponseType.Ok);

			var lbl = new Label("fees must be a numeric");
			dialog.VBox.PackStart(lbl);
			lbl.Show();
			dialog.Run();
			dialog.Destroy();
			return;
		}

        Int32.TryParse(txtNbPeriod.Text, out nbperiod);
        if (!OK && txtNbPeriod.Sensitive)
		{
			Dialog dialog = new Dialog("Error"
									, this
									, DialogFlags.Modal
									, "OK", ResponseType.Ok);

			var lbl = new Label("nbPeriod must be a numeric");
			dialog.VBox.PackStart(lbl);
			lbl.Show();
			dialog.Run();
			dialog.Destroy();
			return;
		}


        if (!File.Exists(txtpathTick.Text))
        {
            Dialog dialog = new Dialog("Error"
                                , this
                                , DialogFlags.Modal
                                , "OK", ResponseType.Ok);

            var lbl = new Label("Tick file invalid");
            dialog.VBox.PackStart(lbl);
            lbl.Show();
            dialog.Run();
            dialog.Destroy();
            return;
        }

        if (!File.Exists(txtpathPrediction.Text))
		{
			Dialog dialog = new Dialog("Error"
								, this
								, DialogFlags.Modal
								, "OK", ResponseType.Ok);

			var lbl = new Label("Profil file invalid");
			dialog.VBox.PackStart(lbl);
			lbl.Show();
			dialog.Run();
			dialog.Destroy();
			return;
		}
#endregion

        //recuperation du nom de fichier uniquemet avec l'extension pour filtrer le fichier de profit
        FileInfo fi = new FileInfo(txtpathTick.Text);


		List<GenotickData> data = new List<GenotickData>();
		//lecture de l'historique des données
		List<String> AllTicks = File.ReadAllLines(txtpathTick.Text).ToList();
		//recuperation des lignes uniquement pour la paire analysée et exclusion du reverse
		List<String> AllPrediction = File.ReadAllLines(txtpathPrediction.Text).ToList().Where(p => p.Contains(fi.Name) && !p.Contains("reverse")).ToList();
        Boolean chk = chkReinvest.Active;

        List<String> AllTrend =null;


		 if (chkTrend.Active)
            AllTrend= File.ReadAllLines(txtTrend.Text).ToList().Where(p => p.Contains(fi.Name) && !p.Contains("reverse")).ToList();
        

        Iclipping clipping;
        //chargement de la méthode de reduction de bruit
        if (rbNo.Active)
            clipping = new NoClipping();
        else if (rdEMA.Active)
            clipping = new ExponentialMovingAverage(nbperiod,0.75);
        else if (rdSMA.Active)
            clipping = new SimpleMovingAverage(nbperiod);
        else 
            clipping = new LinearRegression(nbperiod);
            
        

        TradingSystem sys = new TradingSystem(Convert.ToDouble(txtFees.Text), chk,clipping);


        Int32 trendcmpt = 1;
		for (int i = 1; i < AllTicks.Count; i++)
		{
          
            
			GenotickData tick = new GenotickData();
			String[] charting = AllTicks[i].Split(',');

			//tous les 4 enregistrement on increment par rapport au fichier de trend
			if (AllTrend[trendcmpt + 1] == charting[0])
				trendcmpt++;


			tick.keyChart = DateTime.ParseExact(charting[0], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
			tick.Open = Convert.ToDouble(charting[1], CultureInfo.InvariantCulture);
			tick.High = Convert.ToDouble(charting[2], CultureInfo.InvariantCulture);
			tick.Low = Convert.ToDouble(charting[3], CultureInfo.InvariantCulture);
			tick.Close = Convert.ToDouble(charting[4], CultureInfo.InvariantCulture);

          


            String[] prediction = AllPrediction[i - 1].Split(',');
            String[] OneTrend;
            if (AllTrend!=null)
                OneTrend = AllTrend[trendcmpt - 1].Split(',');
            else
                OneTrend =  new []{"", "", "OUT"};
            
            //on verifie sur on est sur la meme date
            //si ce 'est pas le cas on increment l'offset
            //en effet le fichier de prediction va partir de la plus vieille prediction et pas forcément de la paire que l'on veut analyser
           
			tick.keyPrediction = DateTime.ParseExact(prediction[0], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
			tick.prediction = (Prediction)Enum.Parse(typeof(Prediction), prediction[2]);
            tick.Trend = (Prediction)Enum.Parse(typeof(Prediction), OneTrend[2]);
			//systeme de trading
            sys.Createposition(tick.Open, tick.keyChart, tick.prediction,tick.Trend);
			sys.StopLoss(tick.Low, tick.keyChart);
			sys.close(tick.Open, tick.keyChart, tick.prediction,tick.Trend);

			
		}
		String filename = String.IsNullOrWhiteSpace(txtOutput.Text) ? "result.csv" : txtOutput.Text + ".csv";


        File.WriteAllLines(filename, sys.ExportData());
    }

  

   

    protected void OnRdPressed(object sender, EventArgs e)
    {
        if (((Gtk.RadioButton)sender).Name == "rbNo")
            txtNbPeriod.Sensitive = false;
        else
            txtNbPeriod.Sensitive = true;

        if (((Gtk.RadioButton)sender).Name =="rdEMA")
        {
            txtAlpha.Visible = true;
            lblAlpha.Visible = true;
        }
        else
        {
            txtAlpha.Visible = false;
            lblAlpha.Visible = false;
        }
    }

    protected void OnChkMarginToggled(object sender, EventArgs e)
    {
        lblMarginRate.Visible = ((Gtk.CheckButton)sender).Active;
        txtMarginRate.Visible = ((Gtk.CheckButton)sender).Active;

    
    }

    protected void OnBtnTickTrendClicked(object sender, EventArgs e)
    {
		FileChooserDialog dlg = new FileChooserDialog("Prediction trend data"
													, this
													 , FileChooserAction.Open
													 , "Cancel", ResponseType.Cancel
													 , "Open", ResponseType.Accept);

		if (dlg.Run() == (int)ResponseType.Accept)
			txtTrend.Text = dlg.Filename;


		dlg.Destroy();
    }

    protected void OnChkTrendToggled(object sender, EventArgs e)
    {
        txtTrend.Sensitive = ((Gtk.CheckButton)sender).Active;
        lblTrend.Sensitive = ((Gtk.CheckButton)sender).Active;
        btnTickTrend.Sensitive = ((Gtk.CheckButton)sender).Active;
        lblTrendScale.Sensitive= ((Gtk.CheckButton)sender).Active;
        txtScale.Sensitive = ((Gtk.CheckButton)sender).Active;
    }
}
