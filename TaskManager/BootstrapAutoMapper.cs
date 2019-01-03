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
using AutoMapper;
using TaskManager.Model;

namespace TaskManager
{
    public class BootstrapAutoMapper
    {
        public static void InitializeAutoMapper()
        {
            Mapper.Initialize(x =>
            {
                x.CreateMap<Task, Task>();
            });
        }
    }
}