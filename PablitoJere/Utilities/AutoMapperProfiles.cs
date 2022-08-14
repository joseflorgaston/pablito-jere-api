using AutoMapper;
using PablitoJere.DTOs;
using PablitoJere.Entities;

namespace PablitoJere.Utilities
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Publication, PublicationDTO>();
            CreateMap<PublicationCreateDTO, Publication>();
            CreateMap<PublicationImage, PublicationImageDTO>();
            CreateMap<PublicationImageCreateDTO, PublicationImage>();
        }
        
    }
}
