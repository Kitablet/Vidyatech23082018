using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FISE_Browser.Models
{
    public class Opf
    {
        public string BookId { get; set; }
        public bool Istwopager { get; set; }
        public int totalpages { get; set; }
        public string bookmark { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string pagewidth1 { get; set; }
        public string pagewidth2 { get; set; }
        public List<BookDetails> _list { get; set; }
        public Book book { get; set; }
    }

    public class BookDetails
    {
        public string id { get; set; }
        public string idref { get; set; }
        public string href { get; set; }
        public string height1 { get; set; }
        public string height2 { get; set; }
        public string pageno { get; set; }
    }

}