using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour 
{

	public Transform seeker;
	Vector3 seekerYOffset = new Vector3(0f, 8f, 0f);
	HexGrid grid;
	Stack<HexCell> currentPath;
	HexCell seekerCell;

	void Awake() 
	{
		Application.targetFrameRate = 15;
		grid = GetComponent<HexGrid> ();
	}

	void Start ()
	{
		seekerCell = grid.Cells[0];
		
		int targetIndex = Random.Range(1, grid.height * grid.width);
		HexCell target = grid.Cells[targetIndex];
		target.ToggleTargetHighlight();
		
		currentPath = FindPath(seekerCell, target);
	}

	void Update () 
	{
		HexCell previousSeekerCell = seekerCell;
		
		//get the next cell along the path and move the seeker to it
		seekerCell = currentPath.Pop();
		seeker.position = seekerCell.transform.position + seekerYOffset;
		
		//align seeker with movement between previous and current cell
		Vector3 relativePosition = seekerCell.transform.position - previousSeekerCell.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePosition);
        seeker.transform.rotation = rotation;
		
		//mirror seeker along the x-axis to make it "run"
		seeker.transform.localScale = Vector3.Reflect(seeker.transform.localScale, Vector3.right); 
		
		//when target has been reached, get new path and toggle colors for current cell and the new target
		if (currentPath.Count == 0) 
		{
			int targetIndex = Random.Range(0, grid.height * grid.width);
			while (grid.Cells[targetIndex] == seekerCell) 
			{
				targetIndex = Random.Range(0, grid.height * grid.width);
			}
			HexCell target = grid.Cells[targetIndex];
			target.ToggleTargetHighlight();
			seekerCell.ToggleTargetHighlight();
			currentPath = FindPath(seekerCell, target);
		}
	}

	/*Find shortest path from startCell to targetCell using A* and return it in a stack with
	the path's first cell on top. Current implementation very unefficient as openSet
	is a list, should be swapped for hash or priority queue.*/
	Stack<HexCell> FindPath(HexCell startCell, HexCell targetCell) 
	{
		List<HexCell> openSet = new List<HexCell>();
		HashSet<HexCell> closedSet = new HashSet<HexCell>();
		
		startCell.GCost = 0;
		openSet.Add(startCell);

		while (openSet.Count > 0) 
		{
			HexCell cell = openSet[0];
			
			//gets the cell with lowest cost
			for (int i = 1; i < openSet.Count; i ++) 
			{
				if (openSet[i].FCost <= cell.FCost && openSet[i].HCost < cell.HCost) 
				{
						cell = openSet[i];
				}
			}

			openSet.Remove(cell);
			closedSet.Add(cell);
			
			//stop pathfinding when target is found and return the path in correct order
			if (cell == targetCell)
			{
				return RetracePath(startCell,targetCell);
			}

			foreach (HexCell neighbor in cell.neighbors) 
			{
				if (neighbor == null || closedSet.Contains(neighbor) ) 
				{
					continue;
				}

				int newCostToNeighbor = cell.GCost + cell.Weight;
				if (newCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor)) 
				{
					neighbor.GCost = newCostToNeighbor;
					neighbor.HCost = neighbor.coordinates.DistanceTo(targetCell.coordinates)*5;
					neighbor.CameFrom = cell;

					if (!openSet.Contains(neighbor))
					{
						openSet.Add(neighbor);
					}
				}
			}
		}
		return null;
	}
	
	/*Retraces the path starting from the final cell and places them in a stack, 
	so that the first cell ends up on top*/
	Stack<HexCell> RetracePath(HexCell startCell, HexCell endCell) 
	{
		Stack<HexCell> path = new Stack<HexCell>();
		HexCell currentCell = endCell;

		while (currentCell != startCell && currentCell != null) 
		{
			path.Push(currentCell);
			currentCell = currentCell.CameFrom;
		}
		return path;
	}
}