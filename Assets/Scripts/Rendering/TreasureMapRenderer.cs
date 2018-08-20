using Map.Tiles;
using UnityEngine;

namespace Rendering
{
  [AddComponentMenu("Mapping/Rendering/Treasure Map Renderer")]
  public class TreasureMapRenderer : CubeMapRenderer
  {
    public GameObject Treasure;

    protected override void RenderMap()
    {
      base.RenderMap();
      PlacePlayer();
      PlaceTreasure();
    }

    protected void PlaceTreasure()
    {
      Tile endTile = Map.GetRandomVisitedTile();
      Treasure.transform.position = new Vector3(endTile.X, 1, endTile.Y);
    }
  }
}
