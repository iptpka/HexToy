
public enum HexDirection
{
	NE, E, SE, SW, W, NW 
}


public static class HexDirectionExtensions
{
	//opposite direction in a hexagon is three sides away, so add three and get modulo 6
	public static HexDirection Opposite (this HexDirection direction)
	{
		//return (int)direction < 3 ? (direction + 3) : (direction - 3); //replace with modulo?
		
		return (HexDirection)(((int)direction + 3)%6);
	}
}
