using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetCoreWebAPIDapper.Dtos;
using NetCoreWebAPIDapper.Filters;
using NetCoreWebAPIDapper.Models;

namespace NetCoreWebAPIDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly string _connectionString;
        public RoleController(RoleManager<AppRole> roleManager, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _connectionString = configuration.GetConnectionString("DbConnectionString");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                var result = await conn.QueryAsync<AppRole>("Get_Role_All", null, null, null, CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _roleManager.FindByIdAsync(id));
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetPaging(string keyword, int pageIndex = 0, int pageSize = 10)
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

                var result = await conn.QueryAsync<AppRole>("Get_Role_AllPaging", paramaters, null, null, CommandType.StoredProcedure);

                int totalRow = paramaters.Get<int>("@totalRow");

                var pagedResult = new PagedResult<AppRole>()
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
        public async Task<IActionResult> Post([FromBody] AppRole role)
        {
            var result = await _roleManager.CreateAsync(role);
            if(result)

        }
    }
}