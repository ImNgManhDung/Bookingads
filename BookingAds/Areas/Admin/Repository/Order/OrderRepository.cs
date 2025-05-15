using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using BookingAds.Areas.Admin.Models.ManageOrder;
using BookingAds.Areas.Admin.Repository.Order.Abstractions;
using BookingAds.Modules;

namespace BookingAds.Areas.Admin.Repository.Order
{
    using BookingAds.Entities;
    using BookingAds.Areas.Admin.Models.ManageOrder;
    using MailKit.Search;
    using System.Runtime.InteropServices.ComTypes;

    public class OrderRepository : IOrderRepository
    {
        public int GetPhone(long orderId)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var getEmployeeID = @"SELECT 
                     e.Phone
                   
				    FROM tbOrders o
				    INNER JOIN tbEmployees e 
                    ON e.EmployeeID = o.EmployeeID
				    INNER JOIN tbProducts f 
                    ON f.ProductID = o.ProductID
				    WHERE OrderID = @OrderID";
                var parametersContent = new
                {
                    OrderID = orderId,
                };

                var insertedContentId = conn.ExecuteScalar<int>(getEmployeeID, parametersContent);

                if (insertedContentId == 0)
                {
                    return 0;

                }

                return insertedContentId;
            }
        }

        public bool AcceptGotProduct(long orderID, string linkget)
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
                        var sql = $@"UPDATE tbOrders 
				            SET Status = {OrderStatus.WAITING.Code}
                            , Textlink = @Textlink
				            WHERE OrderID = @OrderID";

                        var parameters = new
                        {
                            OrderID = orderID,
                            Textlink = linkget,
                        };

                        var command = new CommandDefinition(sql, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        result = conn.Execute(command);

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

        public bool AcceptNotPay(long orderID)
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
                        var sql = $@"UPDATE tbOrders 
					        SET Status = {OrderStatus.FAILED.Code}
					        WHERE OrderID = @OrderID";

                        var parameters = new
                        {
                            OrderID = orderID,
                        };

                        var command = new CommandDefinition(sql, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        result = conn.Execute(command);

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

        public bool AcceptPayed(long orderID)
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
                        var sql = $@"UPDATE tbOrders 
					        SET Status = {OrderStatus.SUCCESSED.Code}
					        WHERE OrderID = @OrderID";

                        var parameters = new
                        {
                            OrderID = orderID,
                        };

                        var command = new CommandDefinition(sql, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        result = conn.Execute(command);

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

        public bool Approve(long orderID)
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
                        var sql = $@"UPDATE tbOrders 
				            SET Status = {OrderStatus.WAITING.Code}
                           
				            WHERE OrderID = @OrderID";

                        var parameters = new
                        {
                            OrderID = orderID,

                        };

                        var command = new CommandDefinition(sql, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        result = conn.Execute(command);

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

        public int Count(ViewFilterOrder viewData)
        {
            int result = 0;

            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetCountOrderByFilter = $@"SELECT COUNT(*)
				    FROM tbOrders o
				    INNER JOIN tbEmployees e 
                    ON e.EmployeeID = o.EmployeeID
				    INNER JOIN tbProducts f 
                    ON f.ProductID = o.ProductID
				    WHERE {GenConditionOrder(viewData)}";

                var fromDateTime = DateTimeUtils.ConvertToDateTimeSQL(viewData.FromDatetime);
                var toDatetime = DateTimeUtils.ConvertToDateTimeSQL(viewData.ToDatetime);

                var parameters = new
                {
                    Status = viewData.Status,
                    FromDatetime = fromDateTime == null ? string.Empty : fromDateTime.ToString(),
                    ToDateTime = toDatetime == null ? string.Empty : toDatetime.ToString(),
                    SearchField = viewData.SearchField,
                    SearchValue = $"%{viewData.SearchValue}%",
                    Page = viewData.Page,
                    PageSize = viewData.PageSize,
                };

                var commandGetCountOrderByFilter = new CommandDefinition(sqlGetCountOrderByFilter, parameters: parameters, flags: CommandFlags.NoCache);
                result = conn.QuerySingleOrDefault<int>(commandGetCountOrderByFilter);
            }

            return result;
        }

        public bool Delete(long orderID)
        {
            int result = 0;

            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sql = $@"DELETE FROM tbOrders 
				    WHERE OrderID = @OrderID";

                var parameters = new
                {
                    OrderID = orderID,
                };

                var command = new CommandDefinition(sql, parameters: parameters, flags: CommandFlags.NoCache);
                result = conn.Execute(command);
            }

            return result == 1;
        }

        public bool DeleteAllOrdersAreRejectedOrCanceled()
        {
            int result = 0;

            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sql = $@"DELETE FROM tbOrders 
				    WHERE Status = {OrderStatus.REJECTED.Code} 
                    OR Status = {OrderStatus.CANCELED.Code}";

                var parameters = new
                {
                };

                var command = new CommandDefinition(sql, parameters: parameters, flags: CommandFlags.NoCache);
                result = conn.Execute(command);
            }

            return result == 1;
        }

        public Order GetOrder(long orderID)
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
				    WHERE OrderID = @OrderID";

                var parameters = new
                {
                    OrderID = orderID,
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

        public IReadOnlyList<Order> GetOrders(ViewFilterOrder viewData)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetOrders = $@"{GenOrderPaginateCTE(viewData)}
			        SELECT OrderID
					, OrderedTime
					, Status
					, Type
					, TotalMoney
					, EmployeeID
                    , UserName
                    , FirstName
                    , LastName
                    , Gender
                    , Avatar
                    , LockedAt
                    , Coin
                    , Phone
					, ProductID
			        , ProductName
			        , Quantity
			        , Price
			        , IsLocked
			        , Photo
                    ,CatelogProductsID
                    , CatelogName
			        FROM OrderPaginateCTE 
			        WHERE (@Page = 1 AND @PageSize = 0) 
					        OR RowNum BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize)";

                var fromDateTime = DateTimeUtils.ConvertToDateTimeSQL(viewData.FromDatetime);
                var toDatetime = DateTimeUtils.ConvertToDateTimeSQL(viewData.ToDatetime);

                var parameters = new
                {
                    Status = viewData.Status,
                    FromDatetime = fromDateTime == null ? string.Empty : fromDateTime.ToString(),
                    ToDateTime = toDatetime == null ? string.Empty : toDatetime.ToString(),
                    SearchField = viewData.SearchField,
                    SearchValue = $"%{viewData.SearchValue}%",
                    Page = viewData.Page,
                    PageSize = viewData.PageSize,
                };

                var orders = conn.Query<Order, Employee, Product, CatelogProduct, Order>(
                    sqlGetOrders, (order, employee, product, catelog) =>
                    {
                        order.Employee = employee;
                        order.Product = product;
                        order.CatelogProduct = catelog;
                        return order;
                    }, splitOn: "EmployeeID, ProductID, CatelogProductsID", param: parameters)
                    .Distinct()
                    .ToList();

                return orders;
            }
        }

        public bool Reject(long orderID)
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
                        var sql = $@"UPDATE tbOrders 
					        SET Status = {OrderStatus.REJECTED.Code}
					        WHERE OrderID = @OrderID";

                        var parameters = new
                        {
                            OrderID = orderID,
                        };

                        var command = new CommandDefinition(sql, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        result = conn.Execute(command);

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "<Pending>")]
        private static string GenOrderPaginateCTE(ViewFilterOrder condition)
        {
            var sqlOrderPaginateCTE = $@";WITH OrderPaginateCTE AS (
				    SELECT	o.OrderID
					, o.OrderedTime
					, o.Status
					, o.Type
					, o.TotalMoney
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
                    , ct.CatelogProductsID
                    , ct.CatelogName
					, ROW_NUMBER() OVER (ORDER BY o.OrderedTime DESC) AS RowNum
				    FROM tbOrders o
				    INNER JOIN tbEmployees e 
                    ON e.EmployeeID = o.EmployeeID
				    INNER JOIN tbProducts f 
                    ON f.ProductID = o.ProductID
                    INNER JOIN tbCatelogProducts ct 
                    ON f.CatelogProductsID = ct.CatelogProductsID

                    WHERE {GenConditionOrder(condition)} 
            )";

            return sqlOrderPaginateCTE;
        }

        private static string GenConditionOrder(ViewFilterOrder condition)
        {
            var sqlCondition = new StringBuilder();

            if (condition == null)
            {
                return sqlCondition.ToString();
            }

            // default value when have not condition other
            sqlCondition.Append(" o.OrderID > 0 ");

            // set status value
            if (condition.Status != OrderStatus.DEFAULT)
            {
                sqlCondition.Append(" AND o.Status = @Status ");
            }

            // set datetime value when have fromDateTime and toDateTime
            if (!string.IsNullOrEmpty(condition.FromDatetime) && !string.IsNullOrEmpty(condition.ToDatetime))
            {
                sqlCondition.Append(" AND o.OrderedTime BETWEEN @FromDatetime AND @ToDatetime ");
            }

            // set datetime value when have fromDateTime
            if (!string.IsNullOrEmpty(condition.FromDatetime) && string.IsNullOrEmpty(condition.ToDatetime))
            {
                sqlCondition.Append(" AND o.OrderedTime >= @FromDatetime ");
            }

            // set datetime value when have toDateTime
            if (string.IsNullOrEmpty(condition.FromDatetime) && !string.IsNullOrEmpty(condition.ToDatetime))
            {
                sqlCondition.Append(" AND o.OrderedTime <= @ToDatetime ");
            }

            // set search value via fullname of employee
            if (!string.IsNullOrEmpty(condition.SearchValue))
            {
                if (condition.SearchField == SearchField.EMPLOYEE_FULLNAME)
                {
                    sqlCondition.Append(" AND (e.FirstName COLLATE Vietnamese_CI_AI LIKE @SearchValue ");
                    sqlCondition.Append(" OR e.LastName COLLATE Vietnamese_CI_AI LIKE @SearchValue ");
                    sqlCondition.Append(" OR e.LastName + ' ' + e.FirstName COLLATE Vietnamese_CI_AI LIKE @SearchValue ");
                    sqlCondition.Append(" OR e.FirstName + ' ' + e.LastName COLLATE Vietnamese_CI_AI LIKE @SearchValue) ");
                }

                // set search value
                if (condition.SearchField == SearchField.FOOD_NAME)
                {
                    sqlCondition.Append(" AND f.ProductName COLLATE Vietnamese_CI_AI LIKE @SearchValue ");
                }
            }

            sqlCondition.Append(Environment.NewLine);
            return sqlCondition.ToString();
        }

        public IReadOnlyList<OderDetailMess> LoadDetailMess(long oderID)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var emid = GetEmployeeID(oderID);
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
                    EmployeeID = emid,
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

        public int CreateDetailMess(ViewDetailsMess dataDto)
        {
            try
            {
                using (var conn = ConnectDB.BookingAdsDB())
                {
                    var emid = GetEmployeeID(dataDto.OrderId);
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
                            SenderID = "admin",
                            ReceiverID = emid, // Replace with actual receiver ID
                            OrderID = dataDto.OrderId,
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
                            Image = dataDto.Images,
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

        private static int GetEmployeeID(long orderId)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var getEmployeeID = @"SELECT 
                     e.EmployeeID
                   
				    FROM tbOrders o
				    INNER JOIN tbEmployees e 
                    ON e.EmployeeID = o.EmployeeID
				    INNER JOIN tbProducts f 
                    ON f.ProductID = o.ProductID
				    WHERE OrderID = @OrderID";
                var parametersContent = new
                {
                    OrderID = orderId,
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
