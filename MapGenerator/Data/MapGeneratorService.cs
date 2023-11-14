using MapGenerator.NoiseGenerators;
using System.Drawing;
using System.Numerics;

namespace MapGenerator.Data;

public class MapGeneratorService : IMapGeneratorService
{
    private MapData map;

    //private Dictionary<(TemperatureType, HeightType), Biome> biomesDictionary = new Dictionary<(TemperatureType, HeightType), Biome>
    //{
    //    [(TemperatureType.Cold, HeightType.Medium)] = new Biome(BiomeType.ShallowWater, Color.Blue, 1f, 0.3f)
    //};

    //private Biome[] biomes = {
    //     new Biome(BiomeType.ShallowWater, Color.Blue, 1f, 0.3f),
    //     new Biome(BiomeType.Grassland, Color.Green, 1f, 0.5f),
    //     new Biome(BiomeType.Desert, Color.Yellow, 1f, 0.5f),
    //     new Biome(BiomeType.Mountains, Color.Gray, 0f, 0.5f)
    // };

    private int seed;

    public Task<MapData> GetMap(bool generateNew = false)
    {
        if (map == null || generateNew)
        {
            map = GenerateMap();
            seed++;
        }
        return Task.FromResult(map);
    }

    private MapData GenerateMap()
    {
        float scale = 15f;
        int octaves = 5;
        float persistance = 0.5f;
        float lacunarity = 2f;
        int width = 48;
        int height = 48;
        bool useFalloff = true;

        Node[,] grid = new Node[width, height];
        float[,] falloffMap = FalloffGenerator.Generate(width);
        float[,] heightMap = NoiseGenerator.Generate(width, height, seed, scale, octaves, persistance, lacunarity, Vector2.Zero);
        float[,] temperatureMap = NoiseGenerator.Generate(width, height, seed + 1, scale, octaves, persistance, lacunarity, Vector2.Zero);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (useFalloff)
                {
                    heightMap[x, y] = Math.Clamp(heightMap[x, y] - falloffMap[x, y], 0, 1);
                }

                grid[x, y] = new Node(x, y, null);
                grid[x, y].NoiseValue = heightMap[x, y];

                if (heightMap[x, y] < 0.02f)
                {
                    grid[x, y].Color = "#21618C";
                }
                else if (heightMap[x, y] < 0.2f)
                {
                    grid[x, y].Color = "#2E86C1";
                }
                else if (heightMap[x, y] < 0.40f)
                {
                    grid[x, y].Color = "#F9E79F";
                }
                else if (heightMap[x, y] < 0.55f)
                {
                    grid[x, y].Color = "#28B463";
                }
                else if (heightMap[x, y] < 0.7f)
                {
                    grid[x, y].Color = "#1D8348";
                }
                else if (heightMap[x, y] < 0.85f)
                {
                    grid[x, y].Color = "#616A6B";
                }
                else
                {
                    grid[x, y].Color = "#515A5A";
                }
            }
        }

        MapData map = new MapData(grid);
        return map;
    }
}