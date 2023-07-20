using AutoMapper;
using PubAPI.Interfaces;

namespace PubAPI.Helpers;

public class MappingService : IMappingService
{
    private readonly IMapper _mapper;

    public MappingService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public TDestination MapEntityToDto<TEntity, TDestination>(TEntity entity)
    {
        return _mapper.Map<TDestination>(entity);
    }
    public IEnumerable<TDestination> MapEntityListToDtoList<TEntity, TDestination>(List<TEntity> entities)
    {
        return _mapper.Map<List<TDestination>>(entities);
    }
}
