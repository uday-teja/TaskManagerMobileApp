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

namespace TaskManager
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public List<Task> RawTasks { get; set; }
        private TaskService TaskService { get; set; }
        public ListView TaskList { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            TaskService = new TaskService();
            TaskService.AddTask(new Task() { Name = "First", Description = "First Task" });
            TaskService.AddTask(new Task() { Name = "Second", Description = "Second Task" });
            TaskService.AddTask(new Task() { Name = "Third", Description = "Third Task" });
            RawTasks = TaskService.GetAll();

            TaskList = FindViewById<ListView>(Resource.Id.mainlistview); 
            TaskList.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, RawTasks);
            TaskList.ItemClick += (s, e) =>
            {
                var t = RawTasks.FirstOrDefault(p => p.Id == e.Id + 1);
                Toast.MakeText(this, t.Name, ToastLength.Long).Show();
            };

            var bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);

            bottomNavigation.NavigationItemSelected += (s, e) =>
            {
                switch (e.Item.ItemId)
                {
                    case Resource.Id.action_new:
                        Toast.MakeText(this, "New Clicked", ToastLength.Short).Show();
                        RawTasks.Where(t => t.Status == Status.New);
                        break;
                    case Resource.Id.action_in_progress:
                        Toast.MakeText(this, "In Progress Clicked", ToastLength.Short).Show();
                        RawTasks.Where(t => t.Status == Status.InProgress);
                        break;
                    case Resource.Id.action_completed:
                        Toast.MakeText(this, "Completed Clicked", ToastLength.Short).Show();
                        RawTasks.Where(t => t.Status == Status.Completed);
                        break;
                    default:
                        break;
                }
            };
        }
    }
}