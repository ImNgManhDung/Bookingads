using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using Dapper;
using BookingAds.Constants;
using BookingAds.Models.InformationBookingAds;
using BookingAds.Modules;
using BookingAds.Repository.InformationBookingAds.Abstractions;

namespace BookingAds.Repository.InformationBookingAds
{
    using BookingAds.Entities;
    using MailKit.Search;
    using Microsoft.AspNet.SignalR.Messaging;

    public class InformationBookingAdsRepository : IInformationBookingAdsRepository
    {
        public int Count(ViewFilterBookingAds viewData)
        {
            int result = 0;

            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetCountProductByFilter = $@"SELECT	COUNT(*)
	                FROM tbProducts f
	                WHERE {GenConditionProduct(viewData)}";

                var parameters = new
                {
                    SearchValue = $"%{viewData.SearchValue}%",
                    CatelogProductsID = viewData.CatelogProductsID,
                    Page = viewData.Page,
                    PageSize = viewData.PageSize,
                };
                var command = new CommandDefinition(sqlGetCountProductByFilter, parameters: parameters, flags: CommandFlags.NoCache);
                result = conn.QueryFirstOrDefault<int>(command);
            }

            return result;
        }

        public IReadOnlyList<ProductAttributes> GetProductAttributes(long productId)
        {
            IReadOnlyList<ProductAttributes> data = null;

            using (var connBookingAds = ConnectDB.BookingAdsDB())
            {
                var sqlGetProductAttributesr = $@"select 
                                                *
                                                from tbProductAttributes pa 
                                                inner join tbProducts as p on pa.ProductID = p.ProductID
                                                where pa.ProductID = @ProductID ";

                var parameters = new
                {
                    ProductID = productId,
                };

                var command = new CommandDefinition(sqlGetProductAttributesr, parameters: parameters, flags: CommandFlags.NoCache);

                data = connBookingAds.Query<ProductAttributes>(command).ToList();
                return data;
            }
        }

