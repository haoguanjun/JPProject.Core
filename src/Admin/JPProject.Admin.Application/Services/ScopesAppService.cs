using JPProject.Admin.Application.Interfaces;
using JPProject.Admin.Domain.Interfaces;
using JPProject.Domain.Core.Bus;
using JPProject.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JPProject.Admin.Application.Services
{
    public class ScopesAppService : IScopesAppService
    {
        private IEventStoreRepository _eventStoreRepository;
        private readonly IIdentityResourceRepository _identityResourcesRepository;
        public IMediatorHandler Bus { get; set; }

        public ScopesAppService(
            IMediatorHandler bus,
            IEventStoreRepository eventStoreRepository,
            IIdentityResourceRepository identityResourcesRepository)
        {
            Bus = bus;
            _eventStoreRepository = eventStoreRepository;
            _identityResourcesRepository = identityResourcesRepository;
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<string>> GetScopes(string search)
        {
            var identityScopes = await _identityResourcesRepository.SearchScopes(search);
            var apiScopes = await _identityResourcesRepository.SearchScopes(search);
            identityScopes.AddRange(apiScopes);
            return identityScopes.OrderBy(a => a);
        }
    }
}