using Map.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Map.Generators
{
  /// <summary>
  /// Base of all generators, set of help functions that
  /// can be used across multiple generators
  /// </summary>
  public class BaseGenerator
  {
    #region Attributes
    /// <summary>
    /// The height and width of the map
    /// </summary>
    private Vector2Int _mapSize;
    public Vector2Int MapSize
    {
      get
      {
        return _mapSize;
      }

      set
      {
        if (value.x % 2 == 0)
        {
          throw new Exception("Width must be an odd number");
        }

        if (value.y % 2 == 0)
        {
          throw new Exception("Height must be an off number");
        }

        _mapSize = value;
      }
    }

    /// <summary>
    /// The inner representation of all the tiles in the map
    /// </summary>
    protected Tile[] Map;

    #endregion

    #region Public API
    /// <summary>
    /// Allows access to the underlying tile array
    /// </summary>
    /// <returns></returns>
    public Tile[] GetTiles()
    {
      return Map;
    }

    /// <summary>
    /// Returns a tile from our single dimensional array using Multi dimensional indexes
    /// </summary>
    /// <param name="x">The x coordinate of the tile</param>
    /// <param name="y">The y corrdinate of the tile</param>
    /// <returns>The tile at the coordinates or NULL if the tile doesn't exist</returns>
    public Tile GetTile(int x, int y)
    {
      if (x < 0 || y < 0 || x >= MapSize.x || y >= MapSize.y)
      {
        return null;
      }

      int index = GetMapIndex(x, y);

      if (index < 0 || index >= Map.Length)
      {
        return null;
      }
      return Map[index];
    }

    public Tile GetRandomVisitedTile()
    {
      List<Tile> possibleLocations = Map.Where(t => t.IsFloor()).ToList();
      int index = UnityEngine.Random.Range(0, possibleLocations.Count);
      return possibleLocations.ElementAt(index);
    }

    #endregion

    #region Protected API

    /// <summary>
    /// Gets the tile in the direction numberOfTilesAway from the currentTile.
    /// </summary>
    /// <param name="currentTile">The current tile we are at</param>
    /// <param name="direction">The direction we want to go in</param>
    /// <param name="numberOfTilesAway">How many spaces we want to move</param>
    /// <returns>The tile we ended at or NULL if no tile exists</returns>
    protected Tile GetTileToDirection(Tile currentTile, ETileDirection direction, int numberOfTilesAway = 1)
    {
      switch (direction)
      {
        case ETileDirection.NORTH:
          return GetTile(currentTile.X, currentTile.Y + numberOfTilesAway);
        case ETileDirection.SOUTH:
          return GetTile(currentTile.X, currentTile.Y - numberOfTilesAway);
        case ETileDirection.EAST:
          return GetTile(currentTile.X + numberOfTilesAway, currentTile.Y);
        case ETileDirection.WEST:
          return GetTile(currentTile.X - numberOfTilesAway, currentTile.Y);
        case ETileDirection.NORTH_EAST:
          return GetTile(currentTile.X + numberOfTilesAway, currentTile.Y + numberOfTilesAway);
        case ETileDirection.NORTH_WEST:
          return GetTile(currentTile.X - numberOfTilesAway, currentTile.Y + numberOfTilesAway);
        case ETileDirection.SOUTH_EAST:
          return GetTile(currentTile.X + numberOfTilesAway, currentTile.Y - numberOfTilesAway);
        case ETileDirection.SOUTH_WEST:
          return GetTile(currentTile.X - numberOfTilesAway, currentTile.Y - numberOfTilesAway);
        default:
          return null;
      }
    }

    /// <summary>
    /// Determines if the currentTile is a border tile to any other tile.
    /// Border tile is defined as a border to a region, the map, or another visited tile.
    /// </summary>
    /// <param name="currentTile">The tile we want to check</param>
    /// <param name="skipDirection">Allows us to not look in a certain direction.
    ///     This is so that if we are carving we aren't checking the tile we just came from
    /// </param>
    /// <returns>Whether or not the tile is a border tile</returns>
    protected bool IsBorderTile(Tile currentTile, ETileDirection skipDirection = ETileDirection.NONE)
    {
      foreach (ETileDirection direction in Enum.GetValues(typeof(ETileDirection)))
      {
        if (direction != skipDirection && direction != ETileDirection.NONE)
        {
          Tile tile = GetTileToDirection(currentTile, direction);
          if (CheckIfTileIsOutOfMapOrInRegion(tile))
          {
            return true;
          }

          if (!Tile.diagonals.Contains(direction) && tile.IsFloor())
          {
            return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Checks if the tile is out of the map or if its a region
    /// </summary>
    /// <param name="tile">The tile to check</param>
    /// <returns>Whether or not its out of the map or in a region</returns>
    protected bool CheckIfTileIsOutOfMapOrInRegion(Tile tile)
    {
      return tile == null || tile.ParentRegion != null;
    }

    /// <summary>
    /// Converts the X,Y coordinates into a
    /// single index for use with our Map variable
    /// </summary>
    /// <param name="x">The x coordinate if our map were multi dimensional</param>
    /// <param name="y">The y coordinate if our map were multi dimensional</param>
    /// <returns>The index for the X,Y coordinate in the Map variable</returns>
    protected int GetMapIndex(int x, int y)
    {
      return (int)(x + MapSize.x * y);
    }

    protected Vector2Int GetRandomRegionPlacement(int regionWidth, int regionHeight)
    {
     return new Vector2Int(
         UnityEngine.Random.Range(0, ((int)(MapSize.x - regionWidth)) / 2) * 2 + 1,
         UnityEngine.Random.Range(0, ((int)(MapSize.y - regionHeight)) / 2) * 2 + 1
     );
    }

    #endregion
  }
}
