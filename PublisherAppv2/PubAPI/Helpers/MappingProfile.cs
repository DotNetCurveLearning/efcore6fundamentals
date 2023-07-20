using AutoMapper;
using PubAPI.Dtos;
using PublisherDomain;

namespace PubAPI.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Author, AuthorDto>();
    }
}
