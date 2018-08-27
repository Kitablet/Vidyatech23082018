using System;
using System.Web.Hosting;
using System.IO;
namespace FISE_API.Tasks
{
    public class Task
    {
        /// <summary>
        /// Ctor for Task
        /// </summary>
        private Task()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Ctor for Task
        /// </summary>
        /// <param name="task">Task </param>
        public Task(ScheduleTask task)
        {
            this.Type = task.ScheduleType;
            this.Enabled = task.Enabled;
            this.StopOnError = task.StopOnError;
            this.Name = task.Name;
        }

        private ITask CreateTask()
        {
            ITask task = null;
            if (this.Enabled)
            {
                var type2 = System.Type.GetType(this.Type);
                if (type2 != null)
                {
                    object instance;

                    instance = Activator.CreateInstance(type2);
                    task = instance as ITask;
                }
            }
            return task;
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="throwException">A value indicating whether exception should be thrown if some error happens</param>
        /// <param name="dispose">A value indicating whether all instances hsould be disposed after task run</param>
        public void Execute(string TaskType, bool throwException = false, bool dispose = true)
        {
            this.IsRunning = true;

            //background tasks has an issue with Autofac
            //because scope is generated each time it's requested
            //that's why we get one single scope here
            //this way we can also dispose resources once a task is completed           
            try
            {
                var task = this.CreateTask();
                if (task != null)
                {
                    this.LastStartUtc = DateTime.UtcNow;
                    //execute task
                    task.Execute();
                    this.LastEndUtc = this.LastSuccessUtc = DateTime.UtcNow;
                }
            }
            catch (Exception exc)
            {
                this.Enabled = !this.StopOnError;
                this.LastEndUtc = DateTime.UtcNow;

                if (throwException)
                    throw;
            }

            this.IsRunning = false;
            try
            {               
                File.WriteAllText(HostingEnvironment.MapPath(@"~/App_Data/" + TaskType + ".txt"), this.LastStartUtc.ToString() + "#" + this.LastEndUtc.ToString() + "#" + this.LastSuccessUtc.ToString());                
            }
            catch
            {
            }
        }

        /// <summary>
        /// A value indicating whether a task is running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Datetime of the last start
        /// </summary>
        public DateTime? LastStartUtc { get; private set; }

        /// <summary>
        /// Datetime of the last end
        /// </summary>
        public DateTime? LastEndUtc { get; private set; }

        /// <summary>
        /// Datetime of the last success
        /// </summary>
        public DateTime? LastSuccessUtc { get; private set; }

        /// <summary>
        /// A value indicating type of the task
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// A value indicating whether to stop task on error
        /// </summary>
        public bool StopOnError { get; private set; }

        /// <summary>
        /// Get the task name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A value indicating whether the task is enabled
        /// </summary>
        public bool Enabled { get; set; }
    }
}