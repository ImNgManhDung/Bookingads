using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookingAds.Areas.Admin.Repository.Supports.Abstractions;
using BookingAds.Modules;
using Dapper;

namespace BookingAds.Areas.Admin.Repository.Supports
{
    using BookingAds.Areas.Admin.Models.Supports;
    using BookingAds.Entities;
    using MailKit.Search;

    public class SupportsRepository : ISupportsRepository
    {
        public int Count(ViewFilterSupports viewData)
        {
            int totalRow = 0;
            using (var conn = ConnectDB.BookingAdsDB())
            {
                string sqlGetCountHistoryFilter = $@"SELECT COUNT(*)
                    FROM tbSupports o
                    INNER JOIN dbo.tbEmployees e ON e.EmployeeID = o.EmployeeID 
                    INNER JOIN tbProducts AS f ON f.ProductID = o.ProductID
                    WHERE {GenConditionHistoryProduct(viewData)}";

                var dateStart = DateTimeUtils.ConvertToDateTimeSQL(viewData.DateStart);
                var dateEnd = DateTimeUtils.ConvertToDateTimeSQL(viewData.DateEnd);

                var parameters = new
                {

                    Status = viewData.OrderStatus,
                    SearchValue = $"%{viewData.SearchValue}%",
                    DateStart = dateStart,
                    DateEnd = dateEnd,
                };

                var commandGetCountHistoryFilter = new CommandDefinition(
                        sqlGetCountHistoryFilter,
                        parameters: parameters,
                        flags: CommandFlags.NoCache);

                totalRow = conn.QueryFirstOrDefault<int>(commandGetCountHistoryFilter);
            }

            return totalRow;
        }

        public int CreateSupports(ViewCreate dataview)
        {
            throw new NotImplementedException();
        }

        public int CreateSupportsDetail(ViewDetails dataDto)
        {
            int data = 0;
            try
            {
                using (var conn = ConnectDB.BookingAdsDB())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        var sqlCreateSPd = @"
                    INSERT INTO tbDetailSupports
                    (SenderID, ReceiverID, TimeRep, Messeger, SupportsID, [Image])
                    VALUES
                    (@SenderID, @ReceiverID, GETDATE(), @Messeger, @SupportsID, @Images);
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";

                        var sqlUpdatesp = @"
                    UPDATE tbSupports
                    SET Status = 1
                    WHERE SupportsID = @SupportsID";

                        var parameters = new
                        {
                            SenderID = "admin",
                            ReceiverID = "admin",
                            Messeger = dataDto.Messenger,
                            SupportsID = dataDto.Supports.SupportsID,
                            Images = dataDto.Images,
                        };

                        var commandCreateSPd = new CommandDefinition(sqlCreateSPd, parameters, transaction, flags: CommandFlags.NoCache);
                        var commandUpdatesp = new CommandDefinition(sqlUpdatesp, parameters, transaction, flags: CommandFlags.NoCache);

                        var insertedId = conn.ExecuteScalar<int>(commandCreateSPd);
                        conn.Execute(commandUpdatesp);

                        if (insertedId <= 0)
                        {
                            transaction.Rollback();
                            return data = 0;
                        }

                        transaction.Commit();
                        return insertedId;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return 0;
            }
        }

        public Supports GetSupports(long supportsID)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetProducts = $@"SELECT   
                      o.SupportsID
					, o.Subject
					, o.[To]
					, o.TimeSend
					, o.Status
                    , o.SubjectMesseger
                    , o.ImageRequest
					, e.EmployeeID
                    , e.UserName
                    , e.FirstName
                    , e.LastName
                    , e.Gender
                    , e.Avatar
                    , e.LockedAt
                    , e.Coin
					, f.ProductID  
			        , f.ProductName
			        , f.Quantity
			        , f.Price
			        , f.IsLocked
			        , f.Photo 
                    , z.CatelogProductsID
                    , z.CatelogName                  
                    , ROW_NUMBER() OVER (ORDER BY o.TimeSend DESC) AS RowNum
                    FROM tbSupports o
					INNER JOIN dbo.tbEmployees e 
                    ON e.EmployeeID = o.EmployeeID
					INNER JOIN tbProducts AS f 
                    ON f.ProductID = o.ProductID
                    INNER JOIN tbCatelogProducts AS z 
                    ON f.CatelogProductsID = z.CatelogProductsID
                    WHERE SupportsID = @SupportsID";
                var parameters = new
                {
                    SupportsID = supportsID,
                };
                var data = conn.Query<Supports, Employee, Product, CatelogProduct, Supports>(sqlGetProducts, (supports, employee, product, catelogProduct) =>
                {
                    supports.Employee = employee;
                    supports.Service = product;
                    supports.CatelogProduct = catelogProduct;
                    return supports;
                }, param: parameters, splitOn: "EmployeeID, ProductID,CatelogProductsID").SingleOrDefault();

