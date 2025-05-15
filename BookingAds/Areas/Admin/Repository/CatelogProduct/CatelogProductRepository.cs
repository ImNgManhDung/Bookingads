using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using BookingAds.Areas.Admin.Models.ManageCatelogProduct;
using BookingAds.Areas.Admin.Repository.CatelogProduct.Abstractions;
using BookingAds.Modules;

namespace BookingAds.Areas.Admin.Repository.CatelogProduct
{
    using BookingAds.Entities;

    public class CatelogProductRepository : ICatelogProductRepository
    {
        public IReadOnlyList<CatelogProduct> GetCatelogProducts()
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetCatelogProducts = $@"SELECT CatelogProductsID
                    , CatelogName
                    FROM tbCatelogProducts";
                var catelogProducts = conn.Query<CatelogProduct>(sqlGetCatelogProducts).ToList();

                return catelogProducts;
            }
        }

        public int Count(ViewFilterCatelogProduct viewData)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetCountCatelogProduct = @"SELECT COUNT(*)
	                FROM tbCatelogProducts 
	                WHERE ( (@SearchValue = N'') /* Default search */
                    OR CatelogName LIKE @SearchValue ) /* Search via CatelogName */";

                var param = new
                {
                    SearchValue = $"%{viewData.SearchValue}%",
                    Page = viewData.Page,
                    PageSize = viewData.PageSize,
                };

                var command = new CommandDefinition(sqlGetCountCatelogProduct, parameters: param, flags: CommandFlags.NoCache);

                var totalRow = conn.QuerySingleOrDefault<int>(command);

                return totalRow;
            }
        }

        public CatelogProduct GetCatelogProducts(long catelogID)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetCatelogProducts = $@"SELECT CatelogProductsID
                    , CatelogName 
                    FROM tbCatelogProducts 
                    WHERE CatelogProductsID = @CatelogID";

                var param = new
                {
                    CatelogID = catelogID,
                };

                var command = new CommandDefinition(sqlGetCatelogProducts, parameters: param, flags: CommandFlags.NoCache);
                var data = conn.QueryFirstOrDefault<CatelogProduct>(command);
                return data;
            }
        }

        public IReadOnlyList<CatelogProduct> LoadCatelogProducts(ViewFilterCatelogProduct viewData)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlLoadCatelogProducts = $@"{GenCatelogProductPaginateCTE()}
                    /* Call CTE */
				    SELECT CatelogProductsID 
                    , CatelogName 
                    FROM CatalogProductPaginateCTE 
                    WHERE (@Page = 1 AND @PageSize = 0) /* No Paginate */
		            OR RowNum BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize) /* Paginate via Page and PageSize */";

                var param = new
                {
                    SearchValue = $"%{viewData.SearchValue}%",
                    Page = viewData.Page,
                    PageSize = viewData.PageSize,
                };

                var command = new CommandDefinition(sqlLoadCatelogProducts, parameters: param, flags: CommandFlags.NoCache);

                var data = conn.Query<CatelogProduct>(command).ToList();
                return data;
            }
        }

        public bool UpdateCatelogProduct(ViewCreateCatelogProduct dataDto)
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
                        var updateProductSql = $@"UPDATE tbCatelogProducts
                        SET CatelogName = @CatelogName
                        WHERE CatelogProductsID = @CatelogProductsID";

                        var parameters = new
                        {
                            CatelogProductsID = dataDto.CatelogProductsID,
                            CatelogName = dataDto.CatelogName,
                        };

                        var command = new CommandDefinition(updateProductSql, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
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

        public bool CreateCatelogProduct(ViewCreateCatelogProduct dataDto)
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
                        var sqlInsertCatelogProduct = $@"INSERT INTO tbCatelogProducts(
                                CatelogName
                            ) VALUES (
                                @CatelogName
                            )";

                        var parameters = new
                        {
                            CatelogName = dataDto.CatelogName,
                        };

                        var command = new CommandDefinition(sqlInsertCatelogProduct, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);

                        conn.Execute(command);
                        trans.Commit();

                        return true;
                    }
                    catch (SqlException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool DeleteCatelogProduct(long catelogID)
        {
            int result = 0;

            using (var conn = ConnectDB.BookingAdsDB())
            {
                var insertProductSql = $@"DELETE FROM tbCatelogProducts 
                    WHERE CatelogProductsID = @CatelogProductsID";

                var parameters = new
                {
                    CatelogProductsID = catelogID,
                };

                var command = new CommandDefinition(insertProductSql, parameters: parameters, flags: CommandFlags.NoCache);
                result = conn.Execute(command);
            }

            return result == 1;
        }

        public bool CheckCreateCatelogProduct(string catalogName)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlCheckCreateCatelogProduct = $@"SELECT CatelogName
                    FROM tbCatelogProducts
                    WHERE CatelogName = @CatelogName";

                var parameters = new
                {
                    CatelogName = catalogName,
                };

                var command = new CommandDefinition(sqlCheckCreateCatelogProduct, parameters: parameters, flags: CommandFlags.NoCache);

                var catelogNameInfo = conn.QueryFirstOrDefault<CatelogProduct>(command);

                return catelogNameInfo != null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "<Pending>")]
        private static string GenCatelogProductPaginateCTE()
        {
            var sqlCatelogProductPaginateCTE = @";WITH CatalogProductPaginateCTE AS (
	                SELECT c.CatelogProductsID 
                    , c.CatelogName
                    , ROW_NUMBER() OVER (ORDER BY CatelogProductsID ) AS RowNum	
	                FROM tbCatelogProducts c	                                           
	                WHERE (((@SearchValue = N'') /* Default search */
                    OR (CatelogName COLLATE Vietnamese_CI_AI LIKE @SearchValue))) /* Search via CatelogName */
            )";

            return sqlCatelogProductPaginateCTE;
        }
    }
}