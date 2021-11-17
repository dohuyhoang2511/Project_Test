using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAI.PathFinding;

public class GridCell : Node<Vector2Int>
{
    public bool isWalkable = true;

    private Grid_Viz mGrid;

    public GridCell(Vector2Int index, Grid_Viz grid) : base(index)
    {
        mGrid = grid;
    }

    public override List<Node<Vector2Int>> GetNeighbours()
    {
        return mGrid.GetNeighbours(Value.x, Value.y);
    }
}
