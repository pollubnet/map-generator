namespace MapGenerator.MapGenerators.Data;

public class Node
{
    public int XId { get; set; }
    public int YId { get; set; }
    public Biome Biome { get; set; }

    public string Color { get; set; }
    public float NoiseValue { get; set; }
    public float Cos { get; set; }

    public Node(int xId, int yId, Biome biome)
    {
        XId = xId;
        YId = yId;
        Biome = biome;
    }
}
