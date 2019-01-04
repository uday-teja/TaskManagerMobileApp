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
        public List<Task> Tasks { get; set; }
        public ListView TaskListView { get; set; }
        private TaskService TaskService { get; set; }
        public Task Task { get; set; }

        // Unique ID for our notification: 
        static readonly int NOTIFICATION_ID = 1000;
        static readonly string CHANNEL_ID = "location_notification";
        internal static readonly string COUNT_KEY = "count";

        int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            TaskListView = FindViewById<ListView>(Resource.Id.mainlistview);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            TaskService = new TaskService();
            RawTasks = TaskService.GetAll();
            InitializeClickEvents();
            LoadSelectedTasks(Resource.Id.action_new);
            CreateNotificationChannel();
        }

        public void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var name = "This is Name";
            var description = "This is Description";
            var channel = new NotificationChannel(CHANNEL_ID, name, NotificationImportance.Default)
            {
                Description = description
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        public void GenerateNotification()
        {
            // Pass the current button press count value to the next activity:
            var valuesForActivity = new Bundle();
            valuesForActivity.PutInt(COUNT_KEY, count);

            // When the user clicks the notification, SecondActivity will start up.
            var resultIntent = new Intent(this, typeof(NotificationActivity));

            // Pass some values to SecondActivity:
            resultIntent.PutExtras(valuesForActivity);

            // Construct a back stack for cross-task navigation:
            var stackBuilder = TaskStackBuilder.Create(this);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(NotificationActivity)));
            stackBuilder.AddNextIntent(resultIntent);

            // Create the PendingIntent with the back stack:
            var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            // Build the notification:
            var builder = new NotificationCompat.Builder(this, CHANNEL_ID)
                          .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                          .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
                          .SetContentTitle("Button Clicked") // Set the title
                          .SetNumber(count) // Display the count in the Content Info
                          .SetSmallIcon(Resource.Drawable.logo) // This is the icon to display
                          .SetContentText($"The button has been clicked {count} times."); // the message to display.

            // Finally, publish the notification:
            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(NOTIFICATION_ID, builder.Build());

            // Increment the button press count:
            count++;
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
            //TaskListView.LongClick += TaskList_LongClick;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_activity_actions_menu, menu);
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
            var taskDetails = new Intent(this, typeof(DetailsActivity));
            taskDetails.PutExtra("taskDetails", JsonConvert.SerializeObject(this.Tasks.ElementAt((int)e.Id)));
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
            FindViewById<EditText>(Resource.Id.searchText).Text = string.Empty;
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
                        this.Tasks.Add(TaskService.GetLast());
                    }
                }
                else if (requestCode == 2)
                {
                    var task = JsonConvert.DeserializeObject<Task>(data.GetStringExtra("task"));
                    switch (JsonConvert.DeserializeObject<Crud>(data.GetStringExtra("type")))
                    {
                        case Crud.Update:
                            var updateRawData = this.RawTasks.FirstOrDefault(s => s.Id == task.Id);
                            var updateTask = this.Tasks.FirstOrDefault(s => s.Id == task.Id);
                            if (updateRawData.Status == task.Status)
                                Mapper.Map(task, updateTask);
                            else
                                this.Tasks.Remove(Tasks.FirstOrDefault(t => t.Id == task.Id));
                            Mapper.Map(task, updateRawData);
                            break;
                        case Crud.Delete:
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