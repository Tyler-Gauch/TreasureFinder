using Map.Tiles;
using UnityEngine;

namespace Map.Regions
{
  public abstract class Room : Region
  {
    public int X = 0;
    public int Y = 0;

    public abstract RectInt GetBoundingRect();

    public bool Overlaps(Room other)
    {
      RectInt thisRect = this.GetBoundingRect();
      RectInt thatRect = other.GetBoundingRect();

      if (thisRect.xMin > thatRect.xMax || thatRect.xMin > thisRect.xMax)
      {
        return false;
      }

      if (thisRect.yMin > thatRect.yMax || thatRect.yMin > thisRect.yMax)
      {
        return false;
      }

      return true;
    }

    public bool Overlaps(Tile tile)
    {
      if (tile == null) return false;
      return GetBoundingRect().Contains(new Vector2Int(tile.X, tile.Y));
    }

  }
}
