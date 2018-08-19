using Map.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Map.Tiles
{
  public class Tile
  {

    public static ETileDirection[] diagonals = new ETileDirection[] { ETileDirection.NORTH_EAST, ETileDirection.NORTH_WEST, ETileDirection.SOUTH_EAST, ETileDirection.SOUTH_WEST};
    public static ETileDirection[] cardinal = new ETileDirection[] { ETileDirection.NORTH, ETileDirection.SOUTH, ETileDirection.EAST, ETileDirection.WEST };

    public Vector2 location;
    public bool Visited;
    public Tile NorthTile;
    public Tile SouthTile;
    public Tile WestTile;
    public Tile EastTile;

    public Region ParentRegion;

    public bool DebugMark;

    public int X;
    public int Y;

    public Tile(int x, int y)
    {
      this.X = x;
      this.Y = y;
    }

    public void SetTileAtDirection(Tile nextTile, ETileDirection direction)
    {
      switch (direction)
      {
        case ETileDirection.NORTH:
          NorthTile = nextTile;
          nextTile.SouthTile = this;
          break;
        case ETileDirection.SOUTH:
          SouthTile = nextTile;
          nextTile.NorthTile = this;
          break;
        case ETileDirection.EAST:
          EastTile = nextTile;
          nextTile.WestTile = this;
          break;
        case ETileDirection.WEST:
          WestTile = nextTile;
          nextTile.EastTile = this;
          break;
      }
    }

    public bool IsDeadend()
    {
      int numberOfWalls = 0;
      numberOfWalls += SouthTile == null ? 1 : 0;
      numberOfWalls += NorthTile == null ? 1 : 0;
      numberOfWalls += EastTile == null ? 1 : 0;
      numberOfWalls += WestTile == null ? 1 : 0;

      return numberOfWalls == 3;
    }

    public static ETileDirection GetOppositeDirection(ETileDirection direction)
    {
      switch (direction)
      {
        case ETileDirection.EAST:
          return ETileDirection.WEST;
        case ETileDirection.WEST:
          return ETileDirection.EAST;
        case ETileDirection.NORTH:
          return ETileDirection.SOUTH;
        case ETileDirection.SOUTH:
          return ETileDirection.NORTH;
        case ETileDirection.NORTH_EAST:
          return ETileDirection.NORTH_WEST;
        case ETileDirection.NORTH_WEST:
          return ETileDirection.NORTH_EAST;
        case ETileDirection.SOUTH_EAST:
          return ETileDirection.SOUTH_WEST;
        case ETileDirection.SOUTH_WEST:
          return ETileDirection.SOUTH_EAST;
        default:
          return ETileDirection.NONE;
      }
    }

    public new string ToString()
    {
      return "(" + X + ", " + Y + ")";
    }
  }
}
