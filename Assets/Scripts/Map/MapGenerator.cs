using Map.Regions;
using Map.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
  /// <summary>
  /// Generates a map of randomly sized rooms and links them together with hallways
  /// </summary>
  public class MapGenerator
  {

    /// <summary>
    /// The width of the map, must be an odd integer
    /// </summary>
    public int MapWidth
    {
      get
      {
        return _mapWidth;
      }
      set
      {
        if (value % 2 == 0)
          throw new Exception("Width must be an odd number");

        _mapWidth = value;
      }
    }

    /// <summary>
    /// The width of the map
    /// </summary>
    private int _mapWidth;

    /// <summary>
    /// The height of the map, must be an odd integer
    /// </summary>
    public int MapHeight
    {
      get
      {
        return _mapHeight;
      }
      set
      {
        if (value % 2 == 0)
          throw new Exception("Height must be an odd number");

        _mapHeight = value;
      }
    }

    /// <summary>
    /// The height of the map
    /// </summary>
    private int _mapHeight;

    /// <summary>
    /// The inner representation of all the tiles in the map
    /// </summary>
    private Tile[] Map;

    /// <summary>
    /// The inner representation of all the regions in the map.
    /// Used to hold all the rooms that were added to the map
    /// </summary>
    private List<Region> Regions;

    /// <summary>
    /// Holds the last direction chosen by the maze algorithm
    /// This is used along with the KeepSameDirectionPercentage
    /// to change how windy the paths of our maze are.
    /// </summary>
    private ETileDirection lastDirection = ETileDirection.NONE;

    #region Map Tuning Parameters

    /// <summary>
    /// This is the number of times we will attempt to place a room
    /// in the map. Every time we attemp to place a room, if the room
    /// overlaps with another room we will not place it. So if this is
    /// a low number we have a very low density room count in the map.
    /// But if this is a high number we will attempt to place more rooms
    /// and therefore result in a higher room density.
    /// </summary>
    public int NumberOfRoomPlacementRetries;

    /// <summary>
    /// This determines the percentage of time we should force staying
    /// in the same direction given that the same direction is an option.
    /// The higher this percentage the more straight paths we will generate.
    /// If the percentage is low we will have the more windier jagged paths
    /// </summary>
    public float KeepSameDirectionPercentage = 50f;

    /// <summary>
    /// By default we guarantee that all regions, rooms and paths, will be connected
    /// by at least 1 connection. This parameter is used in conjunction with the
    /// AdditionalConnectionAttempts to say that every time we attempt to place a connection
    /// past the first one, we will only succeed this percent of the time.
    /// </summary>
    public float AddAnotherConnectionPercentage = 50f;

    /// <summary>
    /// This parameter is used in conjunction with AddAnotherConnectionPercentage in order
    /// to place more than one connection. This parameter designates the number of times we will
    /// attempt to place another connection. The more attempts we give the more connections that
    /// will succeed.
    /// </summary>
    public float AdditionalConnectionAttempts = 2;

    /// <summary>
    /// This parameter has to deal with the removal of maze pieces after everything is connected.
    /// The way this works is by removing dead ends. However if we were to remove everything then
    /// we would have no paths. So this parameter tells the algorithm to keep around a certain
    /// percentage of tiles. It will either stop when the percentage is met or when there are
    /// no dead ends left.
    /// </summary>
    public float TilesToKeepPercentage = 50;

    #endregion

    #region Region Sizing Parameters
    /// <summary>
    /// The max height for any region that is placed in the map
    /// </summary>
    public int MaxRegionHeight;

    /// <summary>
    /// The max width for any region that is placed in the map
    /// </summary>
    public int MaxRegionWidth;

    /// <summary>
    /// The min height for any region that is placed in the map
    /// </summary>
    public int MinRegionHeight;

    /// <summary>
    /// The min width for any region that is placed in the map
    /// </summary>
    public int MinRegionWidth;

    #endregion

    public MapGenerator(int width, int height)
    {
      MapWidth = width;
      MapHeight = height;
    }

    /// <summary>
    /// Procedurally generate a map of connected rooms and pathways
    /// </summary>
    public void GenerateMap()
    {
      CreateDefaultTiles();
      PlaceRegions();
      GeneratePaths();
      ConnectPathsAndRegions();
      KillDeadends();
    }

    /// <summary>
    /// Sets up the base parameters and creates our empty, unvisted tiles
    /// </summary>
    private void CreateDefaultTiles()
    {

      Map = new Tile[MapWidth * MapHeight];
      Regions = new List<Region>();

      for (int y = 0; y < MapHeight; y++)
      {
        for (int x = 0; x < MapWidth; x++)
        {
          Map[GetMapIndex(x, y)] = new Tile(x, y);
        }
      }
    }

    /// <summary>
    /// First step to create our map is to place the rooms we want in our map.
    /// This will NumberOfRoomPlacementRetries attempt to place a randomly sized room
    /// in a random location on the map.
    /// </summary>
    private void PlaceRegions()
    {
      for (int numberOfPlacementAttempts = 0; numberOfPlacementAttempts < NumberOfRoomPlacementRetries; numberOfPlacementAttempts++)
      {
        Room newRoom = SquareRoom.CreateRandomSizedRoom(MinRegionWidth, MaxRegionWidth, MinRegionHeight, MaxRegionHeight);
        PlaceRoomRandomly(newRoom);
      }
    }

    /// <summary>
    /// This takes in a room and tries to place it in a random location on the map.
    /// The fancy math is taking from the blog post credited at the top of the file.
    /// Basically this forces the rooms to be placed in locations that line up with
    /// the maze generation. Much like how we don't allow even sized mazes.
    /// </summary>
    /// <param name="room">The room we want to place</param>
    /// <returns>Whether the room was placed or not</returns>
    private bool PlaceRoomRandomly(Room room)
    {
      Rect roomRect = room.GetBoundingRect();

      // this stupidity of math is to get the regions to line up with
      // the maze that gets created in the second step of generation
      Vector2 regionPlacement = new Vector2(
          UnityEngine.Random.Range(0, ((int)(MapWidth - roomRect.width)) / 2) * 2 + 1,
          UnityEngine.Random.Range(0, ((int)(MapHeight - roomRect.height)) / 2) * 2 + 1
      );

      // first check if we place the bottom left corner of
      // the region on the selected tile if it will fit
      Vector2 topRightCorner = new Vector2(
        regionPlacement.x + roomRect.width,
        regionPlacement.y + roomRect.height
      );

      // if the topRightCorner is not a real tile we are placing the region out
      // of the map and this is not a valid location.
      if (GetTile((int)topRightCorner.x, (int)topRightCorner.y) == null)
      {
        // for now we are only going to check bottom left placement
        // TODO: Open this up to trying different configurations before failing
        return false;
      }

      //if the region will fit here lets see if we are overlapping another room
      room.X = (int)regionPlacement.x;
      room.Y = (int)regionPlacement.y;
      if (!CanPlaceRoom(room))
      {
        return false;
      }

      // go through all the tiles the room overlaps and add them
      // to the region, and set their neighboring tiles
      Regions.Add(room);
      for (int x = room.X; x < room.X + roomRect.width; x++)
      {
        for (int y = (int)room.Y; y < room.Y + roomRect.height; y++)
        {
          Tile tile = GetTile(x, y);
          room.AddTile(tile);

          // now we need to set the tiles neighbors
          //TODO: There has got to be a better way!
          Tile north = GetTileToDirection(tile, ETileDirection.NORTH);
          Tile south = GetTileToDirection(tile, ETileDirection.SOUTH);
          Tile east = GetTileToDirection(tile, ETileDirection.EAST);
          Tile west = GetTileToDirection(tile, ETileDirection.WEST);
          if (room.Overlaps(north)) tile.NorthTile = north;
          if (room.Overlaps(south)) tile.SouthTile = south;
          if (room.Overlaps(east)) tile.EastTile = east;
          if (room.Overlaps(west)) tile.WestTile = west;

          tile.Visited = true;
        }
      }

      return true;
    }

    /// <summary>
    /// This determines if the room can be placed where we just tried to place it.
    /// We go through the list of all regions we have already placed and check
    /// if we are overlapping any of them if we are we can not place the room
    /// </summary>
    /// <param name="room">The room to check if its placement is okay</param>
    /// <returns>Whether or not the placement can happen</returns>
    private bool CanPlaceRoom(Room room)
    {
      foreach (Region existingRegion in Regions)
      {
        if (existingRegion is Room)
        {
          if (room.Overlaps(existingRegion as Room))
          {
            return false;
          }
        }
      }

      return true;
    }

    /// <summary>
    /// This function is the maze generation. We use a simple growing tree
    /// algorithm to build our maze. This works with a few steps:
    /// 1) Pick a starting cell and add it to our list
    /// 2) while we have cells to pick
    /// 3) pick a random cell
    /// 4) pick a random unvisted neighbor
    /// 5) add the neighbor to the list
    /// 6) if no neighbors exist remove the cell from the list
    /// 
    /// We then repeat this until all our cells are either visited
    /// or surrounded by visted cells.
    /// </summary>
    private void GeneratePaths()
    {
      for (int y = 1; y < MapHeight; y += 2)
      {
        for (int x = 1; x < MapWidth; x += 2)
        {
          Tile start = GetTile(x, y);
          if (start.Visited || IsBorderTile(start)) continue;

          Stack<Tile> unvisited = new Stack<Tile>();
          unvisited.Push(start);
          Region currentRegion = new Region();

          // take a cell out of the list
          // carve a random cell next to it
          // add that cell to the list

          while (unvisited.Count > 0)
          {
            Tile currentTile = unvisited.Peek();
            currentTile.Visited = true;
            currentRegion.AddTile(currentTile);

            Tile nextTile = CarveTile(currentTile);
            if (nextTile == null)
            {
              unvisited.Pop();
              lastDirection = ETileDirection.NONE;
            }
            else
            {
              unvisited.Push(nextTile);
            }
          }

          Regions.Add(currentRegion);
        }
      }
    }

    /// <summary>
    /// Deals with the actual visiting of tiles. It works by picking a random direction
    /// to carve in, and then we carve the cell we want to go to and the wall in between
    /// the two cells. When carving happens we add the carved cells to the region that
    /// our current cell belongs to. This function also deals with the KeepSameDirectionPercentage
    /// that determines how windy/jagged our maze is.
    /// </summary>
    /// <param name="currentTile">The current tile we are carving from</param>
    /// <param name="forceDirection">A forced direction to carve in. If not passed it will pick randomly</param>
    /// <returns>The tile we ended up carving into or NULL if no tile was picked</returns>
    private Tile CarveTile(Tile currentTile, ETileDirection forceDirection = ETileDirection.NONE)
    {
      ETileDirection chosenDirection;
      if (forceDirection == ETileDirection.NONE)
      {
        List<ETileDirection> directions = new List<ETileDirection>();

        foreach (ETileDirection direction in Tile.cardinal)
        {
          if (CanCarve(currentTile, direction))
          {
            directions.Add(direction);
          }
        }

        if (directions.Count() == 0)
        {
          return null;
        }

        if (directions.Contains(lastDirection) && UnityEngine.Random.Range(0, 100) < KeepSameDirectionPercentage)
        {
          chosenDirection = lastDirection;
        }
        else
        {
          chosenDirection = directions.ElementAt(UnityEngine.Random.Range(0, directions.Count()));
        }
      }
      else
      {
        chosenDirection = forceDirection;
      }

      lastDirection = chosenDirection;

      Tile nextTile = GetTileToDirection(currentTile, chosenDirection, 2);
      Tile wall = GetTileToDirection(currentTile, chosenDirection, 1);
      nextTile.Visited = true;
      wall.Visited = true;
      currentTile.ParentRegion.AddTile(wall);
      currentTile.SetTileAtDirection(wall, chosenDirection);
      wall.SetTileAtDirection(nextTile, chosenDirection);

      return nextTile;
    }

    /// <summary>
    /// Determines whether or not we can go from the currentTile
    /// and carve into the tile 2 spaces in the wanted direction.
    /// We use 2 spaces here because we need to carve through a
    /// wall and then carve through the cell we want to go to.
    /// </summary>
    /// <param name="currentTile">The tile we are at now</param>
    /// <param name="direction">The direction we want to carve in</param>
    /// <returns>Whether or not we can carve to the next tile</returns>
    private bool CanCarve(Tile currentTile, ETileDirection direction)
    {
      Tile tile = GetTileToDirection(currentTile, direction, 2);
      return tile != null && !tile.Visited && !IsBorderTile(tile, Tile.GetOppositeDirection(direction));
    }

    /// <summary>
    /// Gets the tile in the direction numberOfTilesAway from the currentTile.
    /// </summary>
    /// <param name="currentTile">The current tile we are at</param>
    /// <param name="direction">The direction we want to go in</param>
    /// <param name="numberOfTilesAway">How many spaces we want to move</param>
    /// <returns>The tile we ended at or NULL if no tile exists</returns>
    private Tile GetTileToDirection(Tile currentTile, ETileDirection direction, int numberOfTilesAway = 1)
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
    private bool IsBorderTile(Tile currentTile, ETileDirection skipDirection = ETileDirection.NONE)
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

          if (!Tile.diagonals.Contains(direction) && tile.Visited)
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
    private bool CheckIfTileIsOutOfMapOrInRegion(Tile tile)
    {
      return tile == null || tile.ParentRegion != null;
    }

    /// <summary>
    /// This function deals with adding connections between our regions
    /// we randomly placed, and the maze paths we generated around them.
    /// This works first by looking at all regions we have. Then it picks one
    /// finds all connections for that region, then randomly picks a connection
    /// to carve open. It then adds the region we just carved into into the region
    /// we are in. It then removes that region from the list. We then repeat that
    /// until we have only 1 region left.
    /// </summary>
    private void ConnectPathsAndRegions()
    {
      List<Region> allRegions = new List<Region>();
      allRegions.AddRange(Regions);

      while (allRegions.Count > 1)
      {
        Region currentRegion = allRegions.First();
        IList<KeyValuePair<ETileDirection, Tile>> possibleconnections = GetPossibleConnections(currentRegion);

        if (possibleconnections.Count == 0)
        {
          Debug.Log("Region has no connection points");
          continue;
        }

        for (int i = 0; i <= AdditionalConnectionAttempts && possibleconnections.Count > 0; i++)
        {

          if (i > 0 && UnityEngine.Random.Range(0, 100) > AddAnotherConnectionPercentage)
          {
            continue;
          }

          int randomConnection = UnityEngine.Random.Range(0, possibleconnections.Count);
          KeyValuePair<ETileDirection, Tile> connection = possibleconnections.ElementAt(randomConnection);
          possibleconnections.RemoveAt(randomConnection);

          Tile nextRegion = CarveTile(connection.Value, connection.Key);

          if (i == 0)
          {
            Region otherRegion = nextRegion.ParentRegion;
            currentRegion.AddRegion(otherRegion);
            allRegions.Remove(otherRegion);
          }
        }
      }
    }

    /// <summary>
    /// This function gets all possible connections for a region. A connection is defined
    /// as a tile that exists in the current region that can be carved into another tile
    /// that exists in a different region. We return all tile direction pairs for a connection
    /// because a single tile may have multiple ways to carve and make a connection.
    /// </summary>
    /// <param name="region">The region we want to find connections for</param>
    /// <returns>A list of direction, tiles that can be carved to another region</returns>
    private IList<KeyValuePair<ETileDirection, Tile>> GetPossibleConnections(Region region)
    {
      List<KeyValuePair<ETileDirection, Tile>> possibleConnections = new List<KeyValuePair<ETileDirection, Tile>>();

      foreach (Tile tile in region.TilesInRegion)
      {
        foreach (ETileDirection direction in Tile.cardinal)
        {
          Tile otherRegion = GetTileToDirection(tile, direction, 2);

          if (otherRegion != null && otherRegion.ParentRegion != null && tile.ParentRegion != otherRegion.ParentRegion)
          {
            possibleConnections.Add(new KeyValuePair<ETileDirection, Tile>(direction, tile));
          }
        }
      }

      return possibleConnections;
    }

    /// <summary>
    /// Now that our map is all roomed up and connected with hallways we have a bunch of
    /// random deadend hallways and random windy paths. This function helps cut down on the
    /// random windy hallways but killing off a certain percentage of cells that are deadends
    /// </summary>
    private void KillDeadends()
    {
      List<Tile> visited = Map.Where(t => t.Visited).ToList();
      int totalTilesToRemove = visited.Count - (int)(visited.Count * (TilesToKeepPercentage / 100));

      for (int i = 0; i < totalTilesToRemove; i++)
      {
        Tile tile = visited.Where(t => t.IsDeadend()).FirstOrDefault();
        visited.Remove(tile);
        // there are no deadends left, we killed them all
        if (tile == null)
        {
          break;
        }

        tile.Visited = false;

        // find the one not null connection
        if (tile.NorthTile != null) tile.NorthTile.SouthTile = null;
        if (tile.SouthTile != null) tile.SouthTile.NorthTile = null;
        if (tile.EastTile != null) tile.EastTile.WestTile = null;
        if (tile.WestTile != null) tile.WestTile.EastTile = null;

        tile.NorthTile = null;
        tile.SouthTile = null;
        tile.EastTile = null;
        tile.WestTile = null;

        if (tile.ParentRegion != null)
        {
          tile.ParentRegion.RemoveTile(tile);
        }
        else
        {
          Debug.Log("Region less tile?");
        }
      }
    }

    /// <summary>
    /// Converts the X,Y coordinates into a
    /// single index for use with our Map variable
    /// </summary>
    /// <param name="x">The x coordinate if our map were multi dimensional</param>
    /// <param name="y">The y coordinate if our map were multi dimensional</param>
    /// <returns>The index for the X,Y coordinate in the Map variable</returns>
    private int GetMapIndex(int x, int y)
    {
      return (int)(x + MapWidth * y);
    }

    /// <summary>
    /// Returns a tile from our single dimensional array using Multi dimensional indexes
    /// </summary>
    /// <param name="x">The x coordinate of the tile</param>
    /// <param name="y">The y corrdinate of the tile</param>
    /// <returns>The tile at the coordinates or NULL if the tile doesn't exist</returns>
    public Tile GetTile(int x, int y)
    {
      if (x < 0 || y < 0 || x >= MapWidth || y >= MapHeight)
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
      List<Tile> possibleLocations = Map.Where(t => t.Visited).ToList();
      int index = UnityEngine.Random.Range(0, possibleLocations.Count);
      return possibleLocations.ElementAt(index);
    }
  }
}
