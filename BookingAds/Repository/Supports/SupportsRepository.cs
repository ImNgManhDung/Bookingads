using BookingAds.Repository.Supports.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingAds.Models.Supports;
using BookingAds.Modules;
using Dapper;
using System.Text;
using System.Web;

namespace BookingAds.Repository.Supports
{
    using BookingAds.Common.Repository.Account.Abstractions;
    using BookingAds.Common.Repository.Account;
    using BookingAds.Entities;
    using MimeKit;
    using System.Data;

    public class SupportsRepository : ISupportsRepository
    {
        private readonly IAccountRepository _acountRepository = new AccountRepository();

        public int CreateSupports(ViewCreate dataview, long employeeID)
        {
            int data = 0;
            try
            {
                using (var conn = ConnectDB.BookingAdsDB())
                {
                    var sqlGetproductID = @"Select f.ProductID from tbProducts as f 
					inner join tbOrders as o on f.ProductId = o.ProductId  
					inner join tbEmployees as e on o.EmployeeID = e.EmployeeID 
					where 
						o.OrderID = @OderID and o.EmployeeID = @EmployeeID ";

                    var parametersGetProductID = new
                    {
                        OderID = dataview.OderID,
                        EmployeeID = employeeID,
                      
                    };

                    var commandGetproductID = new CommandDefinition(sqlGetproductID, parameters: parametersGetProductID, flags: CommandFlags.NoCache);
                    int  getProductId = conn.ExecuteScalar<int>(commandGetproductID);

                    
                    if (getProductId <= 0 )
                    {
                        return data = 0;
                    }

                    var sqlOrderProduct = $@"   INSERT INTO tbSupports
                           (Subject
                           ,[To]
                           ,TimeSend
                           ,Status
                           ,ProductID
                           ,EmployeeID
                           ,SubjectMesseger
                           ,ImageRequest, 
						   OderID)
                     VALUES
                           (@Subject
                           ,@To
                           ,Getdate()
                           ,0
                           ,@ProductID
                           ,@EmployeeID
                           ,@SubjectMesseger
                           ,@ImageRequest
                           ,@OderID)
                    
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    var parameters = new
                    {
                        Subject = dataview.Subject,
                        To = dataview.To,
                        OderID = dataview.OderID,
                        EmployeeID = employeeID,
                        SubjectMesseger = dataview.SubjectMessener,
                        ImageRequest = dataview.Images,
                        ProductID = getProductId,
                    };

                    var commandOrderProduct = new CommandDefinition(sqlOrderProduct, parameters: parameters, flags: CommandFlags.NoCache);
                    data = conn.ExecuteScalar<int>(commandOrderProduct);

                    //var sqlCreateSP = $@"INSERT INTO tbOrders(
                    //    EmployeeID
                    //    , ProductID
                    //    , OrderedTime
                    //    , Status
                    //    , Type
                    //    , TotalMoney
                    //) VALUES(
                    //    @EmployeeID
                    //    , @ProductID
                    //    , GETDATE()
                    //    , @Status 
                    //    , @Type
                    //    , @TotalMoney
                    //) 
                    //SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    //var parameters1 = new
                    //{

                    //};

                    //var commandcreateDetails = new CommandDefinition(sqlOrderProduct, parameters: parameters1, flags: CommandFlags.NoCache);
                    //conn.ExecuteScalar(commandcreateDetails);




                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ER: {ex}");
            }

            return data;
        }



        public IReadOnlyList<Supports> LoadSupport(ViewFilterSupports viewData, long customerId)
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
                    , SubjectMesseger , ImageRequest
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
                    EmployeeID = customerId,
                    Page = viewData.Page,
                    PageSize = viewData.PageSize,
                };

                var data = conn.Query<Supports, Employee, Product, CatelogProduct, Supports>(sqlLoadHistoryOrderProduct, (supports, employee, product, catelogProduct) =>
                {
                    supports.Employee = employee;
                    supports.Service = product;
                    supports.CatelogProduct = catelogProduct;
                    return supports;
                }, param: parameters, splitOn: "EmployeeID, ProductID,CatelogProductsID")
                    .Distinct()
                    .ToList();

                return data;
            }
        }

        public int Count(ViewFilterSupports viewData, long customerId)
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
                    EmployeeID = customerId,
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

        public int CreateSupportsDetail(ViewDetails dataDto, long customerId)
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
                    SET Status = 0
                    WHERE SupportsID = @SupportsID";

                        var parameters = new
                        {
                            SenderID = customerId,
                            ReceiverID = customerId,
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

      

        public IReadOnlyList<SupportsDetails> LoadSupportDetail(long supportsID, long customerID)
        {
            using (var connBookingAds = ConnectDB.BookingAdsDB())
            {
                // Admin
                //     var sqlLoadInformationBookingAds = $@"SELECT DetailID 
                //, SenderID
                //, TimeRep
                //, Messeger
                //, SupportsID
                //, Image
                //         FROM tbDetailSupports 
                //         WHERE SupportsID = @SupportsID";
                //UpdateStatus( customerID);
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

        private static void UpdateStatus(long customerId)
        {
            using (var connBookingAds = ConnectDB.BookingAdsDB())
            {
                var sqlUpdateStatus = @"UPDATE tbSupports
                        SET Status = 2
                        WHERE EmployeeID = @EmployeeID
                        AND SupportsID IN (
                            SELECT d.SupportsID
                            FROM tbDetailSupports d
                            WHERE d.TimeRep >= DATEADD(DAY, -3, GETDATE()) -- Kiểm tra nếu TimeRep lớn hơn 3 ngày trước
                            GROUP BY d.SupportsID
                            HAVING MAX(d.TimeRep) > DATEADD(DAY, -3, GETDATE()) -- Kiểm tra nếu thời gian lớn nhất vượt quá 3 ngày
);";
                var param = new
                {
                    EmployeeID = customerId,
                };

                var command = new CommandDefinition(sqlUpdateStatus, parameters: param, flags: CommandFlags.NoCache);

                connBookingAds.ExecuteScalar(command);
            }
        }


        #region GenCondition


        private static string GenConditionHistoryProduct(ViewFilterSupports condition)
        {
            var sqlCondition = new StringBuilder();

            if (condition == null)
            {
                return sqlCondition.ToString();
            }

            // default value
            sqlCondition.Append(" e.EmployeeID = @EmployeeID ");

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
       
        #endregion
    }
}
