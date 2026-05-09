using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string firstLevelSceneName = "Tutorial";

    public void PlayGame()
    {
        SceneManager.LoadScene(firstLevelSceneName);
    }
}