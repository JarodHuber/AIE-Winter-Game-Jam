using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void ChangeScene(int sceneToChangeTo)
    {
        if (sceneToChangeTo == -1)
        {
            Application.Quit();
            return;
        }

        SceneManager.LoadScene(sceneToChangeTo);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("curLvl"));
    }
}
