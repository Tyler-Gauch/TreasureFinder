using Map.Tiles;
using Rendering;
using Tiles;
using UI;
using UnityEngine;

namespace Items
{
  [AddComponentMenu("Items/Treasure Chest")]
  public class Treasure : BaseTile
  {
    public MapRenderer map;
    public Menu menu;
    public int Points = 100;

    void OnTriggerEnter(Collider other)
    {
      if (other.gameObject.tag == "Player")
      {
        menu.DisplayBannerMessage("Congratualations! You found the treasure! Can you do it again?");
        menu.UpdateScore(Points);
        Tile t = map.Map.GetRandomVisitedTile();
        transform.position = new Vector3(t.X, 1, t.Y);
      }
    }
  }
}
