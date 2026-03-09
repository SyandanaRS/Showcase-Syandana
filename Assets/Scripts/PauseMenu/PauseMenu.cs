using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject player;

    public bool isPaused = false;

    void Awake()
    {
        Instance = this;
        pauseMenuUI.SetActive(false);
    }

    public void Pause()
    {
        if (isPaused)
        {
            Resume();
            return;
        }

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        player.GetComponent<Player>().enabled = false; // disable movement
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        player.GetComponent<Player>().enabled = true; // enable movement
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}