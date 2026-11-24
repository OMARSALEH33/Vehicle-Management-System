using AutoMapper;
using Challenge_1_Build_APIS.Controllers.Resources;
using Challenge_1_Build_APIS.Core.Models;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;


namespace Challenge_1_Build_APIS.Mapping;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Make, MakeResource>();
        CreateMap<Make, KeyValuePairResource>();
        CreateMap<SaveMakeResource, Make>();

        CreateMap<Model, ModelResource>();
        CreateMap<Model, KeyValuePairResource>();
        CreateMap<SaveModelResource, Model>();
        CreateMap<UpdateModelResource, Model>();

        CreateMap<Feature, FeatureResource>();
        CreateMap<SaveFeatureResource, Feature>();

        CreateMap<Vehicle, VehicleResource>()
            .ForMember(vr => vr.Make, opt => opt.MapFrom(v => v.Model.Make))
            .ForMember(vr => vr.Features, opt => opt.MapFrom(v => v.Features.Select(vf => new KeyValuePairResource { Id = vf.Feature.Id, Name = vf.Feature.Name })))
            .ForMember(vr => vr.Contact, opt => opt.MapFrom(v => new ContactResource
             {
                 Name = v.ContactName,
                 Email = v.ContactEmail,
                 Phone = v.ContactPhone
             }));

        CreateMap<SaveVehicleResource, Vehicle>()
            .ForMember(v => v.Id, opt => opt.Ignore())
            .ForMember(v => v.ContactName, opt => opt.MapFrom(vr => vr.Contact.Name))
            .ForMember(v => v.ContactEmail, opt => opt.MapFrom(vr => vr.Contact.Email))
            .ForMember(v => v.ContactPhone, opt => opt.MapFrom(vr => vr.Contact.Phone))
            .ForMember(v => v.Features, opt => opt.Ignore());
    }
}