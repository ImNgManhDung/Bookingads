using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingAds.Areas.Admin.Models.Dashboard;

namespace BookingAds.Areas.Admin.Repository.Dashboard.Abstractions
{
    internal interface IDashboardRepository
    {
        int CountEmployee();

        int CountProduct();

        int CountOrder();

        int CountRevenue();

        IReadOnlyList<ViewTopThreeProductStatistic> TopThreeProductStatistic();

        IReadOnlyList<ViewRevenueOfCurrentMonthStatistic> RevenueOfCurrentMonthStatistic();
    }
}
