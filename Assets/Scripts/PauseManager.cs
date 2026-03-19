using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance; 
    
    public bool isPaused { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PauseGame()
    {
        Debug.Log("PauseManger paused game");
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }
}
