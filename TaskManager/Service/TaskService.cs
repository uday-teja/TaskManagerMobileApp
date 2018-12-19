using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using TaskManager.Model;
using Environment = System.Environment;

namespace TaskManager.Service
{
    public class TaskService
    {
        string databaseFileName;

        public TaskService()
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "CanFindLocation");

            // Create the folder path.
            System.IO.Directory.CreateDirectory(applicationFolderPath);

            databaseFileName = System.IO.Path.Combine(applicationFolderPath, "CanFindLocation.db");
            var db = new SQLiteConnection(databaseFileName);

            db.CreateTable<Task>();
        }

        public void AddTask(Task task)
        {
            using (var db = new SQLiteConnection(databaseFileName))
            {
                db.Insert(task);
            }
        }

        public void UpdateTask(Task task)
        {
            using (var db = new SQLiteConnection(databaseFileName))
            {
                db.Update(task);
            }
        }

        public void DeleteTask(Task task)
        {
            using (var db = new SQLiteConnection(databaseFileName))
            {
                db.Delete(task);
            }
        }

        public List<Task> GetAll()
        {
            List<Task> tasks;
            using (var db = new SQLiteConnection(databaseFileName))
            {
                tasks = new List<Task>(db.Table<Task>());
            }
            return tasks;
        }

        public Task Get(Task task)
        {
            Task Task;

            using (var db = new SQLiteConnection(databaseFileName))
            {
                Task = db.Get<Task>(task.Id);
            }

            return Task;
        }
    }
}