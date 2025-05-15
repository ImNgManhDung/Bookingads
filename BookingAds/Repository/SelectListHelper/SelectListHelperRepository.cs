using BookingAds.Entities;
using BookingAds.Modules;
using BookingAds.Repository.SelectListHelper.Abstractions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Repository.SelectListHelper
{
    public static class SelectListHelperRepository 
    {
        public static IReadOnlyList<Order> ListServices_Customer(long customerID)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlLoadHistoryOrderProduct = $@"
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
                                         , c.CatelogProductsID
		                                 , c.CatelogName
		                                 , ROW_NUMBER() OVER (ORDER BY o.OrderedTime DESC) AS RowNum
		                                 FROM tbOrders o
			                                INNER JOIN dbo.tbEmployees e 
		                                 ON e.EmployeeID = o.EmployeeID
			                                INNER JOIN tbProducts AS f 
		                                 ON f.ProductID = o.ProductID

 			                                INNER JOIN tbCatelogProducts AS c 
		                                 ON f.CatelogProductsID = c.CatelogProductsID
                                         WHERE o.EmployeeID = @CustomerID
                                        ";
                var parameters = new
                {
                    CustomerID = customerID,
                };

                var data = conn.Query<Order, Employee, Product, CatelogProduct, Order>(sqlLoadHistoryOrderProduct, (order, employee, product,catelogProduct) =>
                {
                    order.Employee = employee;
                    order.Product = product;
                    order.CatelogProduct = catelogProduct;
                    return order;
                }, param: parameters, splitOn: "EmployeeID, ProductID,CatelogProductsID")
                   .Distinct()
                   .ToList();

                return data;
            }
        }
    }
}