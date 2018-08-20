using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rendering.Components
{
  [AddComponentMenu("Components/Base Component")]
  [RequireComponent(typeof(BoxCollider))]
  public class BaseComponent : MonoBehaviour
  {
    public int WidthInTiles;
    public int HeightInTiles;
    public int DepthInTiles;
  }
}
