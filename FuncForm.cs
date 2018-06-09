using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Windows.Forms.DataVisualization.Charting;

namespace Monte_Carlo
{
    public partial class FuncForm : Form
    {
        public FuncForm()
        {
            InitializeComponent();
        }

        private void chart1_Paint(object sender, PaintEventArgs e)          ///// ДОДЕЛАТЬ
        {
            Graphics fill = e.Graphics;
            GraphicsPath fillPath = new GraphicsPath(FillMode.Alternate);

            Axis ax = chart1.ChartAreas[0].AxisX;
            Axis ay = chart1.ChartAreas[0].AxisY;

            PointF[] points = chart1.Series["Bounds"].Points.Select(x => new PointF
            {
                X = (float)ax.ValueToPixelPosition(x.XValue),
                Y = (float)ay.ValueToPixelPosition(x.YValues[0])
            }).ToArray();

            fillPath.AddLines(points);

            points = chart1.Series["Function"].Points.Select(x => new PointF
            {
                X = (float)ax.ValueToPixelPosition(x.XValue),
                Y = (float)ay.ValueToPixelPosition(x.YValues[0])
            }).ToArray();

            fillPath.AddCurve(points); // Все происходит здесь.

            SolidBrush brush = new SolidBrush(Color.DarkBlue);
            fill.FillPath(brush, fillPath);

            fillPath.Dispose();
        }
    }
}
