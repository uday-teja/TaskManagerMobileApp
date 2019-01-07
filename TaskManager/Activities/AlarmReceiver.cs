using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace TaskManager.Activities
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        static readonly string CHANNEL_ID = "task_notification";

        public override void OnReceive(Context context, Intent intent)
        {
            var resultIntent = new Intent(context, typeof(MainActivity));
            resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);

            var pending = PendingIntent.GetActivity(context, 0, resultIntent, PendingIntentFlags.CancelCurrent);
            var builder = new NotificationCompat.Builder(context, CHANNEL_ID)
                           .SetAutoCancel(true)
                           .SetContentTitle(intent.GetStringExtra("title"))
                           .SetSmallIcon(Resource.Drawable.logo)
                           .SetContentText(intent.GetStringExtra("message"));

            builder.SetContentIntent(pending);

            var notification = builder.Build();
            var manager = NotificationManager.FromContext(context);
            manager.Notify(1337, notification);
        }
    }
}