        public IReadOnlyList<Product> GetProducts(ViewFilterBookingAds viewData)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                string sqlGetProducts = $@"{GenProductPaginateCTE(viewData)}
                    SELECT ProductID
			        , ProductName
			        , Quantity
			        , Price
			        , IsLocked
			        , Photo
			        , CatelogProductsID
			        , CatelogName  
                    FROM ProductPaginateCTE 
                    WHERE (@Page = 1 AND @PageSize = 0) 
		                OR RowNum BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize) ";

                if (viewData.SortField != SortField.Default && viewData.SortType != SortTypeConstant.DEFAULT)
                {
                    var sortType = viewData.SortType == SortTypeConstant.ASC ? "ASC" : "DESC";
                    sqlGetProducts += $"ORDER BY {viewData.SortField} {sortType}";
                }

                var parameters = new
                {
                    SearchValue = $"%{viewData.SearchValue}%",
                    CatelogProductsID = viewData.CatelogProductsID,
                    Page = viewData.Page,
                    PageSize = viewData.PageSize,
                };

                var products = conn.Query<Product, CatelogProduct, Product>(sqlGetProducts, (product, catelogProduct) =>
                {
                    product.CatelogProduct = catelogProduct;
                    return product;
                }, splitOn: "CatelogProductsID", param: parameters)
                    .Distinct()
                    .ToList();

                return products;
            }
        }

        public IReadOnlyList<Product> LoadInfomationsProduct()
        {
            using (var connBookingAds = ConnectDB.BookingAdsDB())
            {
                var sqlLoadInformationBookingAds = $@"SELECT ProductID
			        , ProductName
			        , Quantity
			        , Price
			        , IsLocked
			        , Photo
			        , CatelogProductsID 
                    FROM tbProducts";

                var param = new
                {
                };

                var command = new CommandDefinition(sqlLoadInformationBookingAds, parameters: param, flags: CommandFlags.NoCache);

                var data = connBookingAds.Query<Product>(command).ToList();
                return data;
            }
        }

        public IReadOnlyList<Product> LoadInfomationsProduct(long id)
        {
            using (var connBookingAds = ConnectDB.BookingAdsDB())
            {
                var sqlLoadInformationBookingAds = $@"
	                SELECT	f.ProductID
			        , f.ProductName
			        , f.Quantity
			        , f.Price
			        , f.IsLocked
			        , f.Photo
			        , cf.CatelogProductsID
			        , cf.CatelogName
			        , ROW_NUMBER() OVER (ORDER BY ProductID) AS RowNum
	                FROM tbProducts f
	                INNER JOIN tbCatelogProducts cf 
                    ON cf.CatelogProductsID = f.CatelogProductsID
                    WHERE ProductID = @ProductID";

                var param = new
                {
                    ProductID = id,
                };
                var products = connBookingAds.Query<Product, CatelogProduct, Product>(sqlLoadInformationBookingAds, (product, catelogProduct) =>
                {
                    product.CatelogProduct = catelogProduct;
                    return product;
                }, splitOn: "CatelogProductsID", param: param)
                   .Distinct()
                   .ToList();
                return products;
            }
        }

        public bool Recharge(string employeeUserName, long money)
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
                        var sqlRecharge = @"UPDATE tbEmployees
                        SET Coin = COALESCE(Coin, 0) + @Money
                        WHERE UserName = @UserName";

                        var parameters = new
                        {
                            UserName = employeeUserName,
                            Money = money,
                        };

                        var command = new CommandDefinition(sqlRecharge, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
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

        public IReadOnlyList<ProductDescription> GetProductDescription(long productId)
        {
            IReadOnlyList<ProductDescription> data = null;

            using (var connBookingAds = ConnectDB.BookingAdsDB())
            {
                var sqlGetProductAttributesr = $@"select 
                                                DescriptionDetail
                                                from tbProductDescription pa 
                                                inner join tbProducts as p on pa.ProductID = p.ProductID
                                                where pa.ProductID = @ProductID ";

                var parameters = new
                {
                    ProductID = productId,
                };

                var command = new CommandDefinition(sqlGetProductAttributesr, parameters: parameters, flags: CommandFlags.NoCache);

                data = connBookingAds.Query<ProductDescription>(command).ToList();
                return data;
            }
        }

        #region GenCondition
        private static string GenProductPaginateCTE(ViewFilterBookingAds condition)
        {
            condition = condition ?? new ViewFilterBookingAds();

            var sqlProductPaginateCTE = $@";WITH ProductPaginateCTE AS (
	                SELECT	f.ProductID
			        , f.ProductName
			        , f.Quantity
			        , f.Price
			        , f.IsLocked
			        , f.Photo
			        , cf.CatelogProductsID
			        , cf.CatelogName
			        , ROW_NUMBER() OVER (ORDER BY ProductID) AS RowNum
	                FROM tbProducts f
	                INNER JOIN tbCatelogProducts cf 
                    ON cf.CatelogProductsID = f.CatelogProductsID
                    WHERE {GenConditionProduct(condition)}
            )";

            return sqlProductPaginateCTE;
        }

        private static string GenConditionProduct(ViewFilterBookingAds condition)
        {
            var sqlCondition = new StringBuilder();

            if (condition == null)
            {
                return sqlCondition.ToString();
            }

            // default value
            sqlCondition.Append(" f.IsLocked = 0 ");

            // set search value
            if (!string.IsNullOrEmpty(condition.SearchValue))
            {
                sqlCondition.Append(" AND f.ProductName COLLATE Vietnamese_CI_AI LIKE @SearchValue ");
            }

            // set catelogProductID value
            if (condition.CatelogProductsID != 0)
            {
                sqlCondition.Append(" AND f.CatelogProductsID = @CatelogProductsID ");
            }

            sqlCondition.Append(Environment.NewLine);
            return sqlCondition.ToString();
        }


        #endregion

    }
}