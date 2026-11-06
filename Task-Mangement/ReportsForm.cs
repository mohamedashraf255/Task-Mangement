using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Task_Mangement
{
    public class ReportsForm : Form
    {
        readonly List<TaskItem> tasks;
        readonly DateTime asOf = AppClock.Today;

        Label subtitle;
        DonutChart donut;
        BarChart bars;
        PersonBars people;
        OverdueList overdue;

        public ReportsForm(List<TaskItem> tasks)
        {
            this.tasks = tasks;
            Text = "Task Manager - Reports";
            ClientSize = new Size(1280, 800);
            MinimumSize = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Bg;
            Font = Theme.F(9.5f);
            DoubleBuffered = true;

            BuildLayout();
            LoadData();
        }

        void BuildLayout()
        {
            var side = new Sidebar
            {
                Dock = DockStyle.Left, Width = 232, Active = "Reports",
                UserName = "Yousef", UserInfo = "Owner · yousef@example.com",
                ServerText = "Connected · 127.0.0.1:5000"
            };

            side.Navigate += page => Nav.Go(this, page);

            var content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(28, 24, 28, 24), BackColor = Theme.Bg };

            // ---------- header ----------
            var header = new Panel { Dock = DockStyle.Top, Height = 86, BackColor = Theme.Bg };
            var title = new Label
            {
                Text = "Reports", Font = Theme.F(18f, FontStyle.Bold), ForeColor = Theme.Text,
                BackColor = Theme.Bg, AutoSize = true, Location = new Point(0, 0)
            };
            subtitle = new Label
            {
                Font = Theme.F(9f), ForeColor = Theme.Muted, BackColor = Theme.Bg,
                AutoSize = true, Location = new Point(0, 38)
            };

            var bPdf = new RoundButton { Text = "Export PDF", Size = new Size(120, 40) };
            var bExcel = new RoundButton { Text = "Export Excel", Size = new Size(133, 40) };
            var bCsv = new RoundButton { Text = "Export CSV", Size = new Size(120, 40) };
            bPdf.Click += (s, e) => MessageBox.Show("Plug a PDF library in here (PDFsharp / iTextSharp).");
            bExcel.Click += (s, e) => MessageBox.Show("Plug an Excel library in here (ClosedXML / EPPlus).");
            bCsv.Click += (s, e) => ExportCsv();

            header.Resize += (s, e) =>
            {
                bCsv.Left = header.Width - bCsv.Width;
                bExcel.Left = bCsv.Left - 8 - bExcel.Width;
                bPdf.Left = bExcel.Left - 8 - bPdf.Width;
            };
            header.Controls.AddRange(new Control[] { title, subtitle, bPdf, bExcel, bCsv });

            // ---------- 2x2 grid ----------
            var grid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, BackColor = Theme.Bg };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 52f));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 48f));

            donut = new DonutChart();
            bars = new BarChart();
            people = new PersonBars();
            overdue = new OverdueList();

            grid.Controls.Add(MakeCard("Tasks by status", donut, new Padding(0, 0, 9, 9)), 0, 0);
            grid.Controls.Add(MakeCard("Tasks by priority", bars, new Padding(9, 0, 0, 9)), 1, 0);
            grid.Controls.Add(MakeCard("Completed tasks per person", people, new Padding(0, 9, 9, 0)), 0, 1);
            grid.Controls.Add(MakeCard("Overdue tasks", overdue, new Padding(9, 9, 0, 0)), 1, 1);

            content.Controls.Add(grid);
            content.Controls.Add(header);
            Controls.Add(content);
            Controls.Add(side);
        }

        static CardPanel MakeCard(string title, Control chart, Padding margin)
        {
            var card = new CardPanel { Dock = DockStyle.Fill, Margin = margin };
            card.Controls.Add(Theme.CardTitle(title, 18));
            chart.Location = new Point(18, 52);
            card.Resize += (s, e) =>
                chart.Size = new Size(Math.Max(10, card.Width - 36), Math.Max(10, card.Height - 52 - 14));
            card.Controls.Add(chart);
            return card;
        }

        // Everything on screen is computed from the task list.
        void LoadData()
        {
            subtitle.Text = string.Format("All projects · {0} tasks · as of {1}",
                tasks.Count, asOf.ToString("MMM d yyyy", System.Globalization.CultureInfo.InvariantCulture));

            donut.Slices = new List<DonutChart.Slice>
            {
                new DonutChart.Slice("To do", Theme.Gray, tasks.Count(t => t.Status == WorkStatus.ToDo)),
                new DonutChart.Slice("In progress", Theme.Primary, tasks.Count(t => t.Status == WorkStatus.InProgress)),
                new DonutChart.Slice("Done", Theme.Green, tasks.Count(t => t.Status == WorkStatus.Done))
            };

            var prioColors = new Dictionary<Priority, Color>
            {
                { Priority.Urgent, Theme.Red }, { Priority.High, Theme.Orange },
                { Priority.Medium, Theme.Yellow }, { Priority.Low, Theme.Green }
            };
            bars.Bars = Enum.GetValues(typeof(Priority)).Cast<Priority>()
                .Select(p => new BarChart.Bar(p.ToString(), prioColors[p], tasks.Count(t => t.Priority == p)))
                .ToList();

            people.Rows = tasks.Select(t => t.Assignee).Distinct()
                .Select(n => new KeyValuePair<string, int>(
                    n, tasks.Count(t => t.Assignee == n && t.Status == WorkStatus.Done)))
                .OrderByDescending(kv => kv.Value)
                .ToList();

            var open = tasks.Where(t => t.Status != WorkStatus.Done).ToList();
            var late = open.Where(t => t.Deadline.Date < asOf).OrderBy(t => t.Deadline).ToList();
            overdue.Items = late.Select(t => new OverdueList.Item(t.Title, t.Assignee, Fmt.ShortDate(t.Deadline))).ToList();
            overdue.Footer = string.Format("{0} of {1} open tasks are past their deadline", late.Count, open.Count);

            donut.Invalidate(); bars.Invalidate(); people.Invalidate(); overdue.Invalidate();
        }

        void ExportCsv()
        {
            using (var dlg = new SaveFileDialog { Filter = "CSV file|*.csv", FileName = "tasks-report.csv" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                var sb = new StringBuilder();
                sb.AppendLine("Id,Title,Assignee,Project,Status,Priority,Deadline");
                foreach (var t in tasks)
                {
                    sb.AppendLine(string.Join(",", new[]
                    {
                        t.Id.ToString(), Csv(t.Title), Csv(t.Assignee), Csv(t.Project),
                        Fmt.Status(t.Status), t.Priority.ToString(), t.Deadline.ToString("yyyy-MM-dd")
                    }));
                }
                File.WriteAllText(dlg.FileName, sb.ToString(), new UTF8Encoding(true)); // BOM so Excel reads UTF-8
            }
        }

        static string Csv(string s)
        {
            return "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";
        }
    }
}
