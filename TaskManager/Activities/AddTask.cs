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
using TaskManager.Model;

namespace TaskManager.Activities
{
    [Activity(Label = "@string/add_task", Theme = "@style/AppTheme", MainLauncher = false)]
    public class AddTask : AppCompatActivity
    {
        private EditText dueDate;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SetContentView(Resource.Layout.add_task);
            SetStatusSpinner();
            SetDueDatePicker();
        }

        private void SetDueDatePicker()
        {
            dueDate = FindViewById<EditText>(Resource.Id.due_date);
            dueDate.Click += DueDate_Click;
            EditText dueTime = FindViewById<EditText>(Resource.Id.due_time);
            dueTime.Click += DueTime_Click;
        }

        private void DueTime_Click(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                dueDate.Text = time.ToLongDateString();
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private void DueDate_Click(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                dueDate.Text = time.ToLongDateString();
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
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
    }

    public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        // TAG can be any string of your choice.
        public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();

        // Initialize this value to prevent NullReferenceExceptions.
        Action<DateTime> _dateSelectedHandler = delegate { };

        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
        {
            DatePickerFragment frag = new DatePickerFragment();
            frag._dateSelectedHandler = onDateSelected;
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currently = DateTime.Now;
            DatePickerDialog dialog = new DatePickerDialog(Activity, this, currently.Year, currently.Month - 1, currently.Day);
            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            Log.Debug(TAG, selectedDate.ToLongDateString());
            _dateSelectedHandler(selectedDate);
        }
    }
}