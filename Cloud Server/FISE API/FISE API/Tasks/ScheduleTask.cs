using System.Collections.Generic;
using System.Xml.Serialization;

namespace FISE_API.Tasks
{


    [XmlRoot("ScheduleTasks")]
    public class ScheduleTasks
    {
        public ScheduleTasks() { Tasks = new List<ScheduleTask>(); }
        [XmlElement("ScheduleTask")]
        public List<ScheduleTask> Tasks { get; set; }
    }
    public class ScheduleTask
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlAttribute("ScheduleType")]
        public string ScheduleType { get; set; }

        /// <summary>
        /// Gets or sets the run period (in seconds)
        /// </summary>
        [XmlElement("Seconds")]
        public int Seconds { get; set; }

        
        /// <summary>
        /// Gets or sets the value indicating whether a task is enabled
        /// </summary>
        [XmlElement("Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a task should be stopped on some error
        /// </summary>
        [XmlElement("StopOnError")]
        public bool StopOnError { get; set; }        
    }
}