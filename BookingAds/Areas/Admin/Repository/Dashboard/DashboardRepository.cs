using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using BookingAds.Areas.Admin.Repository.Dashboard.Abstractions;
using BookingAds.Modules;

namespace BookingAds.Areas.Admin.Repository.Dashboard
{
    using BookingAds.Areas.Admin.Models.Dashboard;
    using BookingAds.Entities;

    public class DashboardRepository : IDashboardRepository
    {
        public int CountEmployee()
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlCountEmployee = @"SELECT COUNT(EmployeeID) 
                    FROM tbEmployees";

                var command = new CommandDefinition(sqlCountEmployee, flags: CommandFlags.NoCache);

                var count = conn.QueryFirstOrDefault<int>(command);

                return count;
            }
        }

        public int CountProduct()
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlCountProduct = @"SELECT COUNT(ProductID) 
                    FROM tbProducts";

                var command = new CommandDefinition(sqlCountProduct, flags: CommandFlags.NoCache);

                var count = conn.QueryFirstOrDefault<int>(command);

                return count;
            }
        }

        public int CountOrder()
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlCountOrder = @"SELECT COUNT(OrderID) 
                    FROM tbOrders";

                var command = new CommandDefinition(sqlCountOrder, flags: CommandFlags.NoCache);

                var count = conn.QueryFirstOrDefault<int>(command);

                return count;
            }
        }

        public int CountRevenue()
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlCountRevenue = $@"SELECT SUM(TotalMoney) 
                    FROM tbOrders
                    WHERE Status = {OrderStatus.SUCCESSED.Code}";

                var command = new CommandDefinition(sqlCountRevenue, flags: CommandFlags.NoCache);

                var count = conn.QueryFirstOrDefault<int>(command);

                return count;
            }
        }

        public IReadOnlyList<ViewRevenueOfCurrentMonthStatistic> RevenueOfCurrentMonthStatistic()
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlRevenueOfCurrentMonthStatistic = $@"{GenRevenueOfCurrentMonthStatisticTempTable()}
                    SELECT t.Date
	                    , t.Day
	                    , t.Revenue
                    FROM #Tmp_RevenueOfCurrentMonth t";

                var command = new CommandDefinition(sqlRevenueOfCurrentMonthStatistic, flags: CommandFlags.NoCache);

                var data = conn.Query<ViewRevenueOfCurrentMonthStatistic>(command).ToList();

                return data;
            }
        }

        public IReadOnlyList<ViewTopThreeProductStatistic> TopThreeProductStatistic()
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlTopThreeProductStatistic = $@"SELECT TOP 3 f.ProductName
                    , COUNT(o.ProductID) AS CountOrder
                    FROM tbProducts f
                    LEFT JOIN tbOrders o
                    ON o.ProductID = f.ProductID
                    GROUP BY f.ProductName
                    ORDER BY CountOrder DESC";

                var command = new CommandDefinition(sqlTopThreeProductStatistic, flags: CommandFlags.NoCache);

                var data = conn.Query<ViewTopThreeProductStatistic>(command).ToList();

                return data;
            }
        }

        private static string GenRevenueOfCurrentMonthStatisticTempTable()
        {
            var sqlRevenueOfCurrentMonthStatisticCTE = $@"IF OBJECT_ID('tempdb..#Tmp_RevenueOfCurrentMonth') IS NOT NULL
    DROP TABLE #Tmp_RevenueOfCurrentMonth;
                CREATE TABLE #Tmp_RevenueOfCurrentMonth(
	                Date DATE,
	                Day INT,
	                Revenue BIGINT
                )

                DECLARE @day int = 1;
                DECLARE @currentDate DATE = GETDATE()
                WHILE (@day <= DAY(EOMONTH(@currentDate))) 
                BEGIN
	                DECLARE @date DATE = CAST(CONCAT(YEAR(@currentDate), '-', MONTH(@currentDate), '-', @day) AS DATE)
	                DECLARE	@revenue bigint = 0

	                SELECT @revenue = ISNULL(SUM(o.TotalMoney), 0)	
	                FROM tbOrders o
	                WHERE CAST(o.OrderedTime AS DATE) = @date
		                AND o.Status = {OrderStatus.SUCCESSED.Code}
	                GROUP BY CAST(o.OrderedTime AS DATE)

	                INSERT INTO #Tmp_RevenueOfCurrentMonth (
		                Date,
		                Day,
		                Revenue
	                ) VALUES (
		                @date,
		                @day,
		                @revenue
	                )

	                SET @day = @day + 1
                END";

            return sqlRevenueOfCurrentMonthStatisticCTE;
        }
    }
}