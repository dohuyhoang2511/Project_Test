using UnityEngine;
using System.Collections;

public abstract class MazeAlgorithm {
	protected GridCell_Viz[,] gridCells;
	protected int mazeRows, mazeColumns;

	protected MazeAlgorithm(GridCell_Viz[,] gridCells) : base() {
		this.gridCells = gridCells;
		mazeRows = gridCells.GetLength(1);
		mazeColumns = gridCells.GetLength(0);
	}

	public abstract void CreateMaze ();
}
