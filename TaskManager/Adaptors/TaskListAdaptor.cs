using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Java.Lang;
using TaskManager.Model;
using Object = Java.Lang.Object;

namespace TaskManager.Adaptors
{
    public class TaskListAdaptor : BaseAdapter<Task>, IFilterable
    {
        private readonly Activity activity;
        public List<Task> tasks { get; set; }
        private List<Task> allTasks { get; set; }
        public Filter Filter { get; set; }

        public TaskListAdaptor(Activity activity, List<Task> tasks)
        {
            this.activity = activity;
            this.allTasks = this.tasks = tasks;
            this.Filter = new TaskFilter(this);
        }

        public override Task this[int position]
        {
            get
            {
                return tasks[position];
            }
        }

        public override int Count
        {
            get
            {
                return tasks.Count;
            }
        }


        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.task_row_view, null);
            var task = tasks[position];
            var name = view.FindViewById<TextView>(Resource.Id.taskName);
            var description = view.FindViewById<TextView>(Resource.Id.taskDescription);
            var priorityColor = view.FindViewById<LinearLayout>(Resource.Id.task_row_status);
            switch (task.Priority)
            {
                case Priority.Low:
                    priorityColor.SetBackgroundColor(Color.ParseColor("#D3D3D3"));
                    break;
                case Priority.Medium:
                    priorityColor.SetBackgroundColor(Color.ParseColor("#fbbf79"));
                    break;
                case Priority.High:
                    priorityColor.SetBackgroundColor(Color.ParseColor("#bd322c"));
                    break;
            }
            name.Text = task.Name;
            description.Text = task.Description;
            return view;

        }

        public class TaskFilter : Filter
        {
            readonly TaskListAdaptor taskListAdaptor;

            public TaskFilter(TaskListAdaptor taskListAdaptor)
            {
                this.taskListAdaptor = taskListAdaptor;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                var results = new FilterResults();
                if (!string.IsNullOrEmpty(constraint.ToString()))
                {
                    this.taskListAdaptor.tasks = this.taskListAdaptor.allTasks.Where(s => s.Name.Contains(constraint.ToString())).ToList();
                    results.Count = this.taskListAdaptor.tasks.Count();
                }
                else
                {
                    this.taskListAdaptor.tasks = this.taskListAdaptor.allTasks;
                }
                return results;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                this.taskListAdaptor.NotifyDataSetChanged();
            }
        }
    }

    public class ViewHolder : Java.Lang.Object
    {
        public TextView Name { get; set; }
        public TextView Description { get; set; }
        public LinearLayout PriorityColor { get; set; }
    }
}