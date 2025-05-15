using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using BookingAds.Areas.Admin.Models.ManageProduct;
using BookingAds.Areas.Admin.Repository.Product.Abstractions;
using BookingAds.Constants;
using BookingAds.Modules;

namespace BookingAds.Areas.Admin.Repository.Product
{
    using BookingAds.Entities;

    public class ProductRepository : IProductRepository
    {
        public bool CreateProduct(ViewCreateProduct dataDto)
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
                        var sqlCreateProduct = $@"INSERT INTO tbProducts(
                            ProductName
                            , Quantity
                            , Price
                            , CatelogProductsID
                            , Photo
                            , IsLocked
                        ) VALUES (
                            @ProductName
                            , @Quantity
                            , @Price
                            , @CatelogProductsID
                            , @Photo
                            , 0
                        )
                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                        var parameters = new
                        {
                            ProductName = dataDto.ProductName,
                            Quantity = dataDto.Quantity,
                            Price = dataDto.Price,
                            CatelogProductsID = dataDto.CatelogProductsID,
                            Photo = dataDto.Photo,
                        };

                        var commandCreateProduct = new CommandDefinition(sqlCreateProduct, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        result = conn.ExecuteScalar<int>(commandCreateProduct);

                        trans.Commit();
                    }
                    catch (SqlException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }

        public Product GetProduct(long productID)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetProduct = $@"SELECT f.ProductID
                    , f.ProductName
                    , f.Quantity
                    , f.Price
                    , f.IsLocked
                    , f.Photo
                    , cf.CatelogProductsID
                    , cf.CatelogName
                    FROM tbProducts f
                    INNER JOIN tbCatelogProducts cf 
                    ON cf.CatelogProductsID = f.CatelogProductsID
                    WHERE ProductID = @ProductID";

                var parameters = new
                {
                    ProductID = productID,
                };

                var data = conn.Query<Product, CatelogProduct, Product>(sql: sqlGetProduct, (product, catelogProduct) =>
                {
                    product.CatelogProduct = catelogProduct;
                    return product;
                }, splitOn: "CatelogProductsID", param: parameters)
                    .Distinct()
                    .SingleOrDefault();

                return data;
            }
        }

        public IReadOnlyList<Product> GetProducts(ViewFilterBookingAds viewData)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetProducts = $@"{GenProductPaginateCTE()}
                    /* Call CTE */
                    SELECT  ProductID
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

        public int Count(ViewFilterBookingAds viewData)
        {
            int result = 0;

            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetCountProductByFilter = $@"SELECT	COUNT(*)
	                FROM tbProducts f
	                WHERE (( (@SearchValue = N'') 
                    OR (f.ProductName LIKE @SearchValue) )
				    AND ( (@CatelogProductsID = 0) 
                    OR (f.CatelogProductsID = @CatelogProductsID) )) ";

                var parameters = new
                {
                    SearchValue = $"%{viewData.SearchValue}%",
                    CatelogProductsID = viewData.CatelogProductsID,
                };

                var commandGetCountProductByFilter = new CommandDefinition(sqlGetCountProductByFilter, parameters: parameters, flags: CommandFlags.NoCache);
                result = conn.QuerySingleOrDefault<int>(commandGetCountProductByFilter);
            }

            return result;
        }

        public bool RestoreProduct(long productID)
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
                        var sqlRestoreProduct = $@"UPDATE tbProducts
                        SET IsLocked = 0
                        WHERE ProductID = @ProductID";

                        var parameters = new
                        {
                            ProductID = productID,
                        };

                        var commandRestoreProduct = new CommandDefinition(sqlRestoreProduct, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        result = conn.Execute(commandRestoreProduct);

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

        public bool SoftDeleteProduct(long productID)
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
                        var sqlSoftDeleteProduct = $@"UPDATE tbProducts
                        SET IsLocked = 1
                        WHERE ProductID = @ProductID";

                        var parameters = new
                        {
                            ProductID = productID,
                        };

                        var commandSoftDeleteProduct = new CommandDefinition(sqlSoftDeleteProduct, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        result = conn.Execute(commandSoftDeleteProduct);

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

        public bool UpdateProduct(ViewCreateProduct dataDto)
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
                        var sqlUpdateProduct = $@"UPDATE tbProducts
                        SET ProductName      = @ProductName,
                            Quantity      = @Quantity,
                            Price         = @Price,
                            CatelogProductsID = @CatelogProductsID,
                            Photo         = @Photo
                        WHERE ProductID = @ProductID";

                        var parameters = new
                        {
                            ProductName = dataDto.ProductName,
                            Quantity = dataDto.Quantity,
                            Price = dataDto.Price,
                            CatelogProductsID = dataDto.CatelogProductsID,
                            Photo = dataDto.Photo,
                            ProductID = dataDto.ProductID,
                        };

                        var commandUpdateProduct = new CommandDefinition(sqlUpdateProduct, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        result = conn.Execute(commandUpdateProduct);

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
        private static string GenProductPaginateCTE()
        {
            var sqlProductPaginateCTE = @";WITH ProductPaginateCTE AS (
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
	                WHERE (( (@SearchValue = N'') /* Search via default value */
                    OR (f.ProductName COLLATE Vietnamese_CI_AI LIKE @SearchValue) ) /* Search via ProductName */
				    AND ( (@CatelogProductsID = 0) /* Filter via default value */
                    OR (f.CatelogProductsID = @CatelogProductsID) )) /* Filter via CatelogProductsID */
            )";

            return sqlProductPaginateCTE;
        }
    }
}