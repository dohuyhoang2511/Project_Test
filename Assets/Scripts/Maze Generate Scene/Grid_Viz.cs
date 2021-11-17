using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAI.PathFinding;

public class Grid_Viz : MonoBehaviour
{
    public int numX; // columns
    public int numY; // rows

    // the prefab to the grid cell
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private GameObject mazeWallPrefab;
    [SerializeField] private NPC npc;
    [SerializeField] private GameObject gatePrefab;
    private GameObject gate;
    private GridCell_Viz[,] mGridCells;
    private GridCell mStartLocation;
    private int randomXGate;
    private int randomYGate;

    public bool canAutoMove;
    public bool hint;

    // Allow some color selections
    [SerializeField] Color RESET_COLOR = new Color(0.1f, 0.1f, 0.1f, 0.0f);
    [SerializeField] Color ADD_TO_OPEN_LIST = new Color(1.0f, 0.0f, 0.0f, 1.0f);

    void Start()
    {
        CreateGrid();

        MazeAlgorithm ma = new HuntAndKillMazeAlgorithm(mGridCells);
        ma.CreateMaze();

        canAutoMove = false;
        hint = false;
    }

    void Update()
    {
        if (hint == true)
        {
            HandleNPCMoveTo();
            hint = false;
        }
    }

