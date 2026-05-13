using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    public void Retry()
    {
    SceneManager.LoadScene("Fase1");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu_Inicial");
    }
}