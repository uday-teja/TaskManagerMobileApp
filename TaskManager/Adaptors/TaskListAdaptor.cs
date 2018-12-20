using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
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
            }
            var holder = (ViewHolder)view.Tag;
            //holder.Name.Text = tasks[position].Name;
            //holder.de.Text = tasks[position].Description;
            return view;
        }
    }
}