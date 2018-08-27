using System.Collections.Generic;

namespace FISE_Cloud.Models
{
    public class APIPagedList<T>
    {
        public APIPagedList()
        {
            this.Items = new List<T>();
        }
        public int TotalItems { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; }
    }
}