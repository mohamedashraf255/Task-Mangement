using System;
using System.Windows.Forms;

namespace Task_Mangement
{
    // Opens the screen that matches a sidebar item (same Show() style the Dashboard already uses).
    public static class Nav
    {
        public static void Go(Form from, string page)
        {
            Form next = null;
            switch (page)
            {
                case "Dashboard": next = new Dashboard(); break;
                case "Projects": next = new TaskDetailForm(SampleData.All()[0]); break;
                case "Kanban board": next = new WorkFlow(); break;
                case "Reports": next = new ReportsForm(SampleData.All()); break;
            }
            if (next == null || next.GetType() == from.GetType()) return;
            next.StartPosition = FormStartPosition.Manual;
            next.Location = from.Location;
            next.Show();
        }
    }
}
