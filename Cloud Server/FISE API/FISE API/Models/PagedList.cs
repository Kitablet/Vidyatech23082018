using System.Collections.Generic;

namespace FISE_API.Models
{
    public class PagedList<T>
    {
        public PagedList()
        {
            this.Items = new List<T>();
        }
        public int TotalItems { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; }
    }
}