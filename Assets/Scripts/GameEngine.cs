using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEngine : MonoBehaviour
{
    public Timer gameTimer = new Timer(120);
    public GameObject portal = null;
    public Text timerUI = null;

    bool portalOpen = false;

    private void Update()
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
}
