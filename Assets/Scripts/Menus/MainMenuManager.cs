using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string firstLevelSceneName = "Scenes/Tutorial";

    public void PlayGame()
    {
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void OpenOptions()
    {
        Debug.Log("Abrir opções");
    }

    public void OpenCredits()
    {
        Debug.Log("Abrir créditos");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Sair do jogo");
    }
}