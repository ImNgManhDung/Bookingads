using System.Collections.Generic;
using BookingAds.Areas.Admin.Models.ManageEmployee;

namespace BookingAds.Areas.Admin.Repository.Employee.Abstractions
{
    using BookingAds.Entities;

    public interface IEmployeeRepository
    {
        IReadOnlyList<Employee> GetEmployees(ViewFilterEmployee viewData);

        int Count(ViewFilterEmployee viewData);

        bool UpdateStatusAccount(int status, long employeeID);
    }
}
