using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using BookingAds.Areas.Admin.Repository.Product;
using BookingAds.Areas.Admin.Repository.Product.Abstractions;
using BookingAds.Entities;
using BookingAds.Models.HistoryOrderProduct;
using BookingAds.Modules;
using BookingAds.Repository.HistoryOrderProduct.Abstractions;
using MailKit.Search;

namespace BookingAds.Repository.HistoryOrderProduct
{
    public class HistoryOrderProductRepository : IHistoryOrderProductRepository
    {
        private readonly IProductRepository _orderProductRepository = new ProductRepository();

        public IReadOnlyList<Order> LoadHistoryOrderProduct(ViewFilterHistory viewData, long employeeId)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlLoadHistoryOrderProduct = $@"{GenHistoryOrderPaginateCTE(viewData)}

                    SELECT OrderID
					, OrderedTime
					, Status
					, Type
					, TotalMoney
                    , Textlink
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
                    FROM HistoryOrderPaginateCTE 
                    WHERE (@Page = 1 AND @PageSize = 0) 
		                    OR RowNum BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize)";

                var dateStart = DateTimeUtils.ConvertToDateTimeSQL(viewData.DateStart);
                var dateEnd = DateTimeUtils.ConvertToDateTimeSQL(viewData.DateEnd);

                var parameters = new
                {
                    Status = viewData.OrderStatus,
                    DateStart = dateStart, // DateStart == default ? DateTime.Now : DateStart,
                    DateEnd = dateEnd,
                    SearchValue = $"%{viewData.SearchValue}%",
                    EmployeeID = employeeId,
                    Page = viewData.Page,
                    PageSize = viewData.PageSize,
                };

                var data = conn.Query<Order, Employee, Product, Order>(sqlLoadHistoryOrderProduct, (order, employee, product) =>
                {
                    order.Employee = employee;
                    order.Product = product;
                    return order;
                }, param: parameters, splitOn: "EmployeeID, ProductID")
                    .Distinct()
                    .ToList();

