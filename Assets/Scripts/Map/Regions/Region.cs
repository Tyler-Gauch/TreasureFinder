using Map.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Map.Regions
{
  public class Region
  {
    public HashSet<Tile> TilesInRegion;
    public Color RegionColor;
    private static int RegionCount;
    public int RegionID;

    public Region()
    {
      TilesInRegion = new HashSet<Tile>();
      RegionColor = UnityEngine.Random.ColorHSV(0f, 0.75f, 0.75f, 1f, 0.75f, 1f);
      RegionID = RegionCount++;
    }

    public void AddTile(Tile tile)
    {
      if (tile == null) return;

      this.TilesInRegion.Add(tile);
      tile.ParentRegion = this;
    }

    public void RemoveTile(Tile tile)
    {
      if (tile == null) return;

      this.TilesInRegion.Remove(tile);
      tile.ParentRegion = null;
    }

    public void AddRegion(Region otherRegion)
    {
      if (otherRegion == null) return;

      foreach (Tile tile in otherRegion.TilesInRegion)
      {
        this.AddTile(tile);
      }
    }

    public override bool Equals(object obj)
    {
      var region = obj as Region;
      return region != null &&
             RegionID == region.RegionID;
    }

    public override int GetHashCode()
    {
      return 1146541480 + RegionID.GetHashCode();
    }

    public static bool operator ==(Region leftSide, Region rightSide)
    {
      return leftSide?.RegionID == rightSide?.RegionID;
    }

    public static bool operator !=(Region leftSide, Region rightSide)
    {
      return leftSide?.RegionID != rightSide?.RegionID;
    }

  }
}
