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
    [BroadcastReceiver(Enabled = true)]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var message = intent.GetStringExtra("message");
            var title = intent.GetStringExtra("title");
            var builder = new NotificationCompat.Builder(context)
                .SetDefaults((int)NotificationDefaults.All)
                         .SetSmallIcon(Resource.Drawable.logo)
                         .SetVisibility((int)NotificationVisibility.Public)
                         .SetContentTitle("A")
                         .SetContentText("b");
            var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            notificationManager.Notify(1, builder.Build());
        }
    }
}