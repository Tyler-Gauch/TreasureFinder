using Map.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Map.Regions
{
  public abstract class Room : Region
  {
    public int X = 0;
    public int Y = 0;

    public abstract Rect GetBoundingRect();

    public bool Overlaps(Room other)
    {
      // if one region's right side is further left than
      // the other regions left side then they can't be over lapping
      return this.GetBoundingRect().Overlaps(other.GetBoundingRect());
    }

    public bool Overlaps(Tile tile)
    {
      if (tile == null) return false;
      return GetBoundingRect().Contains(new Vector2(tile.X, tile.Y));
    }

  }
}
