using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Text;
using Java.Util;
using Newtonsoft.Json;
using TaskManager.Model;
using TaskManager.Service;
using static Android.App.DatePickerDialog;
using static Android.App.TimePickerDialog;

namespace TaskManager.Activities
{
    [Activity(Label = "@string/add_task", Theme = "@style/AppTheme", MainLauncher = false)]
    public class AddTask : AppCompatActivity, IOnDateSetListener, IOnTimeSetListener
    {
        private EditText dueDate;
        private EditText dueTime;
        private Calendar calendar;
        private int hour = 7;
        private int minutes = 0;
        private TaskService TaskService;
        private Task task;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.add_task);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SetStatusSpinner();
            SetDueDatePicker();
            SetPrioritySpinner();
            var addTask = FindViewById<Button>(Resource.Id.add_button);
            addTask.Click += AddTask_Click;
            var cancelAdd = FindViewById<Button>(Resource.Id.cancel_button);
            cancelAdd.Click += CancelAdd_Click;
            TaskService = new TaskService();
            task = new Task();
            var isUpdate = Intent.GetStringExtra("SelectedTask") ?? string.Empty;
            if (isUpdate != string.Empty)
                EditTask();
        }

        private void EditTask()
        {
            task = JsonConvert.DeserializeObject<Task>(Intent.GetStringExtra("SelectedTask"));
            FindViewById<TextView>(Resource.Id.name).Text = task.Name;
            FindViewById<TextView>(Resource.Id.description).Text = task.Description;
            FindViewById<Spinner>(Resource.Id.status).SetSelection((int)task.Status);
            FindViewById<Spinner>(Resource.Id.priority).SetSelection((int)task.Priority);
            FindViewById<TextView>(Resource.Id.due_date).Text = task.DueDate.ToShortDateString();
            FindViewById<TextView>(Resource.Id.due_time).Text = task.DueDate.ToShortTimeString();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }

        private void CancelAdd_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        private void SetPrioritySpinner()
        {
            var priority = FindViewById<Spinner>(Resource.Id.priority);
            priority.Adapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, Enum.GetValues(typeof(Priority)).Cast<Priority>().Select(e => e.ToString()).ToArray());
        }

        private void AddTask_Click(object sender, EventArgs e)
        {
            task.Name = FindViewById<EditText>(Resource.Id.name).Text;
            task.Description = FindViewById<EditText>(Resource.Id.description).Text;
            task.Priority = (Priority)FindViewById<Spinner>(Resource.Id.priority).SelectedItemPosition;
            task.Status = (Status)FindViewById<Spinner>(Resource.Id.status).SelectedItemPosition;
            var date = $"{FindViewById<EditText>(Resource.Id.due_date).Text}{FindViewById<EditText>(Resource.Id.due_time).Text}";
            //if (date != string.Empty)
            //    task.DueDate = Convert.ToDateTime(date);
            if (this.task != null)
            {
                Intent newTask = new Intent(this, typeof(MainActivity));
                newTask.PutExtra("addnewtask", JsonConvert.SerializeObject(this.task));
                SetResult(Result.Ok, newTask);
                Finish();
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode != Result.Canceled)
            {
                if (requestCode == 1)
                {
                    var task = JsonConvert.DeserializeObject<Task>(data.GetStringExtra("editTask"));
                    if (task != null)
                    {
                        this.task = task;
                    }
                }
            }
        }

        private void SetDueDatePicker()
        {
            dueDate = FindViewById<EditText>(Resource.Id.due_date);
            dueDate.Click += DueDate_Click;
            dueTime = FindViewById<EditText>(Resource.Id.due_time);
            dueTime.Click += DueTime_Click;
        }

        private void DueTime_Click(object sender, EventArgs e)
        {
            var timePickerDialog = new TimePickerDialog(this, this, hour, minutes, true);
            timePickerDialog.Show();
        }

        private void DueDate_Click(object sender, EventArgs e)
        {
            calendar = Calendar.Instance;
            int year = calendar.Get(CalendarField.Year);
            int month = calendar.Get(CalendarField.Month);
            int day = calendar.Get(CalendarField.DayOfMonth);
            var datePickerDialog = new DatePickerDialog(this, this, year, month, day);
            datePickerDialog.Show();
        }

        private void SetStatusSpinner()
        {
            var spinner = FindViewById<Spinner>(Resource.Id.status);
            spinner.Adapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, Enum.GetValues(typeof(Status)).Cast<Status>().Select(e => e.ToString()).ToArray());
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                this.OnBackPressed();
            return base.OnOptionsItemSelected(item);
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            dueDate.Text = $"{dayOfMonth}-{month + 1}-{year}";
            calendar.Set(year, month, dayOfMonth);
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minutes)
        {
            this.hour = hourOfDay;
            this.minutes = minutes;
            var simpleDateFormat = new SimpleDateFormat("hh:mm");

            var date = new Date(0, 0, 0, hour, minutes);
            dueTime.Text = simpleDateFormat.Format(date);
        }
    }
}