                return data;
            }
        }

        public IReadOnlyList<Product> GetProducts(string searchValue)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetProducts = $@"SELECT ProductID
			        , ProductName
			        , Quantity
			        , Price
			        , IsLocked
			        , Photo 
                    FROM tbProducts
	                WHERE ProductName LIKE @SearchValue 
                    AND IsLocked = 0";
                var parameters = new
                {
                    SearchValue = $"%{searchValue}%",
                };
                var commandGetProducts = new CommandDefinition(sqlGetProducts, parameters: parameters, flags: CommandFlags.NoCache);

                var data = conn.Query<Product>(commandGetProducts)
                    .Distinct()
                    .ToList();

                return data;
            }
        }

        public int Count(ViewFilterHistory viewData, long employeeId)
        {
            int totalRow = 0;
            using (var conn = ConnectDB.BookingAdsDB())
            {
                string sqlGetCountHistoryFilter = $@"SELECT COUNT(*)
                    FROM tbOrders o
                    INNER JOIN dbo.tbEmployees e ON e.EmployeeID = o.EmployeeID 
                    INNER JOIN tbProducts AS f ON f.ProductID = o.ProductID
                    WHERE {GenConditionHistoryProduct(viewData)}";

                var dateStart = DateTimeUtils.ConvertToDateTimeSQL(viewData.DateStart);
                var dateEnd = DateTimeUtils.ConvertToDateTimeSQL(viewData.DateEnd);

                var parameters = new
                {
                    EmployeeID = employeeId,
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

        public bool EditOrder(long employeeId, long productId, long orderId)
        {
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
                        var sql = $@"UPDATE tbOrders 
                            SET ProductID = @ProductID,
                                TotalMoney = @TotalMoney
                            WHERE OrderID = @OrderID 
                            AND EmployeeID = @EmployeeID";

                        var parameters = new
                        {
                            EmployeeID = employeeId,
                            ProductID = productId,
                            OrderID = orderId,
                            TotalMoney = _orderProductRepository.GetProduct(productId).Price,
                        };

                        var command = new CommandDefinition(sql, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        conn.Execute(command);

                        trans.Commit();
                    }
                    catch (SqlException ex)
                    {
                        trans.Rollback();
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }

            return true;
        }

        public bool Cancel(long orderId)
        {
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
                        var sql = $@"UPDATE tbOrders 
					    SET Status = {OrderStatus.CANCELED.Code}
					    WHERE OrderID = @OrderID";

                        var parameters = new
                        {
                            OrderID = orderId,
                        };

                        var command = new CommandDefinition(sql, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        conn.Execute(command);

                        trans.Commit();
                    }
                    catch (SqlException ex)
                    {
                        trans.Rollback();
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }

            return true;
        }

        public bool UpdateCoin(long employeeID, decimal newBalance)
        {
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
                        var sqlUpdateCoin = @"UPDATE tbEmployees
                        SET Coin = @NewBalance
                        WHERE EmployeeID = @EmployeeID";

                        var parameters = new
                        {
                            EmployeeID = employeeID,
                            NewBalance = newBalance,
                        };

                        var command = new CommandDefinition(sqlUpdateCoin, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        var result = conn.Execute(command);

                        trans.Commit();

                        return result > 0;
                    }
                    catch (SqlException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        #region GenCondition
        private static string GenConditionHistoryProduct(ViewFilterHistory condition)
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
                sqlCondition.Append("  AND (o.OrderedTime BETWEEN @DateStart AND @DateEnd) ");
            }
            else if (string.IsNullOrEmpty(condition.DateEnd) && !string.IsNullOrEmpty(condition.DateStart))
            {
                sqlCondition.Append("  AND (o.OrderedTime BETWEEN @DateStart AND GetDate()) ");
            }
            else if (!string.IsNullOrEmpty(condition.DateEnd) && string.IsNullOrEmpty(condition.DateStart))
            {
                sqlCondition.Append("  AND (o.OrderedTime <= @DateEnd )");
            }

            if (!string.IsNullOrEmpty(condition.SearchValue))
            {
                sqlCondition.Append(" AND (f.ProductName COLLATE Vietnamese_CI_AI LIKE @SearchValue)");
            }

            return sqlCondition.ToString();
        }

        private static string GenHistoryOrderPaginateCTE(ViewFilterHistory condition)
        {
            var sqlHistoryOrderPaginateCTE = $@"WITH HistoryOrderPaginateCTE AS (
                    SELECT  o.OrderID
					, o.OrderedTime
					, o.Status
					, o.Type
					, o.TotalMoney
                    , o.Textlink
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
                    , ROW_NUMBER() OVER (ORDER BY o.OrderedTime DESC) AS RowNum
                    FROM tbOrders o
					INNER JOIN dbo.tbEmployees e 
                    ON e.EmployeeID = o.EmployeeID
					INNER JOIN tbProducts AS f 
                    ON f.ProductID = o.ProductID
                    WHERE {GenConditionHistoryProduct(condition)}
            )";

            return sqlHistoryOrderPaginateCTE;
        }

        public Order GetOrder(long orderID, long employeeID)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetOrder = $@"SELECT o.OrderID
					, o.OrderedTime
					, o.Status
					, o.Type
					, o.TotalMoney
                    , o.Textlink
                    , e.EmployeeID
                    , e.UserName
                    , e.FirstName
                    , e.LastName
                    , e.Gender
                    , e.Avatar
                    , e.LockedAt
                    , e.Coin
                    , e.Phone
                    , f.ProductID
			        , f.ProductName
			        , f.Quantity
			        , f.Price
			        , f.IsLocked
			        , f.Photo 
				    FROM tbOrders o
				    INNER JOIN tbEmployees e 
                    ON e.EmployeeID = o.EmployeeID
				    INNER JOIN tbProducts f 
                    ON f.ProductID = o.ProductID
				    WHERE OrderID = @OrderID AND e.EmployeeID = @EmployeeId";

                var parameters = new
                {
                    OrderID = orderID,
                    EmployeeId = employeeID,
                };

                var data = conn.Query<Order, Employee, Product, Order>(
                    sqlGetOrder, (order, employee, product) =>
                    {
                        order.Employee = employee;
                        order.Product = product;
                        return order;
                    }, splitOn: "EmployeeID, ProductID", param: parameters)
                    .Distinct()
                    .SingleOrDefault();

                return data;
            }
        }

        public IReadOnlyList<OderDetailMess> LoadDetailMess(long oderID, long employeeID)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetProducts = @"
            SELECT dbo.tbOrders.OrderID,  
                   dbo.tbOrders.EmployeeID,     
                   dbo.tbOderDetailMess.SenderID, 
                   dbo.tbOderDetailMess.CreatedTime, 
                   dbo.tbOderDetailMess.ReadTime, 
                   dbo.tbOderDetailMess.OderDetailMessID, 
                   dbo.tbOderDetailMess.OrderID AS Expr1, 
                   dbo.tbContents.Text, 
                   dbo.tbContents.Image
            FROM dbo.tbOderDetailMess 
            INNER JOIN dbo.tbContents 
                ON dbo.tbContents.OderDetailMessID = dbo.tbOderDetailMess.OderDetailMessID 
            INNER JOIN dbo.tbOrders 
                ON dbo.tbOderDetailMess.OrderID = dbo.tbOrders.OrderID
            WHERE dbo.tbOrders.OrderID = @OrderID 
              AND dbo.tbOrders.EmployeeID = @EmployeeID";

                var parameters = new
                {
                    OrderID = oderID,
                    EmployeeID = employeeID,
                };

                var data = conn.Query<OderDetailMess, Content, OderDetailMess>(
                    sqlGetProducts,
                    (orderdetails, content) =>
                    {
                        orderdetails.Content = content;
                        return orderdetails;
                    },
                    splitOn: "OderDetailMessID", // Chỉ định cột chia tách là OderDetailMessID
                    param: parameters)
                    .Distinct()
                    .ToList();

                return data.AsReadOnly();
            }
        }

        public int CreateDetailMess(ViewDetailsMess dataDto, long customerId)
        {
            try
            {
                using (var conn = ConnectDB.BookingAdsDB())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        // Insert into tbOderDetailMess
                        var sqlCreateOdm = @"
                    INSERT INTO [dbo].[tbOderDetailMess]
                        ([SenderID]
                        ,[ReceiverID]
                        ,[ContentMessID]
                        ,[CreatedTime]
                        ,[ReadTime]
                        ,[OrderID])
                    VALUES
                        (@SenderID, @ReceiverID, '1', GETDATE(), GETDATE(), @OrderID);
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";

                        var parametersOdm = new
                        {
                            SenderID = customerId,
                            ReceiverID = "admin", // Replace with actual receiver ID
                            OrderID = dataDto.OrderId
                        };

                        var insertedId = conn.ExecuteScalar<int>(sqlCreateOdm, parametersOdm, transaction);

                        // Insert into tbContents
                        var sqlCreateContent = @"
                    INSERT INTO [dbo].[tbContents]
                        ([Text]
                        ,[OderDetailMessID]
                        ,[Image])
                    VALUES
                        (@Text, @OderDetailMessID, @Image);
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";

                        var parametersContent = new
                        {
                            Text = dataDto.Messenger,
                            OderDetailMessID = insertedId,
                            Image = dataDto.Images
                        };

                        var insertedContentId = conn.ExecuteScalar<int>(sqlCreateContent, parametersContent, transaction);

                        var i = insertedContentId;

                        // Commit or Rollback transaction based on results
                        if (insertedId > 0 && insertedContentId > 0)
                        {
                            transaction.Commit();
                            return insertedContentId;
                        }
                        else
                        {
                            transaction.Rollback();
                            return 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return 0;
            }
        }


        #endregion
    }
}