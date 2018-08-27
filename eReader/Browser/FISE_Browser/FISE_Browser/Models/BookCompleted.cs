using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FISE_Browser.Models
{
    public class BookCompleted
    {
        public Book Book { get; set; }
        public bool IsCompleted { get; set; }
        public string ReviewJson { get; set; }
        public UserBook UserBook { get; set; }
    }
}