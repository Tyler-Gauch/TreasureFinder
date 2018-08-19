using Map.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rendering
{
  class TreasureMapRenderer : MapRenderer
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
