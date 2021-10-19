using UnityEngine;


public class AStarNode
{
    public int G { get; set; }
    public int H { get; set; }
    public int F { get; set; }
    public AStarNode Parent { get; set; }
    public Vector3Int CellPosition { get; set; }

    public AStarNode(Vector3Int position)
    {
        this.CellPosition = position;
    }
}
