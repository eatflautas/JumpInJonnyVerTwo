using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    
    void Update()
    {
        if (InputManager.instance.PauseJustPressed)
        {
            if (!PauseManager.instance.isPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }
        
    }

    
    #region Pause / Unpause
    
    public void Pause()
    {
        PauseManager.instance.PauseGame();
    }

    public void Unpause()
    {
        PauseManager.instance.ResumeGame();
    }
    
    #endregion
    
}
