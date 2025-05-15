using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using BookingAds.Areas.Admin.Repository.Product;
using BookingAds.Areas.Admin.Repository.Product.Abstractions;
using BookingAds.Entities;
using BookingAds.Modules;
using BookingAds.Repository.InformationBookingAds.Abstractions;

namespace BookingAds.Repository.InfomationProduct
{
    public class OrderProductRepository : IOrderProductRepository
    {
        private readonly IProductRepository _orderProductRepository = new ProductRepository();

        public bool CheckCoin(int employeeId, int productId)
        {
            int data = 0;

            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlCheckCoin = $@"SELECT COUNT(*)  
                    FROM tbEmployees 
                    WHERE EmployeeID = @EmployeeID AND 
                    Coin >= (SELECT Price 
                        FROM tbProducts 
                        WHERE ProductID = @ProductID)";

                var parameters = new
                {
                    EmployeeID = employeeId,
                    ProductID = productId,
                };

                var commandCheckCoin = new CommandDefinition(sqlCheckCoin, parameters: parameters, flags: CommandFlags.NoCache);
                data = conn.QueryFirstOrDefault<int>(commandCheckCoin);
            }

            return data > 0;
        }

        public bool CheckProductIsLocked(int productID)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlCheckProductIsLocked = $@"SELECT COUNT(*)  
                    FROM tbProducts 
                    WHERE ProductID = @ProductID 
                    AND IsLocked = 1";

                var parameters = new
                {
                    ProductID = productID,
                };

                var commandCheckProductIsLocked = new CommandDefinition(sqlCheckProductIsLocked, parameters: parameters, flags: CommandFlags.NoCache);
                var data = conn.QueryFirstOrDefault<int>(commandCheckProductIsLocked);

                return data > 0;
            }
        }

        public void DeductionCoin(int employeeId, int productId)
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
                        var sqlDeductionCoin = @"UPDATE tbEmployees
                        SET Coin = Coin - ( SELECT Price 
                            FROM tbProducts 
                            WHERE ProductID  = @ProductID)
                        WHERE EmployeeID = @EmployeeID";

                        var parameters = new
                        {
                            EmployeeID = employeeId,
                            ProductID = productId,
                        };

                        var commandDeductionCoin = new CommandDefinition(sqlDeductionCoin, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        conn.Execute(commandDeductionCoin);

                        trans.Commit();
                    }
                    catch (SqlException ex)
                    {
                        trans.Rollback();
                        Console.WriteLine($"EX : {ex}");
                    }
                }
            }
        }

        public Order GetOrder(long orderId)
        {
            Order data = new Order();

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
                    OrderID = orderId,
                };

                data = conn.Query<Order, Employee, Product, Order>(
                    sqlGetOrder, (order, employee, product) =>
                    {
                        order.Employee = employee;
                        order.Product = product;
                        return order;
                    }, splitOn: "EmployeeID, ProductID", param: parameters)
                    .Distinct()
                    .SingleOrDefault();
            }

            return data;
        }

        public int OrderProduct(int employeeId, int productId, int payType)
        {
            int data = 0;
            try
            {
                using (var conn = ConnectDB.BookingAdsDB())
                {
                    var sqlOrderProduct = $@"INSERT INTO tbOrders(
                        EmployeeID
                        , ProductID
                        , OrderedTime
                        , Status
                        , Type
                        , TotalMoney
                    ) VALUES(
                        @EmployeeID
                        , @ProductID
                        , GETDATE()
                        , @Status 
                        , @Type
                        , @TotalMoney
                    ) 
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    var parameters = new
                    {
                        EmployeeID = employeeId,
                        ProductID = productId,
                        Status = OrderStatus.PENDING.Code,
                        Type = payType,
                        TotalMoney = _orderProductRepository.GetProduct(productId).Price,
                    };

                    var commandOrderProduct = new CommandDefinition(sqlOrderProduct, parameters: parameters, flags: CommandFlags.NoCache);
                    data = conn.ExecuteScalar<int>(commandOrderProduct);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ER: {ex}");
            }

            return data;
        }
    }
}