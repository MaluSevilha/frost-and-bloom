using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject continueButton;

    private bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        Time.timeScale = 1f;
        EventSystem.current.SetSelectedGameObject(null);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (howToPlayPanel.activeSelf)
            {
                howToPlayPanel.SetActive(false);
                pausePanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(null);
                return;
            }

            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);
        howToPlayPanel.SetActive(false);
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void ContinueGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        Time.timeScale = 1f;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OpenHowToPlay()
    {
        pausePanel.SetActive(false);
        howToPlayPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void BackToPause()
    {
        howToPlayPanel.SetActive(false);
        pausePanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu_Inicial");
    }
}