using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Dapper;
using NetCoreWebAPIDapper.Dtos;
using NetCoreWebAPIDapper.Filters;
using System.ComponentModel.DataAnnotations;
using NetCoreWebAPIDapper.Data.Models;

namespace NetCoreWebAPIDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionController : ControllerBase
    {
        private readonly string _connectionString;
        public FunctionController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DbConnectionString");
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();

                var result = await conn.QueryAsync<Function>("Get_Function_All", null, null, null, CommandType.StoredProcedure);
                return Ok(result);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);
                var result = await conn.QueryAsync<Function>("Get_Function_ById", parameters, null, null, CommandType.StoredProcedure);
                return Ok(result.Single());
            }
        }
        [HttpGet("paging")]
        public async Task<IActionResult> GetPaging(string keyword, int pageIndex, int pageSize)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();

                var paramaters = new DynamicParameters();
                paramaters.Add("@keyword", keyword);
                paramaters.Add("@pageIndex", pageIndex);
                paramaters.Add("@pageSize", pageSize);
                paramaters.Add("@totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var result = await conn.QueryAsync<Function>("Get_Function_AllPaging", paramaters, null, null, CommandType.StoredProcedure);

                int totalRow = paramaters.Get<int>("@totalRow");

                var pagedResult = new PagedResult<Function>()
                {
                    Items = result.ToList(),
                    TotalRow = totalRow,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                return Ok(pagedResult);
            }
        }
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody] Function function)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();
                var paramaters = new DynamicParameters();
                paramaters.Add("@id", function.Id);
                paramaters.Add("@name", function.Name);
                paramaters.Add("@url", function.Url);
                paramaters.Add("@parentId", function.ParentId);
                paramaters.Add("@sortOrder", function.SortOrder);
                paramaters.Add("@cssClass", function.CssClass);
                paramaters.Add("@isActive", function.IsActive);
                await conn.ExecuteAsync("Create_Function", paramaters, null, null, CommandType.StoredProcedure);
                return Ok();
            }
        }
        // PUT: api/Function/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([Required]Guid id, [FromBody] Function function)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();
                var paramaters = new DynamicParameters();
                paramaters.Add("@id", function.Id);
                paramaters.Add("@name", function.Name);
                paramaters.Add("@url", function.Url);
                paramaters.Add("@parentId", function.ParentId);
                paramaters.Add("@sortOrder", function.SortOrder);
                paramaters.Add("@cssClass", function.CssClass);
                paramaters.Add("@isActive", function.IsActive);

                await conn.ExecuteAsync("Update_Function", paramaters, null, null, CommandType.StoredProcedure);
                return Ok();
            }
        }
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();
                var paramaters = new DynamicParameters();
                paramaters.Add("@id", id);
                await conn.ExecuteAsync("Delete_Function_ById", paramaters, null, null, CommandType.StoredProcedure);
                return Ok();
            }
        }
    }
}