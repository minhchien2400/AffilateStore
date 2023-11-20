using Microsoft.AspNetCore.Mvc.Filters;

namespace AffiliateStoreBE.Common.Models
{
    public class FilterModel
    {
        //Paging
        public virtual int Offset { get; set; } = 1;

        public virtual int Limit { get; set; } = 10;

        //Search
        public string SearchText { get; set; }

    }
}
