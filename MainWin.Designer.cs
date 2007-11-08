partial class MainWin
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.Step = new System.Windows.Forms.Timer(this.components);
        this.SuspendLayout();
        //
        // Step
        //
        this.Step.Tick += new System.EventHandler(this.Step_Tick);
        //
        // MainWin
        //
        this.ClientSize = new System.Drawing.Size(238, 238);
        this.DoubleBuffered = true;
        this.Name = "MainWin";
        this.Text = "Game of Life";
        this.Load += new System.EventHandler(this.MainWin_Load);
        this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainWin_MouseUp);
        this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainWin_MouseClick);
        this.SizeChanged += new System.EventHandler(this.MainWin_SizeChanged);
        this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainWin_MouseDown);
        this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainWin_KeyPress);
        this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainWin_MouseMove);
        this.ResumeLayout(false);

    }

    private System.Windows.Forms.Timer Step;
}
