using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Task_Mangement
{
    public class TaskDetailForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);
        const int EM_SETCUEBANNER = 0x1501;

        readonly TaskItem task;
        Pill pillStatus, pillPriority;
        RoundButton btnTodo, btnProgress, btnDone, btnMark;
        FlowLayoutPanel commentList;
        TextBox txtComment;

        public TaskDetailForm(TaskItem task)
        {
            this.task = task;
            Text = "Task Manager - " + task.Title;
            ClientSize = new Size(1280, 880);
            MinimumSize = new Size(1000, 760);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Bg;
            Font = Theme.F(9.5f);
            DoubleBuffered = true;

            BuildLayout();
            RefreshStatus();
        }

        void BuildLayout()
        {
            var side = new Sidebar
            {
                Dock = DockStyle.Left, Width = 232, Active = "Projects",
                UserName = "Yousef", UserInfo = "Owner · yousef@example.com",
                ServerText = "Connected · 127.0.0.1:5000"
            };

            side.Navigate += page => Nav.Go(this, page);

            var content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(28, 24, 28, 24), BackColor = Theme.Bg };

            // ---------- header ----------
            var header = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Theme.Bg };
            var back = new LinkLabel
            {
                Text = "← Back to " + task.Project, AutoSize = true, Location = new Point(0, 0),
                LinkColor = Theme.Primary, ActiveLinkColor = Theme.Primary,
                LinkBehavior = LinkBehavior.HoverUnderline, Font = Theme.F(9f), BackColor = Theme.Bg
            };
            back.Click += (s, e) => Close();

            var title = new Label
            {
                Text = task.Title, Font = Theme.F(18f, FontStyle.Bold), ForeColor = Theme.Text,
                AutoSize = true, Location = new Point(0, 22), BackColor = Theme.Bg
            };
            pillStatus = new Pill { Location = new Point(0, 68) };
            pillPriority = new Pill { Top = 68 };

            var btnEdit = new RoundButton { Text = "Edit", Size = new Size(90, 40) };
            var btnDelete = new RoundButton { Text = "Delete", Size = new Size(90, 40) };
            btnDelete.Click += (s, e) =>
            {
                if (MessageBox.Show("Delete this task?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    Close();
            };
            header.Resize += (s, e) =>
            {
                btnDelete.Left = header.Width - btnDelete.Width;
                btnEdit.Left = btnDelete.Left - btnEdit.Width - 8;
            };
            header.Controls.AddRange(new Control[] { back, title, pillStatus, pillPriority, btnEdit, btnDelete });

            // ---------- body: [left column | right column 332px] ----------
            var body = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Theme.Bg };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 332f));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var left = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2,
                Margin = new Padding(0, 0, 20, 0), BackColor = Theme.Bg
            };
            left.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, 282f));
            left.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            left.Controls.Add(BuildDescriptionCard(), 0, 0);
            left.Controls.Add(BuildCommentsCard(), 0, 1);

            var right = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2,
                Margin = new Padding(0), BackColor = Theme.Bg
            };
            right.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 350f));
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 189f));
            right.Controls.Add(BuildDetailsCard(), 0, 0);
            right.Controls.Add(BuildStatusCard(), 0, 1);

            body.Controls.Add(left, 0, 0);
            body.Controls.Add(right, 1, 0);

            // dock order: later-added controls dock first
            content.Controls.Add(body);
            content.Controls.Add(header);
            Controls.Add(content);
            Controls.Add(side);
        }

        // ---------- cards ----------
        CardPanel BuildDescriptionCard()
        {
            var card = new CardPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 20) };
            card.Controls.Add(Theme.CardTitle("Description", 24));

            var desc = new Label
            {
                Text = task.Description, Font = Theme.F(9.5f), ForeColor = Theme.Muted,
                BackColor = Color.White, Top = 50, Height = 70
            };
            Theme.Stretch(card, desc, 24, 24);
            card.Controls.Add(desc);

            card.Controls.Add(new Label
            {
                Text = "Subtasks", Font = Theme.F(9.5f, FontStyle.Bold), ForeColor = Theme.Text,
                BackColor = Color.White, AutoSize = true, Location = new Point(24, 138)
            });

            for (int i = 0; i < task.Subtasks.Count; i++)
            {
                var st = task.Subtasks[i];
                var cb = new CheckBox
                {
                    Text = st.Title, Checked = st.Done, AutoSize = true, ForeColor = Theme.Muted,
                    BackColor = Color.White, Location = new Point(24, 168 + i * 26)
                };
                cb.CheckedChanged += (s, e) => st.Done = cb.Checked;   // TODO: save to DB
                card.Controls.Add(cb);
            }
            return card;
        }

        CardPanel BuildCommentsCard()
        {
            var card = new CardPanel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            card.Controls.Add(Theme.CardTitle("Comments", 24));

            commentList = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true,
                Location = new Point(24, 52), BackColor = Color.White
            };
            txtComment = new TextBox { Font = Theme.F(10.5f), BorderStyle = BorderStyle.FixedSingle };
            txtComment.HandleCreated += (s, e) =>
                SendMessage(txtComment.Handle, EM_SETCUEBANNER, 1, "Write a comment...");

            var btnSend = new RoundButton { Text = "Send", Primary = true, Size = new Size(100, 40) };
            btnSend.Font = Theme.F(9.5f, FontStyle.Bold);
            btnSend.Click += (s, e) => SendComment();

            card.Resize += (s, e) =>
            {
                commentList.Width = Math.Max(10, card.Width - 48);
                commentList.Height = Math.Max(40, card.Height - 52 - 112);
                txtComment.SetBounds(24, card.Height - 100, Math.Max(10, card.Width - 48), 28);
                btnSend.Location = new Point(card.Width - 24 - btnSend.Width, card.Height - 54);
            };
            card.Controls.Add(commentList);
            card.Controls.Add(txtComment);
            card.Controls.Add(btnSend);

            foreach (var c in task.Comments) AddCommentRow(c);
            return card;
        }

        void AddCommentRow(Comment c)
        {
            var row = new Panel { Size = new Size(700, 76), Margin = new Padding(0), BackColor = Color.White };
            row.Controls.Add(new Avatar(c.Author.Substring(0, 1)) { Location = new Point(4, 2) });
            row.Controls.Add(new Label
            {
                Text = c.Author + " · " + c.When, Font = Theme.F(9f, FontStyle.Bold), ForeColor = Theme.Text,
                BackColor = Color.White, AutoSize = true, Location = new Point(48, 0)
            });
            row.Controls.Add(new Label
            {
                Text = c.Text, Font = Theme.F(9.5f), ForeColor = Theme.Muted, BackColor = Color.White,
                AutoSize = true, MaximumSize = new Size(640, 0), Location = new Point(48, 22)
            });
            commentList.Controls.Add(row);
            commentList.ScrollControlIntoView(row);
        }

        void SendComment()
        {
            string text = txtComment.Text.Trim();
            if (text.Length == 0) return;
            var c = new Comment { Author = "Yousef", When = "just now", Text = text };
            task.Comments.Add(c);          // TODO: send to server / save to DB
            AddCommentRow(c);
            txtComment.Clear();
        }

        CardPanel BuildDetailsCard()
        {
            var card = new CardPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 20) };
            card.Controls.Add(Theme.CardTitle("Details", 24));

            var rows = new[]
            {
                new[] { "Assignee", task.Assignee },
                new[] { "Project", task.Project },
                new[] { "Created", Fmt.Date(task.Created) },
                new[] { "Deadline", Fmt.Date(task.Deadline) },
                new[] { "Time left", Fmt.TimeLeft(task.Deadline) }
            };

            for (int i = 0; i < rows.Length; i++)
            {
                int y = 60 + i * 44;
                card.Controls.Add(new Label
                {
                    Text = rows[i][0], Font = Theme.F(9f), ForeColor = Theme.Muted, BackColor = Color.White,
                    AutoSize = true, Location = new Point(24, y + 5)
                });

                var val = new Label
                {
                    Text = rows[i][1], Font = Theme.F(9.5f, FontStyle.Bold), ForeColor = Theme.Text,
                    BackColor = Color.White, TextAlign = ContentAlignment.TopRight, Height = 22, Top = y + 3
                };
                Theme.Stretch(card, val, 24, 24);
                card.Controls.Add(val);

                var line = new Panel { BackColor = Theme.Border, Height = 1, Top = y + 36 };
                Theme.Stretch(card, line, 24, 24);
                card.Controls.Add(line);
            }
            return card;
        }

        CardPanel BuildStatusCard()
        {
            var card = new CardPanel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            card.Controls.Add(Theme.CardTitle("Status", 24));

            btnTodo = SegButton("To do", WorkStatus.ToDo);
            btnProgress = SegButton("In progress", WorkStatus.InProgress);
            btnDone = SegButton("Done", WorkStatus.Done);

            btnMark = new RoundButton { Text = "Mark as done", Primary = true, Height = 40 };
            btnMark.Font = Theme.F(9.5f, FontStyle.Bold);
            btnMark.Click += (s, e) => SetStatus(WorkStatus.Done);

            card.Resize += (s, e) =>
            {
                int w = (card.Width - 48 - 12) / 3;
                btnTodo.SetBounds(24, 56, w, 30);
                btnProgress.SetBounds(24 + w + 6, 56, w, 30);
                btnDone.SetBounds(24 + 2 * (w + 6), 56, w, 30);
                btnMark.SetBounds(24, 112, Math.Max(10, card.Width - 48), 40);
            };
            card.Controls.AddRange(new Control[] { btnTodo, btnProgress, btnDone, btnMark });
            return card;
        }

        RoundButton SegButton(string text, WorkStatus st)
        {
            var b = new RoundButton { Text = text, Font = Theme.F(8.5f) };
            b.Click += (s, e) => SetStatus(st);
            return b;
        }

        void SetStatus(WorkStatus st)
        {
            task.Status = st;              // TODO: save to DB + notify server
            RefreshStatus();
        }

        void RefreshStatus()
        {
            btnTodo.Primary = task.Status == WorkStatus.ToDo;
            btnProgress.Primary = task.Status == WorkStatus.InProgress;
            btnDone.Primary = task.Status == WorkStatus.Done;
            btnMark.Enabled = task.Status != WorkStatus.Done;
            foreach (var b in new[] { btnTodo, btnProgress, btnDone, btnMark }) b.Invalidate();

            var sc = Fmt.StatusPill(task.Status);
            pillStatus.Set(Fmt.Status(task.Status), sc[0], sc[1]);
            var pc = Fmt.PriorityPill(task.Priority);
            pillPriority.Set(task.Priority.ToString(), pc[0], pc[1]);
            pillPriority.Left = pillStatus.Right + 8;
        }
    }
}
