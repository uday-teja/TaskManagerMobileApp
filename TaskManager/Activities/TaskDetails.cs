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
        public Task SelectedTask { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.task_details);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SelectedTask = JsonConvert.DeserializeObject<Task>(Intent.GetStringExtra("taskDetails"));
            FindViewById<TextView>(Resource.Id.taskdetailname).Text = SelectedTask.Name;
            FindViewById<TextView>(Resource.Id.taskDetaildescription).Text = SelectedTask.Description;
            FindViewById<TextView>(Resource.Id.taskDetailpriority).Text = SelectedTask.Priority.ToString();
            FindViewById<TextView>(Resource.Id.taskDetailstatus).Text = SelectedTask.Status.ToString();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.OnBackPressed();
                    break;
                case Resource.Id.edit:
                    Intent addTask = new Intent(this, typeof(AddTask));
                    addTask.PutExtra("SelectedTask", JsonConvert.SerializeObject(this.SelectedTask));
                    StartActivity(addTask);
                    break;
                case Resource.Id.delete:
                    DeleteTask();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void DeleteTask()
        {
            var dialog = new Android.Support.V7.App.AlertDialog.Builder(this);
            dialog.SetTitle("Delete Task").SetMessage("Are you sure to delete the task?").SetPositiveButton("Yes", OnDeleteTask).SetNegativeButton("No", delegate { });
            dialog.Create().Show();
        }

        private void OnDeleteTask(object sender, DialogClickEventArgs e)
        {
            if (this.SelectedTask != null)
            {
                Intent deleteTask = new Intent(this, typeof(MainActivity));
                deleteTask.PutExtra("addnewtask", JsonConvert.SerializeObject(SelectedTask));
                SetResult(Result.Ok, deleteTask);
                Finish();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
    }
}