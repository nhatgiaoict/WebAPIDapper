﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly string _connectionString;
        public UserController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _connectionString = configuration.GetConnectionString("DbConnectionString");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();
                var result = await conn.QueryAsync<AppUser>("Get_User_All", null, null, null, CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _userManager.FindByIdAsync(id));
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

                var result = await conn.QueryAsync<AppUser>("Get_User_AllPaging", paramaters, null, null, CommandType.StoredProcedure);

                int totalRow = paramaters.Get<int>("@totalRow");

                var pagedResult = new PagedResult<AppUser>()
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
        public async Task<IActionResult>Post([FromBody] AppUser user)
        {
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
                return Ok();
            return BadRequest();
        }

        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> Put([Required] Guid id, [FromBody] AppUser user)
        {
            user.Id = id;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok();
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return Ok();
            return BadRequest();
        }
    }
}