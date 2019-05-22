using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NetCoreWebAPIDapper.Models;
using NetCoreWebAPIDapper.Extensions;
using NetCoreWebAPIDapper.Filters;
using NetCoreWebAPIDapper.Resources;
using Serilog;

namespace NetCoreWebAPIDapper.Controllers
{
    [Route("api/{culture}/[controller]")]
    [ApiController]
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    public class ProductController : ControllerBase
    {
        // GET: api/Product
        private readonly ILogger<ProductController> _logger;
        private readonly IStringLocalizer<ProductController> _localizer;
        private readonly LocService _localService;
        private readonly string _connectionString;
        public ProductController(IConfiguration configuration, ILogger<ProductController> logger,
                                IStringLocalizer<ProductController> localizer,
                                LocService localService)
        {
            _connectionString = configuration.GetConnectionString("DbConnectionString");
            _logger = logger;
            _localizer = localizer;
            _localService = localService;
        }
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            //var text = _localizer["Test"];
            //var text1 = _localService.GetLocalizedHtmlString("ForgotPassword");
            //_logger.LogError("Test Log");
            using(var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                var paramates = new DynamicParameters();
                paramates.Add("@language", CultureInfo.CurrentCulture.Name);
                var result = await conn.QueryAsync<Product>("Get_Product_All", paramates, null, null, CommandType.StoredProcedure);
                return result;
            }
        }

        // GET: api/Product/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<Product> Get(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                var paramaters = new DynamicParameters();
                paramaters.Add("@id", id);
                paramaters.Add("@language", CultureInfo.CurrentCulture.Name);

                var result = await conn.QueryAsync<Product>("Get_Product_ById", paramaters, null, null, CommandType.StoredProcedure);
                return result.Single();
            }
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
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
                paramaters.Add("@language", CultureInfo.CurrentCulture.Name);
                paramaters.Add("@categoryIds", product.CategoryIds);
                paramaters.Add("@id", id);
                var result = await conn.ExecuteAsync("Create_Product", paramaters, null, null, CommandType.StoredProcedure);

                return Ok(id);
            }

        }

        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> Put(Guid id, [FromBody] Product product)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
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
                paramaters.Add("@language", CultureInfo.CurrentCulture.Name);
                paramaters.Add("@categoryIds", product.CategoryIds);
                await conn.ExecuteAsync("Update_Product", paramaters, null, null, CommandType.StoredProcedure);
                return Ok();
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                var paramaters = new DynamicParameters();
                paramaters.Add("@id", id);
                await conn.ExecuteAsync("Delete_Product_ById", paramaters, null, null, CommandType.StoredProcedure);
            }
        }
    }
}
