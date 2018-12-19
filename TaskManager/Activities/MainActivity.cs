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

namespace TaskManager
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public List<Task> RawTasks { get; set; }
        public ListView TaskList { get; set; }
        private TaskService TaskService { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            TaskService = new TaskService();
            RawTasks = TaskService.GetAll();
            TaskList = FindViewById<ListView>(Resource.Id.mainlistview);
            TaskList.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, RawTasks);
            TaskList.ItemClick += TaskList_ItemClick;
            var floatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.floating_add_button);
            floatingActionButton.Click += FloatingActionButton_Click;
            var bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
        }

        private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.Item.ItemId)
            {
                case Resource.Id.action_new:
                    TaskList.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, RawTasks.Where(t => t.Status == Status.New).ToList());
                    break;
                case Resource.Id.action_in_progress:
                    TaskList.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, RawTasks.Where(t => t.Status == Status.InProgress).ToList());
                    break;
                case Resource.Id.action_completed:
                    TaskList.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, RawTasks.Where(t => t.Status == Status.Completed).ToList());
                    break;
                default:
                    break;
            }
        }

        private void TaskList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var t = RawTasks.FirstOrDefault(p => p.Id == e.Id + 1);
            Toast.MakeText(this, t.Name, ToastLength.Short).Show();
        }

        private void FloatingActionButton_Click(object sender, System.EventArgs e)
        {
            var intent = new Intent(this, typeof(AddTask));
            this.StartActivity(intent);
        }
    }
}