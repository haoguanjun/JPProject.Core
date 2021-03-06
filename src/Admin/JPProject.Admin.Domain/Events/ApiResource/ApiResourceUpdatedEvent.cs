using System;
using JPProject.Admin.Domain.Commands.ApiResource;
using JPProject.Domain.Core.Events;

namespace JPProject.Admin.Domain.Events.ApiResource
{
    public class ApiResourceUpdatedEvent : Event
    {
        public IdentityServer4.Models.ApiResource ApiResource { get; }

        public ApiResourceUpdatedEvent(IdentityServer4.Models.ApiResource api)
        {
            ApiResource = api;
            AggregateId = api.Name;
        }
    }
}