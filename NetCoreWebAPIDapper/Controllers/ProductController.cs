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
using NetCoreWebAPIDapper.Data.Interface;
using NetCoreWebAPIDapper.Data.Models;
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
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductController> _logger;
        private readonly IStringLocalizer<ProductController> _localizer;
        private readonly LocService _localService;
        private readonly string _connectionString;
        public ProductController(IConfiguration configuration, ILogger<ProductController> logger,
                                IStringLocalizer<ProductController> localizer,
                                LocService localService,
                                IProductRepository productRepository)
        {
            _connectionString = configuration.GetConnectionString("DbConnectionString");
            _logger = logger;
            _localizer = localizer;
            _localService = localService;
            _productRepository = productRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //var text = _localizer["Test"];
            //var text1 = _localService.GetLocalizedHtmlString("ForgotPassword");
            //_logger.LogError("Test Log");
            var result = await _productRepository.GetAllAsync(CultureInfo.CurrentCulture.Name);
            return Ok(result);
        }

        // GET: api/Product/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await _productRepository.GetByIdAsync(id, CultureInfo.CurrentCulture.Name));
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            var id = await _productRepository.AddAsync(product, CultureInfo.CurrentCulture.Name);
            return Ok(id);
        }

        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> Put(Guid id, [FromBody] Product product)
        {
            await _productRepository.UpdateAsync(id, product, CultureInfo.CurrentCulture.Name);
            return Ok();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productRepository.DeleteAsync(id);
            return Ok();
        }
    }
}
