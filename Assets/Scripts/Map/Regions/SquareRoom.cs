using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Map.Regions
{
  class SquareRoom : Room
  {

    public int Height;
    public int Width;

    public Vector2 TopRightCorner
    {
      get
      {
        return new Vector2(
           X + Width,
           Y + Height
        );
      }
    }

    public Vector2 BottomLeftCorner
    {
      get
      {
        return new Vector2(X, Y);
      }
    }

    public override Rect GetBoundingRect()
    {
      return new Rect(X, Y, Width, Height);
    }

    public static SquareRoom CreateRandomSizedRoom(int MinWidth, int MaxWidth, int MinHeight, int MaxHeight)
    {
      SquareRoom randomRoom = new SquareRoom();
      randomRoom.Height = (int)UnityEngine.Random.Range(MinHeight, MaxHeight);
      randomRoom.Width = (int)UnityEngine.Random.Range(MinWidth, MaxWidth);
      if (randomRoom.Height % 2 == 0) randomRoom.Height++;
      if (randomRoom.Width % 2 == 0) randomRoom.Width++;

      return randomRoom;
    }
  }
}
