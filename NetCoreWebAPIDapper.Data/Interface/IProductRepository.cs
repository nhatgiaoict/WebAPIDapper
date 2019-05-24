using NetCoreWebAPIDapper.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreWebAPIDapper.Data.Interface
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync(string culture);
        Task<Product> GetByIdAsync(Guid id, string culture);
        Task<Guid> AddAsync(Product product, string culture);
        Task UpdateAsync(Guid id, Product product, string culture);
        Task DeleteAsync(Guid id);
    }
}
