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
using Java.Lang;
using TaskManager.Model;

namespace TaskManager.Adaptors
{
    public class TaskListFilter : Filter
    {
        private readonly TaskListAdaptor taskListAdaptor;

        public TaskListFilter(TaskListAdaptor taskListAdaptor)
        {
            this.taskListAdaptor = taskListAdaptor;
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            var filterResults = new FilterResults();
            var results = new List<Task>();
            if (taskListAdaptor.rawTasks == null)
                taskListAdaptor.rawTasks = taskListAdaptor.tasks;
            if (constraint == null) return filterResults;
            if (taskListAdaptor.rawTasks != null)
            {
                results.AddRange(taskListAdaptor.rawTasks.Where(task => task.Name.ToLower().Contains(constraint.ToString().ToLower())));
            }
            filterResults.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
            filterResults.Count = results.Count;
            constraint.Dispose();
            return filterResults;
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            using (var values = results.Values)
            {
                this.taskListAdaptor.tasks = values.ToArray<Java.Lang.Object>().Select(a => a.ToNetObject<Task>()).ToList();
            }
            this.taskListAdaptor.NotifyDataSetChanged();
            constraint.Dispose();
            results.Dispose();
        }
    }

    public class JavaHolder : Java.Lang.Object
    {
        public readonly object Instance;

        public JavaHolder(object instance)
        {
            Instance = instance;
        }
    }

    public static class ObjectExtensions
    {
        public static TObject ToNetObject<TObject>(this Java.Lang.Object value)
        {
            if (value == null)
                return default(TObject);

            if (!(value is JavaHolder))
                throw new InvalidOperationException("Unable to convert to .NET object. Only Java.Lang.Object created with .ToJavaObject() can be converted.");

            TObject returnVal;
            try { returnVal = (TObject)((JavaHolder)value).Instance; }
            finally { value.Dispose(); }
            return returnVal;
        }

        public static Java.Lang.Object ToJavaObject<TObject>(this TObject value)
        {
            if (Equals(value, default(TObject)) && !typeof(TObject).IsValueType)
                return null;

            var holder = new JavaHolder(value);

            return holder;
        }
    }
}