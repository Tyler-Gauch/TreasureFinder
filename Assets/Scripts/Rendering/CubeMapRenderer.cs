using Map.Tiles;
using UnityEngine;

namespace Rendering
{
  [AddComponentMenu("Mapping/Rendering/Cube Map Renderer")]
  public class CubeMapRenderer : MapRenderer
  {
    public GameObject FloorTile;

    public GameObject WallTile;

    public GameObject CeilingTile;

    public GameObject MarkerTile;

    protected override void RenderMap()
    {
      for (int x = 0; x < Map.MapSize.x; x++)
      {
        for (int y = 0; y < Map.MapSize.y; y++)
        {
          Vector3 mapLoc = ConvertMapToScreen(new Vector3(x, 0, y));
          Tile tile = Map.GetTile(x, y);

          if (tile.IsFloor())
          {
            GameObject floor = Instantiate(FloorTile, mapLoc, Quaternion.identity);
            floor.layer = 8; // render to minimap
            MapRender.Add(floor);
            if (CeilingTile != null)
            {
              MapRender.Add(Instantiate(CeilingTile, mapLoc + Vector3.up * 4, Quaternion.identity));
            }
          }
          else if (tile.IsWall())
          {
            GameObject firstWall = Instantiate(WallTile, mapLoc + Vector3.up, Quaternion.identity);
            firstWall.layer = 8; // render to minimap
            MapRender.Add(firstWall);
          } else if (tile.Type == ETileType.DOOR)
          {
            GameObject door = Instantiate(CeilingTile, mapLoc, Quaternion.identity);
            door.layer = 8; // render to minimap
            MapRender.Add(door);
            if (CeilingTile != null)
            {
              MapRender.Add(Instantiate(CeilingTile, mapLoc + Vector3.up * 4, Quaternion.identity));
            }
          }

          if (tile.DebugMark)
          {
            MapRender.Add(Instantiate(MarkerTile, mapLoc, Quaternion.identity));
          }

          PlacePlayer();
        }
      }
    }
  }
}
