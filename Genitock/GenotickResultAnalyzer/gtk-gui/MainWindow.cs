
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.Alignment alignment1;

	private global::Gtk.Table table1;

	private global::Gtk.Button btnTick;

	private global::Gtk.Label Label1;

	private global::Gtk.Entry txtTickPath;

	protected virtual void Build()
	{
		global::Stetic.Gui.Initialize(this);
		// Widget MainWindow
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString("MainWindow");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.alignment1 = new global::Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
		this.alignment1.Name = "alignment1";
		// Container child alignment1.Gtk.Container+ContainerChild
		this.table1 = new global::Gtk.Table(((uint)(3)), ((uint)(3)), false);
		this.table1.Name = "table1";
		this.table1.RowSpacing = ((uint)(6));
		this.table1.ColumnSpacing = ((uint)(6));
		// Container child table1.Gtk.Table+TableChild
		this.btnTick = new global::Gtk.Button();
		this.btnTick.CanFocus = true;
		this.btnTick.Name = "btnTick";
		this.btnTick.UseUnderline = true;
		this.btnTick.Label = global::Mono.Unix.Catalog.GetString("GtkButton");
		this.table1.Add(this.btnTick);
		global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1[this.btnTick]));
		w1.LeftAttach = ((uint)(2));
		w1.RightAttach = ((uint)(3));
		w1.XOptions = ((global::Gtk.AttachOptions)(4));
		w1.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child table1.Gtk.Table+TableChild
		this.Label1 = new global::Gtk.Label();
		this.Label1.Name = "Label1";
		this.Label1.LabelProp = global::Mono.Unix.Catalog.GetString("Tick Files");
		this.table1.Add(this.Label1);
		global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1[this.Label1]));
		w2.XOptions = ((global::Gtk.AttachOptions)(4));
		w2.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child table1.Gtk.Table+TableChild
		this.txtTickPath = new global::Gtk.Entry();
		this.txtTickPath.CanFocus = true;
		this.txtTickPath.Name = "txtTickPath";
		this.txtTickPath.IsEditable = true;
		this.txtTickPath.InvisibleChar = '●';
		this.table1.Add(this.txtTickPath);
		global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1[this.txtTickPath]));
		w3.LeftAttach = ((uint)(1));
		w3.RightAttach = ((uint)(2));
		w3.YOptions = ((global::Gtk.AttachOptions)(4));
		this.alignment1.Add(this.table1);
		this.Add(this.alignment1);
		if ((this.Child != null))
		{
			this.Child.ShowAll();
		}
		this.DefaultWidth = 400;
		this.DefaultHeight = 300;
		this.Show();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
		this.btnTick.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler(this.OnBtnTickButtonReleaseEvent);
		this.btnTick.Clicked += new global::System.EventHandler(this.OnBtnTickClicked);
	}
}
