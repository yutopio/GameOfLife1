using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

public unsafe partial class MainWin : Form
{
    const int WM_SIZING         = 0x0214;
    const int WMSZ_LEFT         = 1;
    const int WMSZ_RIGHT        = 2;
    const int WMSZ_TOP          = 3;
    const int WMSZ_TOPLEFT      = 4;
    const int WMSZ_TOPRIGHT     = 5;
    const int WMSZ_BOTTOM       = 6;
    const int WMSZ_BOTTOMLEFT   = 7;
    const int WMSZ_BOTTOMRIGHT  = 8;

    Size frame;
    bool[,] cellStates;
    int cellWidth, cellHeight;
    bool mouseMove, mouseMoveInCell;
    Point mouseMovePrev;

    public MainWin()
    {
        InitializeComponent();

        // Initialize the state;
        cellWidth = cellHeight = 20;
        cellStates = (bool[,])Array.CreateInstance(typeof(bool), new[] { 22, 22 }, new[] { -1, -1 });
        Render();
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_SIZING)
        {
            var size = (Rect*)m.LParam;
            var flag = new[] { 0, 0, 1, 0, 0, 1, 2, 2, 3 }[(int)m.WParam];
            if ((flag & 1) == 0)
            {
                var temp = size->right - frame.Width;
                size->left = temp - Math.Max((temp - size->left + 6) / 12, 10) * 12 + 2;
            }
            else
            {
                var temp = size->left + frame.Width;
                size->right = temp + Math.Max((size->right - temp + 6) / 12, 10) * 12 - 2;
            }
            if ((flag & 2) == 0)
            {
                var temp = size->bottom - frame.Height;
                size->top = temp - Math.Max((temp - size->top + 6) / 12, 10) * 12 + 2;
            }
            else
            {
                var temp = size->top + frame.Height;
                size->bottom = temp + Math.Max((size->bottom - temp + 6) / 12, 10) * 12 - 2;
            }
        }

        base.WndProc(ref m);
    }

    private void MainWin_Load(object sender, EventArgs e)
    {
        // Get window frame size.
        frame = this.Size - this.ClientSize;
    }

    private void MainWin_SizeChanged(object sender, EventArgs e)
    {
        // Calculate new world size from the window size;
        var width = this.ClientSize.Width / 12 + 3;
        var height = this.ClientSize.Height / 12 + 3;

        // Create new states array and copy from the old.
        var iu = Math.Min(cellStates.GetLength(0), width) - 1;
        var ju = Math.Min(cellStates.GetLength(1), height) - 1;
        var newStates = (bool[,])Array.CreateInstance(typeof(bool), new[] { width, height }, new[] { -1, -1 });
        for (int i = 0; i < iu; i++)
            for (int j = 0; j < ju; j++)
                newStates[i, j] = cellStates[i, j];

        // Save data to local variables.
        cellStates = newStates;
        cellWidth = width - 2;
        cellHeight = height - 2;
        Render();
    }

    private void Step_Tick(object sender, EventArgs e)
    {
        // Create the new state array.
        var newStates = (bool[,])Array.CreateInstance(typeof(bool), new[] { cellWidth + 2, cellHeight + 2 }, new[] { -1, -1 });
        for (int y = 0; y < cellHeight; y++)
            for (int x = 0; x < cellWidth; x++)
            {
                // Count the neighboring live cells.
                var liveCount = 0;
                for (int j = y - 1; j <= y + 1; j++)
                    for (int i = x - 1; i <= x + 1; i++)
                        if (cellStates[i, j]) liveCount++;

                // Evaluate the cell live state.
                if (cellStates[x, y])
                    newStates[x, y] = liveCount >= 3 && liveCount <= 4;
                else
                    newStates[x, y] = liveCount == 3;
            }

        // Replace the state and render it.
        cellStates = newStates;
        Render();
    }

    void Render()
    {
        Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.FillRectangle(Brushes.LightGray, new Rectangle(Point.Empty, bmp.Size));
            for (int y = 0; y < cellHeight; y++)
                for (int x = 0; x < cellWidth; x++)
                    g.FillRectangle(cellStates[x, y] ? Brushes.Black : Brushes.White,
                        new Rectangle(x * 12, y * 12, 10, 10));
        }
        this.BackgroundImage = bmp;
    }

    private void MainWin_KeyPress(object sender, KeyPressEventArgs e)
    {
        switch (e.KeyChar)
        {
            case 'r':
                Random r = new Random();
                for (int y = 0; y < cellHeight; y++)
                    for (int x = 0; x < cellWidth; x++)
                        cellStates[x, y] = r.NextDouble() < .2;
                Render();
                break;

            case 'c':
                cellStates = (bool[,])Array.CreateInstance(typeof(bool), new[] { cellWidth + 2, cellHeight + 2 }, new[] { -1, -1 });
                Render();
                break;
        }
    }

    private void MainWin_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
            // When clicked by right, start or stop the time.
            this.Step.Enabled = !this.Step.Enabled;
    }

    private void MainWin_MouseDown(object sender, MouseEventArgs e)
    {
        if ((int)(e.Button & MouseButtons.Left) != 0)
        {
            this.Capture = mouseMove = true;
            int x = e.X / 12, y = e.Y / 12;
            if ((e.X - x * 12 < 10) && (e.Y - y * 12 < 10))
                cellStates[x, y] = !cellStates[x, y];
            Render();

            mouseMoveInCell = true;
            mouseMovePrev = new Point(e.X, e.Y);
        }
    }

    void MainWin_MouseMove(object sender, MouseEventArgs e)
    {
        if (mouseMove)
        {
            var dist = Math.Sqrt(Math.Pow(e.X - mouseMovePrev.X, 2) + Math.Pow(e.Y - mouseMovePrev.Y, 2)) / 2;
            var dx = (e.X - mouseMovePrev.X) / dist;
            var dy = (e.Y - mouseMovePrev.Y) / dist;
            double x = mouseMovePrev.X, y = mouseMovePrev.Y;
            for (int i = 0; i < dist; i++)
                MainWin_MouseMoveInternal((int)(x += dx), (int)(y += dy));
            mouseMovePrev = new Point(e.X, e.Y);
        }
    }

    void MainWin_MouseMoveInternal(int x, int y)
    {
        int xx = x / 12, yy = y / 12;
        if ((x - xx * 12 < 10) && (y - yy * 12 < 10) && (xx >= 0 && xx < cellWidth && yy >= 0 && yy < cellHeight))
        {
            if (!mouseMoveInCell)
            {
                cellStates[xx, yy] = !cellStates[xx, yy];
                Render();
                mouseMoveInCell = true;
            }
        }
        else mouseMoveInCell = false;
    }

    void MainWin_MouseUp(object sender, MouseEventArgs e)
    {
        if ((int)(e.Button & MouseButtons.Left) != 0)
        {
            this.Capture = mouseMove = false;
            Render();
        }
    }
}
