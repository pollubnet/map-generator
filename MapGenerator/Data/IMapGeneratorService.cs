namespace MapGenerator.Data;

public interface IMapGeneratorService
{
    Task<MapData> GetMap(bool generateNew = false);
}
