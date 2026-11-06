using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Task_Mangement
{
    public static class Theme
    {
        public static Color Hex(string h) { return ColorTranslator.FromHtml(h); }

        public static readonly Color Sidebar = Hex("#1B2540");
        public static readonly Color SidebarText = Hex("#A9B3CC");
        public static readonly Color SidebarDot = Hex("#8A94AE");
        public static readonly Color Bg = Hex("#F4F6FA");
        public static readonly Color Border = Hex("#E3E7EF");
        public static readonly Color Text = Hex("#1A2238");
        public static readonly Color Muted = Hex("#5F6B85");
        public static readonly Color Primary = Hex("#2F6FED");
        public static readonly Color Track = Hex("#E8ECF4");
        public static readonly Color Green = Hex("#2E9E5B");
        public static readonly Color GreenLight = Hex("#3DD68C");
        public static readonly Color Red = Hex("#D92D20");
        public static readonly Color Orange = Hex("#E8821A");
        public static readonly Color Yellow = Hex("#E0B400");
        public static readonly Color Gray = Hex("#9AA3B5");

        public static Font F(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font("Segoe UI", size, style);
        }

        public static GraphicsPath Round(Rectangle r, int radius)
        {
            int d = radius * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        public static GraphicsPath RoundTop(Rectangle r, int radius)
        {
            var p = new GraphicsPath();
            int d = radius * 2;
            if (r.Height < d) { p.AddRectangle(r); return p; }
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddLine(r.Right, r.Y + radius, r.Right, r.Bottom);
            p.AddLine(r.Right, r.Bottom, r.X, r.Bottom);
            p.CloseFigure();
            return p;
        }

        // Keeps a child's Left/Width in sync with its parent's width.
        public static void Stretch(Control parent, Control c, int left, int right)
        {
            Action fit = () => { c.Left = left; c.Width = Math.Max(10, parent.Width - left - right); };
            parent.Resize += (s, e) => fit();
            fit();
        }

        public static Label CardTitle(string text, int x)
        {
            return new Label
            {
                Text = text,
                Font = F(11.5f, FontStyle.Bold),
                ForeColor = Text,
                BackColor = Color.White,
                AutoSize = true,
                Location = new Point(x, 18)
            };
        }
    }

    // White rounded card with a thin border.
    public class CardPanel : Panel
    {
        public int Radius = 12;

        public CardPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.White;
        }

        protected override void OnPaintBackground(PaintEventArgs e) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent != null ? Parent.BackColor : Theme.Bg);
            var r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var p = Theme.Round(r, Radius))
            using (var b = new SolidBrush(Color.White))
            using (var pen = new Pen(Theme.Border))
            {
                g.FillPath(b, p);
                g.DrawPath(pen, p);
            }
        }
    }

    // Primary = filled blue, otherwise white with border.
    public class RoundButton : Button
    {
        public bool Primary;

        public RoundButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            Cursor = Cursors.Hand;
            Font = Theme.F(9.5f);
            SetStyle(ControlStyles.Selectable, false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent != null ? Parent.BackColor : Theme.Bg);

            Color fill = Primary ? Theme.Primary : Color.White;
            Color fore = Primary ? Color.White : Theme.Text;
            if (!Enabled && Primary) fill = Theme.Hex("#9DB8F5");

            var r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var p = Theme.Round(r, Height >= 36 ? 8 : 6))
            using (var b = new SolidBrush(fill))
            using (var pen = new Pen(Primary ? fill : Theme.Border))
            {
                g.FillPath(b, p);
                g.DrawPath(pen, p);
            }
            TextRenderer.DrawText(g, Text, Font, r, fore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }

    // Small rounded badge (status / priority).
    public class Pill : Control
    {
        Color fill = Color.LightGray;

        public Pill()
        {
            DoubleBuffered = true;
            Font = Theme.F(8.5f, FontStyle.Bold);
            Height = 22;
        }

        public void Set(string text, Color fill, Color fore)
        {
            Text = text;
            this.fill = fill;
            ForeColor = fore;
            Width = TextRenderer.MeasureText(text, Font).Width + 22;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent != null ? Parent.BackColor : Theme.Bg);
            var r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var p = Theme.Round(r, (Height - 1) / 2))
            using (var b = new SolidBrush(fill))
                g.FillPath(b, p);
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }

    // Circle with a letter (comment author).
    public class Avatar : Control
    {
        readonly Font f = Theme.F(9.5f, FontStyle.Bold);

        public Avatar(string letter)
        {
            Text = letter;
            Size = new Size(32, 32);
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent != null ? Parent.BackColor : Color.White);
            using (var b = new SolidBrush(Theme.Hex("#E3EAFB")))
                g.FillEllipse(b, 0, 0, Width - 1, Height - 1);
            TextRenderer.DrawText(g, Text, f, ClientRectangle, Theme.Hex("#1E3A8A"),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }

    // Left navigation bar, shared by every screen.
    public class Sidebar : Control
    {
        static readonly string[] Items = { "Dashboard", "Projects", "Kanban board", "Reports", "Notifications", "Profile" };

        string active = "Projects";
        public string Active { get { return active; } set { active = value; Invalidate(); } }
        public int NotificationCount = 3;
        public string UserName = "", UserInfo = "", ServerText = "";
        public event Action<string> Navigate;

        readonly Font fTitle = Theme.F(13f, FontStyle.Bold);
        readonly Font fItem = Theme.F(10f);
        readonly Font fBold = Theme.F(10f, FontStyle.Bold);
        readonly Font fSmall = Theme.F(8.5f);
        readonly Font fSmallBold = Theme.F(8.5f, FontStyle.Bold);

        public Sidebar()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            Width = 232;
        }

        Rectangle ItemRect(int i) { return new Rectangle(12, 69 + i * 46, Width - 24, 42); }

        int HitTest(Point p)
        {
            for (int i = 0; i < Items.Length; i++)
                if (ItemRect(i).Contains(p)) return i;
            return -1;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Cursor = HitTest(e.Location) >= 0 ? Cursors.Hand : Cursors.Default;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            int i = HitTest(e.Location);
            if (i < 0) return;
            Active = Items[i];
            var h = Navigate;
            if (h != null) h(Items[i]);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Theme.Sidebar);

            TextRenderer.DrawText(g, "Task Manager", fTitle, new Point(22, 20), Color.White);

            for (int i = 0; i < Items.Length; i++)
            {
                var r = ItemRect(i);
                bool on = Items[i] == active;

                if (on)
                    using (var p = Theme.Round(r, 8))
                    using (var b = new SolidBrush(Theme.Primary))
                        g.FillPath(b, p);

                using (var b = new SolidBrush(on ? Color.White : Theme.SidebarDot))
                    g.FillEllipse(b, r.X + 20, r.Y + 18, 6, 6);

                TextRenderer.DrawText(g, Items[i], fItem,
                    new Rectangle(r.X + 40, r.Y, r.Width - 40, r.Height),
                    on ? Color.White : Theme.SidebarText,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

                if (Items[i] == "Notifications" && NotificationCount > 0)
                {
                    var br = new Rectangle(r.Right - 36, r.Y + 11, 26, 20);
                    using (var p = Theme.Round(br, 10))
                    using (var b = new SolidBrush(Theme.Hex("#EF4444")))
                        g.FillPath(b, p);
                    TextRenderer.DrawText(g, NotificationCount.ToString(), fSmallBold, br, Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                }
            }

            int y = Height - 124;
            using (var pen = new Pen(Color.FromArgb(40, 255, 255, 255)))
                g.DrawLine(pen, 16, y, Width - 16, y);
            using (var b = new SolidBrush(Theme.GreenLight))
                g.FillEllipse(b, 24, y + 22, 8, 8);
            TextRenderer.DrawText(g, ServerText, fSmall, new Point(38, y + 17), Color.White);
            TextRenderer.DrawText(g, UserName, fBold, new Point(22, y + 44), Color.White);
            TextRenderer.DrawText(g, UserInfo, fSmall, new Point(22, y + 70), Color.White);
        }
    }
}
