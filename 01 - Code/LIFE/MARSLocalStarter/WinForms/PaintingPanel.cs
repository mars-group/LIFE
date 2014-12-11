using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MARSLocalStarter.WinForms
{
    public class PaintingPanel
        : Panel
    {
        public PaintingPanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer,
            true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            e.Graphics.InterpolationMode = InterpolationMode.Low;

            base.OnPaint(e);
        }
    }
}
