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
        public List<Task> rawTasks;
        public List<Task> tasks;
        private readonly Activity activity;
        public Filter Filter { get; private set; }

        public TaskListAdaptor(Activity activity, List<Task> tasks)
        {
            this.activity = activity;
            this.tasks = tasks;
            Filter = new TaskListFilter(this);
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
    }

    public class ViewHolder : Java.Lang.Object
    {
        public TextView Name { get; set; }
        public TextView Description { get; set; }
        public LinearLayout PriorityColor { get; set; }
    }
}