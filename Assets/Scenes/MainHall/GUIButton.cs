using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIButton : MonoBehaviour
{
    //following instruction from here
    //https://code.tutsplus.com/articles/quick-tip-pause-slow-motion-double-time-in-unity--active-8015
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 30, 80, 30), "SLOW"))
        {
            //// call our toggle function
            //slowMo();
        }
        if (GUI.Button(new Rect(0, 0, 120, 30), "PAUSE"))
        {
            //// call our toggle function
            //doPauseToggle();
        }

    }
}
