using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HexGrid : MonoBehaviour
{
    public int width = 6;
	public int height = 6;
	
	public Color defaultColor = Color.white;
	public Color highlightColor = Color.blue;
	
	public HexCell cellPrefab;
	
	public HexCell[] Cells { get; private set; }
	
	public Text cellLabelPrefab;
	
	Canvas gridCanvas;

	void Awake ()
	{
		gridCanvas = GetComponentInChildren<Canvas>();
		
		Cells = new HexCell[height * width];
		for (int z = 0, i = 0; z < height; z++) 
		{
			for (int x = 0; x < width; x++) 
			{
				CreateCell(x, z, i++);
			}
		}
	}
	
	/*Creates new HexCell object, sets it into the Cells array and initializes it.
	Every odd row off cells is offset to the left, to keep the grid boundaries straight
	Each cell gets a randomized weight, ie. the cost of travelling through that particular cell.*/
	void CreateCell (int x, int z, int i)
	{
		Vector3 position;
		float offset = z * 0.5f - z / 2;
		position.x = (x + offset) * (HexCell.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexCell.outerRadius * 1.5f);
		
		HexCell cell = Instantiate<HexCell>(cellPrefab);
		
		Cells[i] = cell;
		cell.Weight = Random.Range(1,10) + (30 * Random.Range(0,2)) + (20 * Random.Range(0,2));
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		
		if (x > 0)
		{
			cell.SetNeighbor(HexDirection.W, Cells[i - 1]);
		}
		if (z > 0)
		{
			if ((z % 2) == 0 ) {
				cell.SetNeighbor(HexDirection.SE, Cells[i - width]);
				if (x > 0)
				{
					cell.SetNeighbor(HexDirection.SW, Cells[i - width - 1]);
				}
			}
			else 
			{
				cell.SetNeighbor(HexDirection.SW, Cells[i - width]);
				if (x < width - 1)
				{
					cell.SetNeighbor(HexDirection.SE, Cells[i - width + 1]);
				}
			}
		}
		
		//debug text labels
		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition = 
			new Vector2(position.x, position.z);
		label.text = cell.Weight.ToString();//cell.coordinates.ToStringOnSeparateLines();
	}
	
	public HexCell CellFromWorldPoint (Vector3 position)
	{
		//world space position to local
		position = transform.InverseTransformPoint(position); 
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		return Cells[index];
	}
}
