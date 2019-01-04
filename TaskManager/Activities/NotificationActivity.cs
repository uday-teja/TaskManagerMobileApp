using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TaskManager.Activities
{
    [Activity(Label = "Notification Activity")]
    public class NotificationActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Get the count value passed to us from MainActivity:
            var count = Intent.Extras.GetInt(MainActivity.COUNT_KEY, -1);

            // No count was passed? Then just return.
            if (count <= 0)
            {
                return;
            }

            // Display the count sent from the first activity:
            SetContentView(Resource.Layout.notifications_view);
            //var txtView = FindViewById<TextView>(Resource.Id.notificationtext);
            //txtView.Text = $"You clicked the button {count} times in the previous activity.";
        }
    }
}