using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using TaskManager.Model;

namespace TaskManager.Activities
{
    [Activity(Label = "Task Details", Theme = "@style/AppTheme", MainLauncher = false)]
    public class TaskDetails : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SetContentView(Resource.Layout.task_details);
            var selectedTask = JsonConvert.DeserializeObject<Task>(Intent.GetStringExtra("taskDetails"));
            FindViewById<TextView>(Resource.Id.taskdetailname).Text = selectedTask.Name;
            FindViewById<TextView>(Resource.Id.taskDetaildescription).Text = selectedTask.Description;
            FindViewById<TextView>(Resource.Id.taskDetailpriority).Text = selectedTask.Priority.ToString();
            FindViewById<TextView>(Resource.Id.taskDetailstatus).Text = selectedTask.Status.ToString();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                this.OnBackPressed();
            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }
    }
}