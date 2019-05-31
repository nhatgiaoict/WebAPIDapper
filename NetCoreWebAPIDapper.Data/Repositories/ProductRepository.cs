using Dapper;
using Microsoft.Extensions.Configuration;
using NetCoreWebAPIDapper.Data.Interface;
using NetCoreWebAPIDapper.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreWebAPIDapper.Data.Repositories
{
    
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;
        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DbConnectionString");
        }
        public async Task<Guid> AddAsync(Product product, string culture)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();
                var paramaters = new DynamicParameters();
                var id = Guid.NewGuid();
                paramaters.Add("@name", product.Name);
                paramaters.Add("@description", product.Description);
                paramaters.Add("@content", product.Content);
                paramaters.Add("@seoDescription", product.SeoDescription);
                paramaters.Add("@seoAlias", product.SeoAlias);
                paramaters.Add("@seoTitle", product.SeoTitle);
                paramaters.Add("@seoKeyword", product.SeoKeyword);
                paramaters.Add("@sku", product.Sku);
                paramaters.Add("@price", product.Price);
                paramaters.Add("@discountPrice", product.DiscountPrice);
                paramaters.Add("@isActive", product.IsActive);
                paramaters.Add("@imageUrl", product.ImageUrl);
                paramaters.Add("@language", culture);
                paramaters.Add("@categoryIds", product.CategoryIds);
                paramaters.Add("@id", id);
                var result = await conn.ExecuteAsync("Create_Product", paramaters, null, null, CommandType.StoredProcedure);

                return id;
            }

        }

        public async Task DeleteAsync(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();
                var paramaters = new DynamicParameters();
                paramaters.Add("@id", id);
                await conn.ExecuteAsync("Delete_Product_ById", paramaters, null, null, CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync(string culture)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();
                var paramates = new DynamicParameters();
                paramates.Add("@language", culture);
                var result = await conn.QueryAsync<Product>("Get_Product_All", paramates, null, null, CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<Product> GetByIdAsync(Guid id, string culture)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();
                var paramaters = new DynamicParameters();
                paramaters.Add("@id", id);
                paramaters.Add("@language", culture);

                var result = await conn.QueryAsync<Product>("Get_Product_ById", paramaters, null, null, CommandType.StoredProcedure);
                return result.Single();
            }
        }

        public async Task UpdateAsync(Guid id, Product product, string culture)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();
                var paramaters = new DynamicParameters();
                paramaters.Add("@id", id);
                paramaters.Add("@name", product.Name);
                paramaters.Add("@description", product.Description);
                paramaters.Add("@content", product.Content);
                paramaters.Add("@seoDescription", product.SeoDescription);
                paramaters.Add("@seoAlias", product.SeoAlias);
                paramaters.Add("@seoTitle", product.SeoTitle);
                paramaters.Add("@seoKeyword", product.SeoKeyword);
                paramaters.Add("@sku", product.Sku);
                paramaters.Add("@price", product.Price);
                paramaters.Add("@discountPrice", product.DiscountPrice);
                paramaters.Add("@isActive", product.IsActive);
                paramaters.Add("@imageUrl", product.ImageUrl);
                paramaters.Add("@language", culture);
                paramaters.Add("@categoryIds", product.CategoryIds);
                await conn.ExecuteAsync("Update_Product", paramaters, null, null, CommandType.StoredProcedure);
            }
        }
    }
}
