using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SetContentView(Resource.Layout.add_task);
            SetStatusSpinner();
            SetDueDatePicker();
            var addTask = FindViewById<Button>(Resource.Id.add_button);
            addTask.Click += AddTask_Click;
        }

        private void AddTask_Click(object sender, EventArgs e)
        {
            TaskService = new TaskService();
            Task task = new Task();
            task.Name = "Yayy";
            TaskService.AddTask(task);
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
            TimePickerDialog timePickerDialog = new TimePickerDialog(this, this, hour, minutes, true);
            timePickerDialog.Show();
        }

        private void DueDate_Click(object sender, EventArgs e)
        {
            calendar = Calendar.Instance;
            int year = calendar.Get(CalendarField.Year);
            int month = calendar.Get(CalendarField.Month);
            int day = calendar.Get(CalendarField.DayOfMonth);
            DatePickerDialog datePickerDialog = new DatePickerDialog(this, this, year, month, day);
            datePickerDialog.Show();
        }

        private void SetStatusSpinner()
        {
            Spinner spinner = FindViewById<Spinner>(Resource.Id.task_status);
            var arrayForAdapter = Enum.GetValues(typeof(Status)).Cast<Status>().Select(e => e.ToString()).ToArray();
            spinner.Adapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, arrayForAdapter);
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
            SimpleDateFormat simpleDateFormat = new SimpleDateFormat("hh:mm:ss");
            Date date = new Date(0, 0, 0, hour, minutes);
            dueTime.Text = simpleDateFormat.Format(date);
        }
    }
}