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

namespace TaskManager
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public List<Task> RawTasks { get; set; }
        public List<Task> Tasks { get; set; }
        public ListView TaskListView { get; set; }
        private TaskService TaskService { get; set; }
        public Task Task { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            BootstrapAutoMapper.InitializeAutoMapper();
            TaskListView = FindViewById<ListView>(Resource.Id.mainlistview);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            TaskService = new TaskService();
            RawTasks = TaskService.GetAll();
            InitializeClickEvents();
            LoadSelectedTasks(Resource.Id.action_new);
        }

        private void InitializeClickEvents()
        {
            TaskListView.ItemClick += TaskList_ItemClick;
            var floatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.floating_add_button);
            floatingActionButton.Click += FloatingActionButton_Click;
            var bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
            var search = FindViewById<EditText>(Resource.Id.searchText);
            search.TextChanged += Search_TextChanged;
            TaskListView.LongClick += TaskList_LongClick;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_activity_actions, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        private void TaskList_LongClick(object sender, View.LongClickEventArgs e)
        {
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }

        private void Search_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            ((TaskListAdaptor)TaskListView.Adapter).Filter.InvokeFilter(e.Text.ToString());
        }

        private void TaskList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var taskDetails = new Intent(this, typeof(TaskDetails));
            taskDetails.PutExtra("taskDetails", JsonConvert.SerializeObject(this.Tasks.ElementAt((int)e.Id)));
            this.StartActivityForResult(taskDetails, 2);
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
                    TaskListView.Adapter = GetTasks(Status.New);
                    break;
                case Resource.Id.action_in_progress:
                    TaskListView.Adapter = GetTasks(Status.InProgress);
                    break;
                case Resource.Id.action_completed:
                    TaskListView.Adapter = GetTasks(Status.Completed);
                    break;
            }
        }

        private TaskListAdaptor GetTasks(Status status)
        {
            this.Tasks = RawTasks.Where(t => t.Status == status).ToList();
            return new TaskListAdaptor(this, this.Tasks);
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
            if (resultCode != Result.Canceled)
            {
                if (requestCode == 1)
                {
                    var task = JsonConvert.DeserializeObject<Task>(data.GetStringExtra("newtask"));
                    if (task != null)
                    {
                        this.TaskService.AddTask(task);
                        this.RawTasks.Add(TaskService.GetLast());
                        this.Tasks.Add(TaskService.GetLast());
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
                            var updateTask = this.Tasks.FirstOrDefault(s => s.Id == task.Id);
                            if (updateRawData.Status == task.Status)
                                Mapper.Map(task, updateTask);
                            else
                                this.Tasks.Remove(Tasks.FirstOrDefault(t => t.Id == task.Id));
                            Mapper.Map(task, updateRawData);
                            break;
                        case Crud.Delete:
                            this.TaskService.DeleteTask(task);
                            this.RawTasks.Remove(RawTasks.FirstOrDefault(t => t.Id == task.Id));
                            var deleteTask = this.Tasks.FirstOrDefault(s => s.Id == task.Id);
                            if (deleteTask != null)
                                this.Tasks.Remove(Tasks.FirstOrDefault(t => t.Id == task.Id));
                            break;
                    }
                }
            }
            (TaskListView.Adapter as TaskListAdaptor).NotifyDataSetChanged();
            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}