    void CreateGrid()
    {
        mGridCells = new GridCell_Viz[numX, numY];

        randomXGate = Random.Range(5, 9);
        randomYGate = Random.Range(5, 12);

        for (int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                mGridCells[i, j] = new GridCell_Viz();

                GameObject floor = mGridCells[i, j].floor;
                floor = Instantiate(gridCellPrefab, new Vector3(i, j, 0.0f), Quaternion.identity) as GameObject;
                floor.transform.SetParent(transform);
                floor.name = "Floor " + i + "," + j;
                mGridCells[i, j] = floor.GetComponent<GridCell_Viz>();
                mGridCells[i, j].Cell = new GridCell(new Vector2Int(i, j), this);

                if (i == randomXGate && j == randomYGate)
                {
                    gate = Instantiate(gatePrefab, new Vector3(randomXGate, randomYGate, 0.0f), Quaternion.identity) as GameObject;
                    gate.transform.SetParent(transform);
                    gate.name = "Gate";
                    mGridCells[i, j] = gate.GetComponent<GridCell_Viz>();
                    mGridCells[i, j].Cell = new GridCell(new Vector2Int(i, j), this);

                }

                if (i == 0)
                {
                    mGridCells[i, j].westWall = Instantiate(mazeWallPrefab, new Vector3(i, j, 0.0f), Quaternion.identity) as GameObject;
                    mGridCells[i, j].westWall.transform.SetParent(transform);
                    mGridCells[i, j].westWall.transform.Rotate(Vector3.forward, 90f);
                    mGridCells[i, j].westWall.name = "West Wall " + i + "," + j;
                }

                mGridCells[i, j].eastWall = Instantiate(mazeWallPrefab, new Vector3(i, j, 0.0f), Quaternion.identity) as GameObject;
                mGridCells[i, j].eastWall.transform.SetParent(transform);
                mGridCells[i, j].eastWall.transform.Rotate(Vector3.forward, -90f);
                mGridCells[i, j].eastWall.name = "East Wall " + i + "," + j;

                if (j == 0 || j == 12)
                {
                    mGridCells[i, j].northWall = Instantiate(mazeWallPrefab, new Vector3(i, j, 0.0f), Quaternion.identity) as GameObject;
                    mGridCells[i, j].northWall.transform.SetParent(transform);
                    mGridCells[i, j].northWall.name = "North Wall " + i + "," + j;
                }

                mGridCells[i, j].southWall = Instantiate(mazeWallPrefab, new Vector3(i, j, 0.0f), Quaternion.identity) as GameObject;
                mGridCells[i, j].southWall.transform.Rotate(Vector3.forward, 180f);
                mGridCells[i, j].southWall.transform.SetParent(transform);
                mGridCells[i, j].southWall.name = "South Wall " + i + "," + j;

                //GameObject cell = Instantiate(gridCellPrefab, new Vector3(i, j, 0.0f), Quaternion.identity);
                //cell.name = "cell_" + i.ToString() + "_" + j.ToString();
                //cell.transform.SetParent(transform);
                //mGridCells[i, j] = cell.GetComponent<GridCell_Viz>();
                //mGridCells[i, j].Cell = new GridCell(new Vector2Int(i, j), this);
            }
        }
        mStartLocation = mGridCells[0, 0].Cell;
        npc.transform.position = new Vector3(mGridCells[0, 12].floor.transform.position.x, mGridCells[0, 12].floor.transform.position.y, 0.0f);
    }

    GridCell UpdatePlayerPositions()
    {
        Vector3 currentPos = npc.transform.position;
        mStartLocation = mGridCells[Mathf.RoundToInt(currentPos.x), Mathf.RoundToInt(currentPos.y)].Cell;
        return mStartLocation;
    }

    void HandleNPCMoveTo()
    {
        UpdatePlayerPositions();

        GameObject hitObj = gate.transform.gameObject;

        GridCell_Viz cellViz = hitObj.GetComponent<GridCell_Viz>();
        if (cellViz)
        {
            StartCoroutine(Coroutine_FindPathAndMove(mStartLocation, cellViz.Cell, npc));
        }
    }

    public void ResetColors()
    {
        for (int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                if (mGridCells[i, j].Cell.isWalkable)
                {
                    mGridCells[i, j].SetInnerColor(RESET_COLOR);
                }
                else
                {
                    mGridCells[i, j].SetInnerColor(RESET_COLOR);
                }
            }
        }
    }

    IEnumerator Coroutine_FindPathAndMove(GridCell start, GridCell goal, NPC obj)
    {
        ResetColors();

        AStarPathFinder<Vector2Int> pathFinder = new AStarPathFinder<Vector2Int>();
        pathFinder.HeuristicCost = ManhattanCostFunc;
        pathFinder.NodeTraversalCost = EuclideanCostFunc;

        pathFinder.Initialize(start, goal);

        PathFinderStatus status = pathFinder.Status;
        while (status == PathFinderStatus.RUNNING)
        {
            status = pathFinder.Step();
            yield return new WaitForSeconds(0.0f);
        }

        if (status == PathFinderStatus.SUCCESS)
        {
            List<Vector2Int> reverse_path = new List<Vector2Int>();
            PathFinder<Vector2Int>.PathFinderNode node = pathFinder.CurrentNode;

            while (node != null)
            {
                reverse_path.Add(node.Location.Value);
                node = node.Parent;
            }

            for (int i = reverse_path.Count - 1; i >= 0; i--)
            {
                Vector2Int index = reverse_path[i];
                Vector3 pos = mGridCells[index.x, index.y].transform.position;
                if(canAutoMove == true)
                {
                    obj.AddWayPoint(pos.x, pos.y);
                }

                //Change the color of the path
                mGridCells[index.x, index.y].SetInnerColor(ADD_TO_OPEN_LIST);
            }
            mStartLocation = goal;
        }
        if (status == PathFinderStatus.FAILURE)
        {
            Debug.Log("cannot find path");
        }
    }

    public static float ManhattanCostFunc(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public static float EuclideanCostFunc(Vector2Int a, Vector2Int b)
    {
        return (a - b).magnitude;
    }

    // GetNeighbours 
    public List<Node<Vector2Int>> GetNeighbours(int xx, int yy)
    {
        List<Node<Vector2Int>> neighbours = new List<Node<Vector2Int>>();

        //Top.
        if (yy < numY - 1)
        {
            if (mGridCells[xx, yy].northWall == null && mGridCells[xx, yy + 1].southWall == null)
            {
                neighbours.Add(mGridCells[xx, yy + 1].Cell);
            }
        }

        //Right.
        if (xx < numX - 1)
        {
            if (mGridCells[xx, yy].eastWall == null && mGridCells[xx + 1, yy].westWall == null)
            {
                neighbours.Add(mGridCells[xx + 1, yy].Cell);
            }
        }

        //Down
        if (yy > 0)
        {
            if (mGridCells[xx, yy].southWall == null && mGridCells[xx, yy - 1].northWall == null)
            {
                neighbours.Add(mGridCells[xx, yy - 1].Cell);
            }
        }

        //Left
        if (xx > 0)
        {
            if (mGridCells[xx, yy].westWall == null && mGridCells[xx - 1, yy].eastWall == null)
            {
                neighbours.Add(mGridCells[xx - 1, yy].Cell);
            }
        }

        ////Top-left
        //if (yy < numY - 1 && xx > 0)
        //{
        //    if ((mGridCells[xx, yy].northWall == null
        //        && mGridCells[xx, yy + 1].southWall == null
        //        && mGridCells[xx, yy + 1].westWall == null
        //        && mGridCells[xx - 1, yy + 1].eastWall == null)
        //        || (mGridCells[xx, yy].westWall == null
        //        && mGridCells[xx - 1, yy].eastWall == null
        //        && mGridCells[xx - 1, yy].northWall == null
        //        && mGridCells[xx - 1, yy + 1].southWall == null))
        //    {
        //        neighbours.Add(mGridCells[xx - 1, yy + 1].Cell);
        //    }
        //}
        return neighbours;
    }
}
