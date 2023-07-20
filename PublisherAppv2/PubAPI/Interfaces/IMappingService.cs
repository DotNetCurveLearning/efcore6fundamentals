namespace PubAPI.Interfaces;

public interface IMappingService
{
    TDestination MapEntityToDto<TEntity, TDestination>(TEntity entity);
    IEnumerable<TDestination> MapEntityListToDtoList<TEntity, TDestination>(List<TEntity> entities);
}
