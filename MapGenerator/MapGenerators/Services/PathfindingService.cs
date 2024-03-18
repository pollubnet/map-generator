using MapGenerator.MapGenerators.Data;

namespace MapGenerator.MapGenerators.Services
{
    public class PathfindingNode
    {
        private Random random = new Random();
        private readonly Node node;

        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost => GCost + HCost;
        public bool Walkable { get; private set; }
        public PathfindingNode? PreviousNode { get; set; }

        public int XId => node.XId;
        public int YId => node.YId;
        public Biome Biome => node.Biome;

        public PathfindingNode(Node node)
        {
            this.node = node;
            GCost = 0;
            HCost = 0;
            PreviousNode = null;
            Walkable = random.NextDouble() > 0.5; // TODO, when our mapGenetator will be returning some BiomeTypes, it'll has to change to determine walkability chcecking the types of a specific biome
        }
    }
    public class PathfindingService : IPathfindingService<PathfindingNode>
    {
        public PathfindingNode[,] PathfindingNodeMap { get; set; }
        private int mapRows;
        private int mapColumns;

        private readonly MapGeneratorService mapGeneratorService;

        public PathfindingService(MapData mapData)
        {
            Node[,] nodeMap = mapData.Grid;

            CreatePathfindingNodeMap(nodeMap);
        }

        private void CreatePathfindingNodeMap(Node[,] nodeMap)
        {
            mapRows = nodeMap.GetLength(0);
            mapColumns = nodeMap.GetLength(1);
            PathfindingNodeMap = new PathfindingNode[mapRows, mapColumns];

            for (int i = 0; i < mapRows; i++)
            {
                for (int j = 0; j < mapColumns; j++)
                {
                    PathfindingNodeMap[i, j] = new PathfindingNode(nodeMap[i, j]);
                }
            }
        }

        public List<PathfindingNode> FindPath(PathfindingNode start, PathfindingNode stop)
        {
            List<PathfindingNode> toEvaluateSet = new List<PathfindingNode>();
            List<PathfindingNode> evaluatedSet = new List<PathfindingNode>();

            toEvaluateSet.Add(start);

            while (toEvaluateSet.Count > 0)
            {
                PathfindingNode current = GetLowestFScoreNode(toEvaluateSet);

                if (current.XId == stop.XId && current.YId == stop.YId)
                {
                    return ReconstructPath(current);
                }

                toEvaluateSet.Remove(current);
                evaluatedSet.Add(current);

                foreach (PathfindingNode neighbor in GetNeighbors(current))
                {
                    if (evaluatedSet.Contains(neighbor) || !neighbor.Walkable)
                        continue;

                    int tempGCost = current.GCost + 1;

                    if (!toEvaluateSet.Contains(neighbor) || tempGCost < neighbor.GCost)
                    {
                        neighbor.PreviousNode = current;
                        neighbor.GCost = tempGCost;
                        neighbor.HCost = Math.Abs(neighbor.XId - stop.XId) + Math.Abs(neighbor.YId - stop.YId);

                        if (!toEvaluateSet.Contains(neighbor))
                            toEvaluateSet.Add(neighbor);
                    }
                }
            }

            Console.WriteLine("No valid path found.");
            return null;
        }

        private List<PathfindingNode> GetNeighbors(PathfindingNode node)
        {
            List<PathfindingNode> neighbors = new List<PathfindingNode>();
            int[] dx = { 0, 1, 0, -1, 1, 1, -1, -1 };
            int[] dy = { 1, 0, -1, 0, 1, -1, 1, -1 }; // dx and dy arrays are determining 18 different move sequences, aka dx[i] = -1, dy[i] = 1 means we are moving to a top left tile

            for (int i = 0; i < 8; i++)
            {
                int newX = node.XId + dx[i];
                int newY = node.YId + dy[i];

                if (newX >= 0 && newX < mapRows && newY >= 0 && newY < mapColumns)
                {
                    PathfindingNode neighbor = PathfindingNodeMap[newX, newY];
                    if (neighbor != null)
                        neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }


        private List<PathfindingNode> ReconstructPath(PathfindingNode node) 
        {
            List<PathfindingNode> path = new List<PathfindingNode>();

            while (node != null)
            {
                path.Insert(0, node);
                node = node.PreviousNode;
            }

            return path;
        }

        private PathfindingNode GetLowestFScoreNode(List<PathfindingNode> nodes)
        {
            PathfindingNode lowest = nodes[0];
            foreach (PathfindingNode node in nodes)
            {
                if (node.FCost < lowest.FCost)
                    lowest = node;
            }
            return lowest;
        }
    }
}
