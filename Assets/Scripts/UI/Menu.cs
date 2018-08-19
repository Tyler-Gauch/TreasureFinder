using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public class Menu : MonoBehaviour
  {
    private Text Banner;
    private Text Info;
    private Text Score;
    private int CurrentPoints = 0;

    private Canvas canvas;

    private void Start()
    {
      canvas = GetComponent<Canvas>();

      Banner = transform.Find("BannerMessage").GetComponent<Text>();
      Info = transform.Find("InfoMessage").GetComponent<Text>();
      Score = transform.Find("Score").GetComponent<Text>();

      UpdateScore(0);
      Info.text = "";
      Banner.text = "";
    }

    public void DisplayBannerMessage(string message)
    {
      Banner.text = message;
      StartCoroutine(FadeTextToZeroAlpha(5, Banner));
    }

    public void DisplayInfoMessage(string message)
    {
      Info.text = message;
      StartCoroutine(FadeTextToZeroAlpha(5, Info));
    }

    public void UpdateScore(int points)
    {
      CurrentPoints += points;
      Score.text = $"Points: {CurrentPoints}";
    }

    private IEnumerator FadeTextToZeroAlpha(float t, Text screenText)
    {
      screenText.color = new Color(screenText.color.r, screenText.color.g, screenText.color.b, 1);
      while (screenText.color.a > 0.0f)
      {
        screenText.color = new Color(screenText.color.r, screenText.color.g, screenText.color.b, screenText.color.a - (Time.deltaTime / t));
        yield return null;
      }
    }
  }
}
