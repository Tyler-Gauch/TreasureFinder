using Map.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Map.Regions
{
  [Serializable]
  public class PredefinedRoom : Room
  {
    public Vector2Int Size;
    public List<Vector2Int> Doorways;

    public override RectInt GetBoundingRect()
    {
      return new RectInt(X, Y, Size.x, Size.y);
    }

    public bool IsDoorway(Tile tile)
    {
      Vector2Int tileLoc = new Vector2Int(tile.X - X, tile.Y - Y);
      return Doorways.Where(d => d == tileLoc).Count() > 0;
    }
  }
}
