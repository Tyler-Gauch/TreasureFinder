using Map.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rendering
{
  public class PredefinedRoomGenerator : MapRenderer
  {
    public int MapToScreenConversion = 10;

    /// <summary>
    /// Converts the map coordinates to screen coordinates
    /// </summary>
    /// <param name="map">The map coordinates to convert</param>
    /// <returns>The map coordinates in screen coordinates</returns>
    public override Vector3 ConvertMapToScreen(Vector3 map)
    {
      return map * MapToScreenConversion;
    }

    /// <summary>
    /// Converts the screen coordinates to map coordinates
    /// </summary>
    /// <param name="screen">The screen coordinates to convert</param>
    /// <returns>the screen coordinates in map coordinates</returns>
    public override Vector3 ConvertScreenToMap(Vector3 screen)
    {
      return screen / MapToScreenConversion;
    }

    protected override void PlacePlayer()
    {
      
    }
  }
}
