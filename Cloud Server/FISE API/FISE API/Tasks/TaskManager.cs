using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace FISE_API.Tasks
{
    public class TaskManager
    {
        private static readonly TaskManager _taskManager = new TaskManager();
        private readonly List<TaskThread> _taskThreads = new List<TaskThread>();
        private int _notRunTasksInterval = 60 * 30; //30 minutes

        private TaskManager()
        {
        }

        /// <summary>
        /// Initializes the task manager with the property values specified in the configuration file.
        /// </summary>
        public void Initialize()
        {
            this._taskThreads.Clear();

            System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~\\App_Data\\Log.txt"), "Task Started:=" + DateTime.Now.ToString() + Environment.NewLine);
           
            List<ScheduleTask> scheduleTasks;
            
            using (var reader = new StreamReader(HttpContext.Current.Server.MapPath("~\\App_Data\\ScheduleTask.xml")))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<ScheduleTask>),
                    new XmlRootAttribute("ScheduleTasks"));
                scheduleTasks = (List<ScheduleTask>)deserializer.Deserialize(reader);
                scheduleTasks = scheduleTasks.OrderBy(x => x.Seconds).ToList();
            }

            foreach (var scheduleTask in scheduleTasks)
            {
                //create a thread
                var taskThread = new TaskThread
                {
                    Seconds = scheduleTask.Seconds
                };
                var task = new Task(scheduleTask);
                taskThread.AddTask(task);              
                this._taskThreads.Add(taskThread);
            }

            ////group by threads with the same seconds
            //foreach (var scheduleTaskGrouped in scheduleTasks.GroupBy(x => x.Seconds))
            //{
            //    //create a thread
            //    var taskThread = new TaskThread
            //    {
            //        Seconds = scheduleTaskGrouped.Key
            //    };
            //    foreach (var scheduleTask in scheduleTaskGrouped)
            //    {
            //        var task = new Task(scheduleTask);
            //        taskThread.AddTask(task);
            //    }
            //    this._taskThreads.Add(taskThread);
            //}

            //sometimes a task period could be set to several hours (or even days).
            //in this case a probability that it'll be run is quite small (an application could be restarted)
            //we should manually run the tasks which weren't run for a long time            
            List<ScheduleTask> notRunTasks = new List<ScheduleTask>();
            foreach (ScheduleTask item in scheduleTasks)
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("~\\App_Data\\" + item.ScheduleType + ".txt")))
                {
                    string LastStartUtc = string.Empty;

                    LastStartUtc = File.ReadAllText(HttpContext.Current.Server.MapPath("~\\App_Data\\" + item.ScheduleType + ".txt")).Split('#')[0].ToString();
                    if (string.IsNullOrEmpty(LastStartUtc))
                        File.WriteAllText(HttpContext.Current.Server.MapPath("~\\App_Data\\" + item.ScheduleType + ".txt"), DateTime.UtcNow.ToString() + "#" + DateTime.UtcNow + "#" + DateTime.UtcNow);

                    LastStartUtc = File.ReadAllText(HttpContext.Current.Server.MapPath("~\\App_Data\\" + item.ScheduleType + ".txt")).Split('#')[0].ToString();

                    if (!String.IsNullOrEmpty(LastStartUtc) || DateTime.Parse(LastStartUtc).AddSeconds(_notRunTasksInterval) < DateTime.UtcNow)
                        notRunTasks.Add(item);
                }
            }
            
            //create a thread for the tasks which weren't run for a long time
            if (notRunTasks.Count > 0)
            {
                var taskThread = new TaskThread
                {
                    RunOnlyOnce = true,
                    Seconds = 60 * 5 //let's run such tasks in 5 minutes after application start
                };
                foreach (var scheduleTask in notRunTasks)
                {
                    var task = new Task(scheduleTask);
                    taskThread.AddTask(task);
                }
                this._taskThreads.Add(taskThread);
            }
        }

        /// <summary>
        /// Starts the task manager
        /// </summary>
        public void Start()
        {
            foreach (var taskThread in this._taskThreads)
            {
                taskThread.InitTimer();
            }
        }

        /// <summary>
        /// Stops the task manager
        /// </summary>
        public void Stop()
        {
            foreach (var taskThread in this._taskThreads)
            {
                taskThread.Dispose();
            }
        }

        /// <summary>
        /// Gets the task mamanger instance
        /// </summary>
        public static TaskManager Instance
        {
            get
            {
                return _taskManager;
            }
        }

        /// <summary>
        /// Gets a list of task threads of this task manager
        /// </summary>
        public IList<TaskThread> TaskThreads
        {
            get
            {
                return new ReadOnlyCollection<TaskThread>(this._taskThreads);
            }
        }
    }
}