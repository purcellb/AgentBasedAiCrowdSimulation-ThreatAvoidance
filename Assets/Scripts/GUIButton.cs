using UnityEngine;

namespace Assets.Scenes.MainHall
{
    public class GUIButton : MonoBehaviour
    {
        //simple time manipulation buttons for fun
        private void OnGUI()
        {
            if (GUI.Button(new Rect(1, 1, 80, 30), "SLOW"))
            {
                Time.timeScale = .1f;
            }
            if (GUI.Button(new Rect(82, 1, 80, 30), "NORMAL"))
            {
                Time.timeScale = 1.0f;
            }
            if (GUI.Button(new Rect(1, 32, 100, 30), "PAUSE"))
            {
                Time.timeScale = 0.0f;
            }
        }
    }
}
