using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexCell : MonoBehaviour
{
	public HexCoordinates coordinates;
	
	public Material defaultMaterial;
	public Material obstacleMaterial;
	public Material targetMaterial;
	
	/*magic number is the ratio of the inner and outer radiuses of any hexagon*/
	public const float outerRadius = 10f;
	public const float innerRadius = outerRadius * 0.866025404f;
	
	/*for use with A* pathfinding. 
	GCost is the cheapest known path to this cell.
	HCost is the estimate for the cost to reach goal going through this cell.
	FCost is the search priority of a cell. Cell with lowest value is prioritised in the search
	*/
	public int GCost { get; set; } = 1;
	public int HCost { get; set; }
	public int FCost 
	{ 
		get { return GCost + HCost;	}
	}
	
	private const int obstacleWeight = 30; //cells with a higher weight use obstacleMaterial
	
	private int _weight;
	
	public int Weight 
	{ 
		get
		{
			return _weight;
		}
		set 
		{
			_weight = value;
				transform.localScale = transform.localScale + Vector3.up * (Weight/3);
		}		
	}
	
	//The cell currently found as cheapest route to this cell.
	public HexCell CameFrom { get; set; }
	
	bool target = false; //used for highlighting the target
	
	Renderer rend;

	public HexCell[] neighbors;
	
	void Awake () 
	{
		rend = GetComponent<Renderer>();
	}
	
	void Start ()
	{
		if (Weight > obstacleWeight)
		{
			rend.material = obstacleMaterial;
		}
	}
	
	public HexCell GetNeighbor (HexDirection direction)
	{
		return neighbors[(int)direction];
	}
	
	public void SetNeighbor (HexDirection direction, HexCell cell) 
	{
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	public void ToggleTargetHighlight ()
	{
		if (!target)
		{
			rend.material = targetMaterial;
		}
		else 
		{
			if (Weight < obstacleWeight) 
			{
				rend.material = defaultMaterial;
			}
			else 
			{
				rend.material = obstacleMaterial;
			}
		}
		target = !target;
	}
}
