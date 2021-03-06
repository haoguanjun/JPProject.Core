using AspNetCore.IQueryable.Extensions;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPProject.Admin.Domain.Interfaces
{
    public interface IPersistedGrantRepository
    {
        Task<List<PersistedGrant>> Search(ICustomQueryable search);
        Task<int> Count(ICustomQueryable search);
        Task<PersistedGrant> GetGrant(string key);
        void Remove(PersistedGrant grant);
    }
}