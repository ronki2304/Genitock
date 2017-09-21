using System;
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

    protected void OnBtnTickButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
    {
       
    }

    protected void OnBtnTickClicked(object sender, EventArgs e)
    {
		FileChooserDialog dlg = new FileChooserDialog("meuh"
													, this
													 , FileChooserAction.Open
													 , "Cancel", ResponseType.Cancel
													 , "Open", ResponseType.Accept);

        if (dlg.Run() == (int)ResponseType.Accept)
            txtTickPath.Text = dlg.Filename;
          

        dlg.Destroy();
    }
}
