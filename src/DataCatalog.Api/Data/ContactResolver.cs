using AutoMapper;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Data.Model;
using Microsoft.Extensions.Options;

namespace DataCatalog.Api.Data
{
    public class ContactResolver : IValueResolver<Dataset, DatasetResponse, ContactInfo>
    {
        private readonly ContactInfo _contactInfo;
        
        public ContactResolver(IOptions<ContactInfo> contactInfo)
        {
            _contactInfo = contactInfo.Value;
        }
        
        public ContactInfo Resolve(Dataset source, DatasetResponse destination, ContactInfo destMember, ResolutionContext context)
        {
            return _contactInfo;
        }
    }
    
    public class DomainContactResolver : IValueResolver<Domain.Dataset, DatasetResponse, ContactInfo>
    {
        private readonly ContactInfo _contactInfo;
        
        public DomainContactResolver(IOptions<ContactInfo> contactInfo)
        {
            _contactInfo = contactInfo.Value;
        }
        
        public ContactInfo Resolve(Domain.Dataset source, DatasetResponse destination, ContactInfo destMember, ResolutionContext context)
        {
            return _contactInfo;
        }
    }
}