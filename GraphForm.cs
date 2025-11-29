#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LabWork
{
    internal sealed class GraphForm : Form
    {
        private readonly GraphDrawer _drawer;

        public GraphForm()
        {
            Text = "y = arccos(x) / (2x + 1)";
            MinimumSize = new Size(320, 240);
            BackColor = Color.White;
            DoubleBuffered = true;

            _drawer = new GraphDrawer(0.1, 0.9, 0.1);

            Resize += OnResizeInvalidate;
        }

        private void OnResizeInvalidate(object? sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _drawer.Draw(e.Graphics, ClientRectangle);
        }
    }
}