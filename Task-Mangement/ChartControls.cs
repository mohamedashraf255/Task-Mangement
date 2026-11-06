using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Task_Mangement
{
    // Donut + legend ("Tasks by status")
    public class DonutChart : Control
    {
        public class Slice
        {
            public string Label; public Color Color; public int Value;
            public Slice(string label, Color color, int value) { Label = label; Color = color; Value = value; }
        }

        public List<Slice> Slices = new List<Slice>();
        readonly Font fBig = Theme.F(20f, FontStyle.Bold);
        readonly Font fSmall = Theme.F(8.5f);
        readonly Font fLegend = Theme.F(9.5f);

        public DonutChart() { DoubleBuffered = true; ResizeRedraw = true; BackColor = Color.White; }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int total = Slices.Sum(s => s.Value);
            int d = Math.Min(Height - 8, 176), thick = 36, left = 8, top = (Height - d) / 2;
            var ring = new Rectangle(left + thick / 2, top + thick / 2, d - thick, d - thick);

            using (var track = new Pen(Theme.Track, thick)) g.DrawEllipse(track, ring);

            float start = -90f;
            if (total > 0)
            {
                foreach (var s in Slices)
                {
                    if (s.Value <= 0) continue;
                    float sweep = 360f * s.Value / total;
                    using (var pen = new Pen(s.Color, thick)) g.DrawArc(pen, ring, start, sweep);
                    start += sweep;
                }
            }

            TextRenderer.DrawText(g, total.ToString(), fBig, new Rectangle(left, top + d / 2 - 28, d, 34),
                Theme.Text, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            TextRenderer.DrawText(g, "tasks", fSmall, new Rectangle(left, top + d / 2 + 6, d, 18),
                Theme.Muted, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            int lx = left + d + 40, rowH = 34;
            int ly = Height / 2 - Slices.Count * rowH / 2;
            for (int i = 0; i < Slices.Count; i++)
            {
                var s = Slices[i];
                int y = ly + i * rowH;
                using (var b = new SolidBrush(s.Color)) g.FillRectangle(b, lx, y + 11, 12, 12);
                int pct = total == 0 ? 0 : (int)Math.Round(100.0 * s.Value / total);
                TextRenderer.DrawText(g, string.Format("{0} · {1} ({2}%)", s.Label, s.Value, pct), fLegend,
                    new Rectangle(lx + 22, y, Math.Max(10, Width - lx - 22), rowH),
                    Theme.Muted, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }
        }
    }

    // Vertical bars ("Tasks by priority")
    public class BarChart : Control
    {
        public class Bar
        {
            public string Label; public Color Color; public int Value;
            public Bar(string label, Color color, int value) { Label = label; Color = color; Value = value; }
        }

        public List<Bar> Bars = new List<Bar>();
        readonly Font fVal = Theme.F(10f, FontStyle.Bold);
        readonly Font fLbl = Theme.F(9f);

        public BarChart() { DoubleBuffered = true; ResizeRedraw = true; BackColor = Color.White; }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (Bars.Count == 0) return;

            int pad = 16, baseY = Height - 34, topY = 26;
            int max = Math.Max(1, Bars.Max(b => b.Value));
            using (var pen = new Pen(Theme.Border)) g.DrawLine(pen, pad, baseY, Width - pad, baseY);

            float slot = (Width - 2f * pad) / Bars.Count;
            int bw = Math.Min(56, (int)(slot * 0.6f));

            for (int i = 0; i < Bars.Count; i++)
            {
                var b = Bars[i];
                float cx = pad + slot * (i + 0.5f);
                int h = (int)((baseY - topY - 4) * (b.Value / (float)max));
                var r = new Rectangle((int)(cx - bw / 2f), baseY - h, bw, h);

                if (h > 0)
                    using (var p = Theme.RoundTop(r, 6))
                    using (var br = new SolidBrush(b.Color))
                        g.FillPath(br, p);

                TextRenderer.DrawText(g, b.Value.ToString(), fVal,
                    new Rectangle((int)cx - 30, baseY - h - 24, 60, 20), Theme.Text,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(g, b.Label, fLbl,
                    new Rectangle((int)cx - 40, baseY + 6, 80, 22), Theme.Muted,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }
    }

    // Horizontal progress rows ("Completed tasks per person")
    public class PersonBars : Control
    {
        public List<KeyValuePair<string, int>> Rows = new List<KeyValuePair<string, int>>();
        readonly Font fName = Theme.F(9.5f);
        readonly Font fVal = Theme.F(9.5f, FontStyle.Bold);

        public PersonBars() { DoubleBuffered = true; ResizeRedraw = true; BackColor = Color.White; }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (Rows.Count == 0) return;

            int max = Math.Max(1, Rows.Max(r => r.Value));
            for (int i = 0; i < Rows.Count; i++)
            {
                int y = 10 + i * 44;
                var row = Rows[i];

                TextRenderer.DrawText(g, row.Key, fName, new Rectangle(0, y, 80, 24), Theme.Text,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

                var track = new Rectangle(82, y + 6, Math.Max(20, Width - 82 - 44), 12);
                using (var p = Theme.Round(track, 6))
                using (var b = new SolidBrush(Theme.Track))
                    g.FillPath(b, p);

                if (row.Value > 0)
                {
                    int fw = Math.Max(12, (int)(track.Width * row.Value / (float)max));
                    var fr = new Rectangle(track.X, track.Y, fw, track.Height);
                    using (var p = Theme.Round(fr, 6))
                    using (var b = new SolidBrush(Theme.Primary))
                        g.FillPath(b, p);
                }

                TextRenderer.DrawText(g, row.Value.ToString(), fVal, new Rectangle(Width - 36, y, 36, 24),
                    Theme.Text, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            }
        }
    }

    // Pink rows + footer ("Overdue tasks")
    public class OverdueList : Control
    {
        public class Item
        {
            public string Title, Assignee, Due;
            public Item(string title, string assignee, string due) { Title = title; Assignee = assignee; Due = due; }
        }

        public List<Item> Items = new List<Item>();
        public string Footer = "";
        readonly Font fTitle = Theme.F(9.5f, FontStyle.Bold);
        readonly Font fText = Theme.F(9.5f);
        readonly Font fFoot = Theme.F(9f);
        static readonly Color RowBg = Theme.Hex("#FDEEEE");
        static readonly Color RedText = Theme.Hex("#B42318");

        public OverdueList() { DoubleBuffered = true; ResizeRedraw = true; BackColor = Color.White; }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            for (int i = 0; i < Items.Count; i++)
            {
                var it = Items[i];
                var r = new Rectangle(0, 4 + i * 52, Width - 1, 40);
                using (var p = Theme.Round(r, 8))
                using (var b = new SolidBrush(RowBg))
                    g.FillPath(b, p);

                TextRenderer.DrawText(g, it.Title, fTitle, new Rectangle(r.X + 16, r.Y, (int)(Width * 0.5f) - 16, r.Height),
                    Theme.Text, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                TextRenderer.DrawText(g, it.Assignee, fText, new Rectangle((int)(Width * 0.55f), r.Y, 120, r.Height),
                    Theme.Muted, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                TextRenderer.DrawText(g, it.Due, fTitle, new Rectangle(r.Right - 96, r.Y, 80, r.Height),
                    RedText, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            }

            TextRenderer.DrawText(g, Footer, fFoot, new Rectangle(0, Height - 26, Width, 22),
                Theme.Muted, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }
    }
}