                return data;
            }
        }

        public IReadOnlyList<Supports> LoadSupport(ViewFilterSupports viewData)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {

                //UpdateStatus(customerId);
                var sqlLoadHistoryOrderProduct = $@"{GenHistoryOrderPaginateCTE(viewData)}

                    SELECT SupportsID
					, Subject
					, [To]
					, TimeSend
					, Status
                    , SubjectMesseger 
                    , ImageRequest
					, EmployeeID
                    , UserName
                    , FirstName
                    , LastName
                    , Gender
                    , Avatar
                    , LockedAt
                    , Coin
					, ProductID
			        , ProductName
			        , Quantity
			        , Price
			        , IsLocked
			        , Photo 
                    ,CatelogProductsID
                    , CatelogName
                    FROM HistoryOrderPaginateCTE 
                    WHERE (@Page = 1 AND @PageSize = 0) 
		                    OR RowNum BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize)";

                var dateStart = DateTimeUtils.ConvertToDateTimeSQL(viewData.DateStart);
                var dateEnd = DateTimeUtils.ConvertToDateTimeSQL(viewData.DateEnd);

                var parameters = new
                {
                    Status = viewData.OrderStatus,
                    DateStart = dateStart, // datestart == default ? datetime.now : datestart,
                    DateEnd = dateEnd,
                    SearchValue = $"%{viewData.SearchValue}%",
                    //EmployeeID = customerId,
                    Page = viewData.Page,
                    PageSize = viewData.PageSize,
                };

                var data = conn.Query<Supports, Employee, Product, CatelogProduct, Supports>(sqlLoadHistoryOrderProduct, (supports, employee, product, catelogProduct) =>
                {
                    supports.Employee = employee;
                    supports.Service = product;
                    supports.CatelogProduct = catelogProduct;
                    return supports;
                }, param: parameters, splitOn: "EmployeeID,ProductID,CatelogProductsID")
                    .Distinct()
                    .ToList();

                return data;
            }
        }

        public IReadOnlyList<SupportsDetails> LoadSupportDetail(long supportsID)
        {
            using (var connBookingAds = ConnectDB.BookingAdsDB())
            {
                var customerID = GetEmployye(supportsID);

                var sqlLoadInformationBookingAds = $@"SELECT 
                    spd.DetailID
			        , spd.SenderID
			        , spd.TimeRep
			        , spd.Messeger
			        , spd.SupportsID
			        , spd.Image
                    FROM tbDetailSupports spd
					inner join tbSupports sp on sp.SupportsID = spd.SupportsID

                    WHERE spd.SupportsID = @SupportsID and sp.EmployeeID = @EmployeeID ";

                var param = new
                {
                    EmployeeID = customerID,
                    SupportsID = supportsID,
                };

                var command = new CommandDefinition(sqlLoadInformationBookingAds, parameters: param, flags: CommandFlags.NoCache);

                var data = connBookingAds.Query<SupportsDetails>(command).ToList();

                if (data == null)
                {
                    return null;
                }

                return data;
            }
        }

        private static string GenConditionHistoryProduct(ViewFilterSupports condition)
        {
            var sqlCondition = new StringBuilder();

            if (condition == null)
            {
                return sqlCondition.ToString();
            }

            sqlCondition.Append(" e.EmployeeID = e.EmployeeID ");

            if (condition.OrderStatus != OrderStatus.DEFAULT)
            {
                sqlCondition.Append(" AND (o.Status = @Status)");
            }

            if (!string.IsNullOrEmpty(condition.DateEnd) && !string.IsNullOrEmpty(condition.DateStart))
            {
                sqlCondition.Append("  AND (o.TimeSend BETWEEN @DateStart AND @DateEnd) ");
            }
            else if (string.IsNullOrEmpty(condition.DateEnd) && !string.IsNullOrEmpty(condition.DateStart))
            {
                sqlCondition.Append("  AND (o.TimeSend BETWEEN @DateStart AND GetDate()) ");
            }
            else if (!string.IsNullOrEmpty(condition.DateEnd) && string.IsNullOrEmpty(condition.DateStart))
            {
                sqlCondition.Append("  AND (o.TimeSend <= @DateEnd )");
            }

            if (!string.IsNullOrEmpty(condition.SearchValue))
            {
                sqlCondition.Append(" AND (f.ProductName COLLATE Vietnamese_CI_AI LIKE @SearchValue)");
            }

            return sqlCondition.ToString();
        }

        private static string GenHistoryOrderPaginateCTE(ViewFilterSupports condition)
        {
            var sqlHistoryOrderPaginateCTE = $@"WITH HistoryOrderPaginateCTE AS (
                    SELECT   o.SupportsID
					, o.Subject
					, o.[To]
					, o.TimeSend
					, o.Status
                    , o.SubjectMesseger
                    , o.ImageRequest
					, e.EmployeeID
                    , e.UserName
                    , e.FirstName
                    , e.LastName
                    , e.Gender
                    , e.Avatar
                    , e.LockedAt
                    , e.Coin
					, f.ProductID  
			        , f.ProductName
			        , f.Quantity
			        , f.Price
			        , f.IsLocked
			        , f.Photo 
                    , z.CatelogProductsID
                    , z.CatelogName                  
                    , ROW_NUMBER() OVER (ORDER BY o.TimeSend DESC) AS RowNum
                    FROM tbSupports o
					INNER JOIN dbo.tbEmployees e 
                    ON e.EmployeeID = o.EmployeeID
					INNER JOIN tbProducts AS f 
                    ON f.ProductID = o.ProductID
                    INNER JOIN tbCatelogProducts AS z 
                    ON f.CatelogProductsID = z.CatelogProductsID
                    WHERE {GenConditionHistoryProduct(condition)}
            )";

            return sqlHistoryOrderPaginateCTE;
        }

        private static int GetEmployye(long supportsID)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var getEmployeeID = @"SELECT   
                     
					 e.EmployeeID
                    
                    FROM tbSupports o
					INNER JOIN dbo.tbEmployees e 
                    ON e.EmployeeID = o.EmployeeID
					INNER JOIN tbProducts AS f 
                    ON f.ProductID = o.ProductID
                    INNER JOIN tbCatelogProducts AS z 
                    ON f.CatelogProductsID = z.CatelogProductsID
                    WHERE SupportsID = @SupportsID";
                var parametersContent = new
                {
                    SupportsID = supportsID,
                };

                var insertedContentId = conn.ExecuteScalar<int>(getEmployeeID, parametersContent);

                if (insertedContentId == 0)
                {
                    return 0;

                }

                return insertedContentId;
            }
        }
    }
}