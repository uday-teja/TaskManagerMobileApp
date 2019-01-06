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
using AutoMapper;
using Android.Support.V4.App;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;

namespace TaskManager
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {
        public List<Task> RawTasks { get; set; }
        public Task Task { get; set; }
        public ListView TaskListView { get; set; }
        private TaskService TaskService { get; set; }
        private TaskListAdaptor TaskListAdaptor { get; set; }
        private Status currentStatus { get; set; }
        private SearchView SearchView { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            this.TaskService = new TaskService();
            this.RawTasks = TaskService.GetAll();
            this.TaskListView = FindViewById<ListView>(Resource.Id.mainlistview);
            SetToolbar();
            InitializeClickEvents();
            TaskListAdaptor = new TaskListAdaptor(this, this.RawTasks.Where(s => s.Status == Status.New).ToList());
            TaskListView.Adapter = TaskListAdaptor;
        }

        private void SetToolbar()
        {
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
        }

        private void InitializeClickEvents()
        {
            TaskListView.ItemClick += TaskList_ItemClick;
            var floatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.floating_add_button);
            floatingActionButton.Click += FloatingActionButton_Click;
            var bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
            SearchView = FindViewById<SearchView>(Resource.Id.searchView1);
            SearchView.QueryTextChange += SearchQueryTextChange;
        }

        private void SearchQueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            TaskListAdaptor.Filter.InvokeFilter(e.NewText);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_activity_actions_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        private void TaskList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var taskDetails = new Intent(this, typeof(DetailsActivity));
            taskDetails.PutExtra("taskDetails", JsonConvert.SerializeObject(this.TaskListAdaptor.Tasks[e.Position]));
            this.StartActivityForResult(taskDetails, 2);
        }

        private void FloatingActionButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(AddEditActivity));
            this.StartActivityForResult(intent, 1);
        }

        private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            LoadSelectedTasks(e.Item.ItemId);
            SearchView.SetQuery("", false);
            this.TaskListAdaptor.NotifyDataSetChanged();
        }

        private void LoadSelectedTasks(int id)
        {
            this.TaskListAdaptor.Tasks.Clear();
            switch (id)
            {
                case Resource.Id.action_new:
                    currentStatus = Status.New;
                    this.TaskListAdaptor.Tasks.AddRange(this.RawTasks.Where(s => s.Status == Status.New));
                    break;
                case Resource.Id.action_in_progress:
                    currentStatus = Status.InProgress;
                    this.TaskListAdaptor.Tasks.AddRange(this.RawTasks.Where(s => s.Status == Status.InProgress));
                    break;
                case Resource.Id.action_completed:
                    currentStatus = Status.Completed;
                    this.TaskListAdaptor.Tasks.AddRange(this.RawTasks.Where(s => s.Status == Status.Completed));
                    break;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.Settings:
                    var settings = new Intent(this, typeof(SettingsActivity));
                    StartActivity(settings);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode != Result.Canceled)
            {
                if (requestCode == 1)
                {
                    var task = JsonConvert.DeserializeObject<Task>(data.GetStringExtra("newtask"));
                    if (task != null)
                    {
                        this.TaskService.AddTask(task);
                        this.RawTasks.Add(TaskService.GetLast());
                        if (task.Status == currentStatus)
                            this.TaskListAdaptor.Tasks.Add(TaskService.GetLast());
                    }
                }
                else if (requestCode == 2)
                {
                    var task = JsonConvert.DeserializeObject<Task>(data.GetStringExtra("task"));
                    switch (JsonConvert.DeserializeObject<Crud>(data.GetStringExtra("type")))
                    {
                        case Crud.Update:
                            this.TaskService.UpdateTask(task);
                            var updateRawData = this.RawTasks.FirstOrDefault(s => s.Id == task.Id);
                            var updateTask = this.TaskListAdaptor.Tasks.FirstOrDefault(s => s.Id == task.Id);
                            if (updateRawData.Status == task.Status)
                            {
                                updateTask.Name = task.Name;
                                updateTask.Description = task.Description;
                                updateTask.DueDate = task.DueDate;
                                updateTask.Status = task.Status;
                                updateTask.Priority = task.Priority;
                            }
                            else
                                this.TaskListAdaptor.Tasks.Remove(this.TaskListAdaptor.Tasks.FirstOrDefault(t => t.Id == task.Id));
                            updateRawData.Name = task.Name;
                            updateRawData.Description = task.Description;
                            updateRawData.DueDate = task.DueDate;
                            updateRawData.Status = task.Status;
                            updateRawData.Priority = task.Priority;
                            break;
                        case Crud.Delete:
                            this.TaskService.DeleteTask(task);
                            this.RawTasks.Remove(RawTasks.FirstOrDefault(t => t.Id == task.Id));
                            this.TaskListAdaptor.Tasks.Remove(this.TaskListAdaptor.Tasks.FirstOrDefault(t => t.Id == task.Id));
                            break;
                    }
                }
            }
            this.TaskListAdaptor.NotifyDataSetChanged();
            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}