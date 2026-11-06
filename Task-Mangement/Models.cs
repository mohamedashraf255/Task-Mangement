using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace Task_Mangement
{
    public enum WorkStatus { ToDo, InProgress, Done }
    public enum Priority { Urgent, High, Medium, Low }

    public class Subtask { public string Title; public bool Done; }
    public class Comment { public string Author, When, Text; }

    public class TaskItem
    {
        public int Id;
        public string Title, Description, Assignee, Project;
        public WorkStatus Status;
        public Priority Priority;
        public DateTime Created, Deadline;
        public List<Subtask> Subtasks = new List<Subtask>();
        public List<Comment> Comments = new List<Comment>();
    }

    // The screenshots are dated Oct 31 2026. Replace with DateTime.Today in the real app.
    public static class AppClock
    {
        public static DateTime Today { get { return new DateTime(2026, 10, 31); } }
    }

    public static class Fmt
    {
        public static string Status(WorkStatus s)
        {
            switch (s)
            {
                case WorkStatus.ToDo: return "To do";
                case WorkStatus.InProgress: return "In progress";
                default: return "Done";
            }
        }

        // { background, text }
        public static Color[] StatusPill(WorkStatus s)
        {
            switch (s)
            {
                case WorkStatus.ToDo: return new[] { Theme.Hex("#E8ECF4"), Theme.Hex("#475569") };
                case WorkStatus.InProgress: return new[] { Theme.Hex("#DCE7FD"), Theme.Hex("#1E4FBF") };
                default: return new[] { Theme.Hex("#D9F2E3"), Theme.Hex("#1E7B45") };
            }
        }

        public static Color[] PriorityPill(Priority p)
        {
            switch (p)
            {
                case Priority.Urgent: return new[] { Theme.Hex("#FDE2E0"), Theme.Hex("#B42318") };
                case Priority.High: return new[] { Theme.Hex("#FDE8D3"), Theme.Hex("#B45309") };
                case Priority.Medium: return new[] { Theme.Hex("#FBF1C7"), Theme.Hex("#8A6D00") };
                default: return new[] { Theme.Hex("#D9F2E3"), Theme.Hex("#1E7B45") };
            }
        }

        public static string Date(DateTime d)
        {
            return d.ToString("MMM d, yyyy", CultureInfo.InvariantCulture);
        }

        public static string ShortDate(DateTime d)
        {
            return d.ToString("MMM d", CultureInfo.InvariantCulture);
        }

        public static string TimeLeft(DateTime deadline)
        {
            int d = (deadline.Date - AppClock.Today).Days;
            if (d < 0) return "Overdue by " + (-d) + (d == -1 ? " day" : " days");
            if (d == 0) return "Due today";
            return d + (d == 1 ? " day" : " days");
        }
    }

    public static class SampleData
    {
        static TaskItem T(int id, string title, string who, WorkStatus s, Priority p, DateTime created, DateTime deadline)
        {
            return new TaskItem
            {
                Id = id, Title = title, Assignee = who, Status = s, Priority = p,
                Created = created, Deadline = deadline, Project = "CS312 Project",
                Description = ""
            };
        }

        public static List<TaskItem> All()
        {
            var tcp = T(1, "Implement TCP server", "Yousef", WorkStatus.InProgress, Priority.High,
                        new DateTime(2026, 10, 24), new DateTime(2026, 11, 3));
            tcp.Description = "TcpListener on the server, one Dictionary<string, TcpClient> of online users. " +
                              "Messages are JSON: Login, Assign, Complete. Offline users get a row in the " +
                              "Notifications table and read it at next login.";
            tcp.Subtasks.Add(new Subtask { Title = "TcpListener + accept loop", Done = true });
            tcp.Subtasks.Add(new Subtask { Title = "Online users dictionary", Done = true });
            tcp.Subtasks.Add(new Subtask { Title = "Push notification on Assign", Done = false });
            tcp.Comments.Add(new Comment { Author = "Sara", When = "2 hours ago", Text = "Pushed the DB script, you can connect now." });
            tcp.Comments.Add(new Comment { Author = "Ahmed", When = "yesterday", Text = "Should the port be configurable?" });
            tcp.Comments.Add(new Comment { Author = "Yousef", When = "yesterday", Text = "Yes, from App settings, default 5000." });

            return new List<TaskItem>
            {
                tcp,
                T(2, "Create SQL script", "Ahmed", WorkStatus.ToDo, Priority.Urgent, new DateTime(2026, 10, 20), new DateTime(2026, 10, 29)),
                T(3, "Prepare demo scenario", "Yousef", WorkStatus.InProgress, Priority.Medium, new DateTime(2026, 10, 21), new DateTime(2026, 10, 30)),
                T(4, "Design login form", "Mona", WorkStatus.ToDo, Priority.High, new DateTime(2026, 10, 25), new DateTime(2026, 11, 5)),
                T(5, "Write unit tests", "Ahmed", WorkStatus.ToDo, Priority.Medium, new DateTime(2026, 10, 26), new DateTime(2026, 11, 8)),
                T(6, "Build Kanban board", "Sara", WorkStatus.ToDo, Priority.High, new DateTime(2026, 10, 26), new DateTime(2026, 11, 10)),
                T(7, "Design database schema", "Sara", WorkStatus.Done, Priority.Medium, new DateTime(2026, 10, 15), new DateTime(2026, 10, 22)),
                T(8, "Write project proposal", "Mona", WorkStatus.Done, Priority.Low, new DateTime(2026, 10, 12), new DateTime(2026, 10, 18)),
            };
        }
    }
}
