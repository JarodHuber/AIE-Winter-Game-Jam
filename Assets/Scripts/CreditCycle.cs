using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditCycle : MonoBehaviour
{
    public Sprite[] credits;
    public Image img = null;
    public Timer holdLength = new Timer(3);
    public Timer creditNum = new Timer(3);

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            creditNum.Check(1);

        img.sprite = credits[(int)creditNum.Time];
    }
}
