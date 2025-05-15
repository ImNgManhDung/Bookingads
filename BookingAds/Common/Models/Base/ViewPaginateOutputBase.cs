using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Common.Models.Base
{
    public class ViewPaginateOutputBase<T>
        where T : class
    {
        public int CurrentPage { get; set; }

        public int CurrentPageSize { get; set; }

        public int TotalRow { get; set; }

        public int TotalPage
        {
            get
            {
                return (int)Math.Ceiling((double)TotalRow / CurrentPageSize);
            }
        }

        public IReadOnlyList<T> Data { get; set; }
    }
}