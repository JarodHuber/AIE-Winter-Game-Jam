using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialoge : MonoBehaviour
{
    public Timer textSpeed = new Timer(.01f);
    [TextArea]
    public string[] dialoge;

    Text text = null;
    bool textFinished = false;
    int charCounter = 0;
    int stringCounter = 0;

    private void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    private void Update()
    {
        if(!textFinished)
        {
            if (Input.GetMouseButtonDown(0))
            {
                text.text = dialoge[stringCounter];

                textSpeed.Reset();
                charCounter = 0;

                textFinished = true;

                return;
            }

            if (textSpeed.Check())
            {
                if (charCounter < dialoge[stringCounter].Length)
                {
                    text.text += dialoge[stringCounter][charCounter];
                    charCounter++;
                }
                else
                {
                    text.text = dialoge[stringCounter];
                    charCounter = 0;

                    textFinished = true;
                }
            }

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            stringCounter++;

            if(stringCounter >= dialoge.Length)
            {
                // Exit dialoge

                GameObject.FindGameObjectWithTag("GameController").GetComponent<EnemyManager>().paused = false;

                Destroy(gameObject);
                return;
            }

            text.text = "";
            textFinished = false;
        }
    }
}
