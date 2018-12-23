using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Support.Design.Widget;
using TaskManager.Model;
using System.Collections.Generic;
using TaskManager.Service;
using System.Linq;
using Android.Views;
using Android.Content;
using TaskManager.Activities;
using TaskManager.Adaptors;
using System;
using Newtonsoft.Json;

namespace TaskManager
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public List<Task> RawTasks { get; set; }
        public ListView TaskList { get; set; }
        private TaskService TaskService { get; set; }
        public Task Task { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            TaskService = new TaskService();
            RawTasks = TaskService.GetAll();
            TaskList = FindViewById<ListView>(Resource.Id.mainlistview);
            Task = new Task();
            InitializeClickEvents();
            LoadSelectedTasks(Resource.Id.action_new);
        }

        private void InitializeClickEvents()
        {
            TaskList.ItemClick += TaskList_ItemClick;
            var floatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.floating_add_button);
            floatingActionButton.Click += FloatingActionButton_Click;
            var bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
            var search = FindViewById<EditText>(Resource.Id.searchText);
            search.TextChanged += Search_TextChanged;
        }

        private void Search_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            ((TaskListAdaptor)TaskList.Adapter).Filter.InvokeFilter(e.Text.ToString());
        }

        private void TaskList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //var t = RawTasks.FirstOrDefault(p => p.Id == e.Id + 1);
            //Toast.MakeText(this, t.Name, ToastLength.Short).Show();
            Intent newTask = new Intent(this, typeof(AddTask));
            newTask.PutExtra("editTask", JsonConvert.SerializeObject(this.RawTasks.FirstOrDefault(p => p.Id == e.Id + 1)));
            SetResult(Result.Ok, newTask);
            StartActivityForResult(newTask,1);
        }

        private void FloatingActionButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(AddTask));
            this.StartActivityForResult(intent, 1);
        }

        private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            LoadSelectedTasks(e.Item.ItemId);
        }

        private void LoadSelectedTasks(int id)
        {
            switch (id)
            {
                case Resource.Id.action_new:
                    TaskList.Adapter = GetTasks(Status.New);
                    break;
                case Resource.Id.action_in_progress:
                    TaskList.Adapter = GetTasks(Status.InProgress);
                    break;
                case Resource.Id.action_completed:
                    TaskList.Adapter = GetTasks(Status.Completed);
                    break;
            }
        }

        private TaskListAdaptor GetTasks(Status status)
        {
            return new TaskListAdaptor(this, RawTasks.Where(t => t.Status == status).ToList());
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var inflator = MenuInflater;
            inflator.Inflate(Resource.Menu.main_activity_actions, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.Settings:
                    var settings = new Intent(this, typeof(Settings));
                    StartActivity(settings);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode != Result.Canceled)
            {
                if (requestCode == 1)
                {
                    var task = JsonConvert.DeserializeObject<Task>(data.GetStringExtra("addnewtask"));
                    if (task != null)
                    {
                        TaskService.AddTask(task);
                        RawTasks.Add(task);
                    }
                    LoadSelectedTasks(Resource.Id.action_new);
                }
            }
        }
    }
}