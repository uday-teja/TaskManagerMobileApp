using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TaskManager.Model;
using static Android.Support.V7.Widget.RecyclerView;

namespace TaskManager.Adaptors
{
    public class TaskListAdaptor : BaseAdapter<Task>
    {
        private List<Task> tasks;

        public TaskListAdaptor(List<Task> tasks)
        {
            this.tasks = tasks;
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
            var view = convertView;
            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.task_row_view, parent, false);
                var name = view.FindViewById<TextView>(Resource.Id.taskName);
                var description = view.FindViewById<TextView>(Resource.Id.taskDescription);
                var priorityColor = view.FindViewById<LinearLayout>(Resource.Id.task_row_status);
                switch (tasks[position].Priority)
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
                view.Tag = new ViewHolder()
                {
                    Name = name,
                    Description = description,
                    PriorityColor = priorityColor
                };
            };
            var holder = (ViewHolder)view.Tag;
            holder.Name.Text = tasks[position].Name;
            holder.Description.Text = tasks[position].Description;
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