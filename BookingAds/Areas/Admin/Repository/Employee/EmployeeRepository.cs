using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using Dapper;
using BookingAds.Areas.Admin.Models.ManageEmployee;
using BookingAds.Areas.Admin.Repository.Employee.Abstractions;
using BookingAds.Constants;
using BookingAds.Modules;

namespace BookingAds.Areas.Admin.Repository.Employee
{
    using BookingAds.Entities;

    public class EmployeeRepository : IEmployeeRepository
    {
        public int Count(ViewFilterEmployee viewData)
        {
            int totalRow = 0;

            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetCountEmployeeByFilter = $@"SELECT COUNT(*)
                    FROM tbEmployees
                    WHERE {GenConditionEmployee(viewData)}";

                var parameters = new
                {
                    Gender = viewData.Gender,
                    SearchValue = $"%{viewData.SearchValue}%",
                    FilterField = viewData.Field,
                };

                var commandGetCountEmployeeByFilter = new CommandDefinition(
                        sqlGetCountEmployeeByFilter,
                        parameters: parameters,
                        flags: CommandFlags.NoCache);
                totalRow = conn.QuerySingleOrDefault<int>(commandGetCountEmployeeByFilter);
            }

            return totalRow;
        }

        public IReadOnlyList<Employee> GetEmployees(ViewFilterEmployee viewData)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetEmployees = $@"{GenEmployeePaginateCTE(viewData)}
                    /* Call CTE */
                    SELECT EmployeeID
                    , UserName
                    , Password
                    , FirstName
                    , LastName
                    , Gender
                    , Avatar
                    , LockedAt
                    , Coin 
                    , Phone
                    FROM EmployeePaginateCTE 
                    WHERE (@Page = 1 AND @PageSize = 0) /* No Paginate */
                    OR RowNum BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize) /* Paginate via Page and PageSize */";

                var parameters = new
                {
                    Gender = viewData.Gender,
                    SearchValue = $"%{viewData.SearchValue}%",
                    FilterField = viewData.Field,
                    Page = viewData.Page,
                    PageSize = viewData.PageSize,
                };

                var command = new CommandDefinition(
                        sqlGetEmployees,
                        parameters: parameters,
                        flags: CommandFlags.NoCache);
                var data = conn.Query<Employee>(command).ToList();

                return data;
            }
        }

        public bool UpdateStatusAccount(int status, long employeeId)
        {
            int result = 0;

            using (var conn = ConnectDB.BookingAdsDB())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var sqlUpdateStatusAccount = $@"UPDATE tbEmployees
                            SET LockedAt = {(status == AccountStatusConstant.LOCK ? "SYSDATETIME()" : "NULL")}
                            WHERE EmployeeID = @EmployeeID";

                        var parameters = new
                        {
                            EmployeeID = employeeId,
                        };

                        var commandUpdateStatusAccount = new CommandDefinition(sqlUpdateStatusAccount, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        result = conn.Execute(commandUpdateStatusAccount);

                        trans.Commit();
                    }
                    catch (SqlException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return result == 1;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "<Pending>")]
        private static string GenEmployeePaginateCTE(ViewFilterEmployee condition)
        {
            var sqlEmployeePaginateCTE = $@"WITH EmployeePaginateCTE AS (
                    SELECT EmployeeID
                    , UserName
                    , Password
                    , FirstName
                    , LastName
                    , Gender
                    , Avatar
                    , LockedAt
                    , Coin
                    , Phone
                    , ROW_NUMBER() OVER (ORDER BY EmployeeID) AS RowNum
                    FROM tbEmployees 
                    WHERE {GenConditionEmployee(condition)}
            )";

            return sqlEmployeePaginateCTE;
        }

        private static string GenConditionEmployee(ViewFilterEmployee condition)
        {
            var sqlCondition = new StringBuilder();

            if (sqlCondition == null)
            {
                return sqlCondition.ToString();
            }

            // default value when have not condition other
            sqlCondition.Append(" EmployeeID > 0 ");

            // set gender value
            if (condition.Gender != FilterGender.Default)
            {
                sqlCondition.Append(" AND Gender = @Gender ");
            }

            // set search value
            if (condition.Field == FilterField.FullName)
            {
                sqlCondition.Append(" AND (FirstName COLLATE Vietnamese_CI_AI LIKE @SearchValue ");
                sqlCondition.Append(" OR LastName COLLATE Vietnamese_CI_AI LIKE @SearchValue ");
                sqlCondition.Append(" OR LastName + ' ' + FirstName COLLATE Vietnamese_CI_AI LIKE @SearchValue ");
                sqlCondition.Append(" OR FirstName + ' ' + LastName COLLATE Vietnamese_CI_AI LIKE @SearchValue) ");
            }

            if (condition.Field == FilterField.FirstName)
            {
                sqlCondition.Append(" AND FirstName COLLATE Vietnamese_CI_AI LIKE @SearchValue ");
            }

            if (condition.Field == FilterField.LastName)
            {
                sqlCondition.Append(" AND LastName COLLATE Vietnamese_CI_AI LIKE @SearchValue ");
            }

            sqlCondition.Append(Environment.NewLine);
            return sqlCondition.ToString();
        }
    }
}