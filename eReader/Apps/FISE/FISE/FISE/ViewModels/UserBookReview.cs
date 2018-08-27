using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kitablet.ViewModels
{
    [XmlRoot(ElementName = "Question")]
    public class Question
    {
        [XmlElement(ElementName = "Id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "Type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "Text")]
        public string Text { get; set; }
        [XmlElement(ElementName = "Value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "MaxWord")]
        public string MaxWord { get; set; }
        [XmlElement(ElementName = "Options")]
        public Options Options { get; set; }
    }

    [XmlRoot(ElementName = "Option")]
    public class Option
    {
        [XmlElement(ElementName = "ID")]
        public string ID { get; set; }
        [XmlElement(ElementName = "Text")]
        public string Text { get; set; }
        [XmlElement(ElementName = "Value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "Options")]
    public class Options
    {
        [XmlElement(ElementName = "Option")]
        public List<Option> Option { get; set; }
    }

    [XmlRoot(ElementName = "Questions")]
    public class Questions
    {
        [XmlElement(ElementName = "Question")]
        public List<Question> Question { get; set; }
    }

    [XmlRoot(ElementName = "BookReview")]
    public class UserBookReview
    {
        [XmlElement(ElementName = "Questions")]
        public Questions Questions { get; set; }
    }
}
