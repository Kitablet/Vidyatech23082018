using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kitablet.ViewModels
{
    [XmlRoot(ElementName = "BookFile")]
    public class BookFile
    {
        [XmlElement(ElementName = "FileName")]
        public string FileName { get; set; }
        [XmlElement(ElementName = "FileURL")]
        public string FileURL { get; set; }
        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }
    }

    [XmlRoot(ElementName = "BookFiles")]
    public class BookFiles
    {
        [XmlElement(ElementName = "BookFile")]
        public List<BookFile> BookFile { get; set; }
    }

    [XmlRoot(ElementName = "File")]
    public class DownloadFile
    {
        [XmlElement(ElementName = "BookID")]
        public string BookID { get; set; }
        [XmlElement(ElementName = "BookFiles")]
        public BookFiles BookFiles { get; set; }
        [XmlElement(ElementName = "IsDownloaded")]
        public string IsDownloaded { get; set; }
        [XmlElement(ElementName = "IsUnZip")]
        public string IsUnZip { get; set; }
        [XmlElement(ElementName = "IsEncrypted")]
        public string IsEncrypted { get; set; }
        [XmlElement(ElementName = "IsDecrypted")]
        public string IsDecrypted { get; set; }
        [XmlElement(ElementName = "IsProcessing")]
        public string IsProcessing { get; set; }
    }

    [XmlRoot(ElementName = "Files")]
    public class BooksStatus
    {
        [XmlElement(ElementName = "File")]
        public List<DownloadFile> DownloadFile { get; set; }
    }
}
