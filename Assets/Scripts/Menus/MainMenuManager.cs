using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject mainMenuPanel;
    public GameObject howPlayPanel;

    public void OpenHowPlay()
    {
        mainMenuPanel.SetActive(false);
        howPlayPanel.SetActive(true);
    }

    public void CloseHowPlay()
    {
        howPlayPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
    }
}