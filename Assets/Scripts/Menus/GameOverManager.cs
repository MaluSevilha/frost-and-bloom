using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuManager : MonoBehaviour
{
    public void Retry()
    {
    string lastLevel = PlayerPrefs.GetString("LastLevel", "Tutorial");
    SceneManager.LoadScene(lastLevel);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu_Inicial");
    }
}