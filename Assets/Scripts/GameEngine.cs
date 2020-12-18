using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEngine : MonoBehaviour
{
    #region Timer
    public Timer gameTimer = new Timer(120);
    public GameObject portal = null;
    public Text timerUI = null;

    bool portalOpen = false;
    #endregion

    public GameObject[] healthBar;

    private void Update()
    {
        Timer();
    }

    void Timer()
    {
        if (portalOpen)
        {
            timerUI.text = "PORTAL OPEN";
            return;
        }

        if (gameTimer.Check(false))
        {
            portalOpen = true;
            portal.SetActive(true);
            return;
        }

        int min = (int)gameTimer.TimeRemaining / 60;

        timerUI.text = ((min != 0) ? min.ToString() + " : " : "") + ((int)gameTimer.TimeRemaining % 60).ToString((min != 0) ? "00" : "");
    }

    public void RaiseHealthBar(int health)
    {
        for (int x = 0; x < health; x++)
        {
            healthBar[x].SetActive(true);
        }
    }

    public void LowerHealthBar(int health)
    {
        for (int x = health; x < healthBar.Length; x++)
        {
            healthBar[x].SetActive(false);
        }
    }
}
