using UnityEngine;
using System.Collections;

public class HuntAndKillMazeAlgorithm : MazeAlgorithm
{

    private int currentRow = 0;
    private int currentColumn = 0;

    private bool courseComplete = false;

    public HuntAndKillMazeAlgorithm(GridCell_Viz[,] gridCells) : base(gridCells)
    {
    }

    public override void CreateMaze()
    {
        HuntAndKill();
    }

    private void HuntAndKill()
    {
        gridCells[currentColumn, currentRow].isVisited = true;

        while (!courseComplete)
        {
            Kill(); // Will run until it hits a dead end.
            Hunt(); // Finds the next unvisited cell with an adjacent visited cell. If it can't find any, it sets courseComplete to true.
        }
    }

    private void Kill()
    {
        while (RouteStillAvailable(currentColumn, currentRow))
        {
            int direction = Random.Range(1, 5);
            //int direction = ProceduralNumberGenerator.GetNextNumber ();

            if (direction == 1 && CellIsAvailable(currentColumn, currentRow - 1))
            {
                // North
                DestroyWallIfItExists(gridCells[currentColumn, currentRow - 1].northWall);
                DestroyWallIfItExists(gridCells[currentColumn, currentRow].southWall);
                currentRow--;
            }
            else if (direction == 2 && CellIsAvailable(currentColumn, currentRow + 1))
            {
                // South
                DestroyWallIfItExists(gridCells[currentColumn, currentRow + 1].southWall);
                DestroyWallIfItExists(gridCells[currentColumn, currentRow].northWall);
                currentRow++;
            }
            else if (direction == 3 && CellIsAvailable(currentColumn + 1, currentRow))
            {
                // East
                DestroyWallIfItExists(gridCells[currentColumn, currentRow].eastWall);
                DestroyWallIfItExists(gridCells[currentColumn + 1, currentRow].westWall);
                currentColumn++;
            }
            else if (direction == 4 && CellIsAvailable(currentColumn - 1, currentRow))
            {
                // West
                DestroyWallIfItExists(gridCells[currentColumn, currentRow].westWall);
                DestroyWallIfItExists(gridCells[currentColumn - 1, currentRow].eastWall);
                currentColumn--;
            }

            gridCells[currentColumn, currentRow].isVisited = true;
        }
    }

    private void Hunt()
    {
        courseComplete = true; // Set it to this, and see if we can prove otherwise below!

        for (int c = 0; c < mazeColumns; c++)
        {
            for (int r = 0; r < mazeRows; r++)
            {
                if (!gridCells[c, r].isVisited && CellHasAnAdjacentVisitedCell(c, r))
                {
                    courseComplete = false; // Yep, we found something so definitely do another Kill cycle.
                    currentRow = r;
                    currentColumn = c;
                    DestroyAdjacentWall(currentColumn, currentRow);
                    gridCells[currentColumn, currentRow].isVisited = true;
                    return; // Exit the function
                }
            }
        }
    }


    private bool RouteStillAvailable(int column, int row)
    {
        int availableRoutes = 0;

        if (row > 0 && !gridCells[column, row - 1].isVisited)
        {
            availableRoutes++;
        }

        if (row < mazeRows - 1 && !gridCells[column, row + 1].isVisited)
        {
            availableRoutes++;
        }

        if (column > 0 && !gridCells[column - 1, row].isVisited)
        {
            availableRoutes++;
        }

        if (column < mazeColumns - 1 && !gridCells[column + 1, row].isVisited)
        {
            availableRoutes++;
        }

        return availableRoutes > 0;
    }

    private bool CellIsAvailable(int column, int row)
    {
        if (row >= 0 && row < mazeRows && column >= 0 && column < mazeColumns && !gridCells[column, row].isVisited)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DestroyWallIfItExists(GameObject wall)
    {
        if (wall != null)
        {
            GameObject.Destroy(wall);
        }
    }

    private bool CellHasAnAdjacentVisitedCell(int column, int row)
    {
        int visitedCells = 0;

        // Look 1 row up (north) if we're on row 1 or greater
        if (row > 0 && gridCells[column, row - 1].isVisited)
        {
            visitedCells++;
        }

        // Look one row down (south) if we're the second-to-last row (or less)
        if (row < (mazeRows - 2) && gridCells[column, row + 1].isVisited)
        {
            visitedCells++;
        }

        // Look one row left (west) if we're column 1 or greater
        if (column > 0 && gridCells[column - 1, row].isVisited)
        {
            visitedCells++;
        }

        // Look one row right (east) if we're the second-to-last column (or less)
        if (column < (mazeColumns - 2) && gridCells[column + 1, row].isVisited)
        {
            visitedCells++;
        }

        // return true if there are any adjacent visited cells to this one
        return visitedCells > 0;
    }

    private void DestroyAdjacentWall(int column, int row)
    {
        bool wallDestroyed = false;

        while (!wallDestroyed)
        {
            int direction = Random.Range(1, 5);
            //int direction = ProceduralNumberGenerator.GetNextNumber ();

            if (direction == 1 && row > 0 && gridCells[column, row - 1].isVisited)
            {
                DestroyWallIfItExists(gridCells[column, row - 1].northWall);
                DestroyWallIfItExists(gridCells[column, row].southWall);
                wallDestroyed = true;
            }
            else if (direction == 2 && row < (mazeRows - 2) && gridCells[column, row + 1].isVisited)
            {
                DestroyWallIfItExists(gridCells[column, row + 1].southWall);
                DestroyWallIfItExists(gridCells[column, row].northWall);
                wallDestroyed = true;
            }
            else if (direction == 3 && column > 0 && gridCells[column - 1, row].isVisited)
            {
                DestroyWallIfItExists(gridCells[column, row].westWall);
                DestroyWallIfItExists(gridCells[column - 1, row].eastWall);
                wallDestroyed = true;
            }
            else if (direction == 4 && column < (mazeColumns - 2) && gridCells[column + 1, row].isVisited)
            {
                DestroyWallIfItExists(gridCells[column, row].eastWall);
                DestroyWallIfItExists(gridCells[column + 1, row].westWall);
                wallDestroyed = true;
            }
        }
    }

}
