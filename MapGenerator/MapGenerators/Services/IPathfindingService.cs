namespace MapGenerator.MapGenerators.Services
{
    public interface IPathfindingService<TNode>
    {
        public List<TNode> FindPath(TNode start, TNode stop);
        TNode[,] PathfindingNodeMap { get; }
    }
}
