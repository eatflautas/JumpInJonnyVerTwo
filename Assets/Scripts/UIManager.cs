using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameState gameState;
    public GameObject HUDManager;

    private UIDocument doc;
    private HUDBehavior hud;
    
    public enum GameState
    {
        running,
        paused,
        dead
    }
    
    // UI Variables:
    
    // Wrappers
    private VisualElement buttonsWrapper;

    // Pause Screen Buttons
    private Button continueButton;
    private Button restartButton;
    private Button levelSelectButton;
    private Button mainMenuButton;
    
    // Level Select Buttons
    [SerializeField] private VisualTreeAsset levelSelectButtonsTemplate;
    // Creates buttons based on file Above
    private VisualElement levelSelectButtons;
    
    // Dead Buttons
    [SerializeField] 
    private VisualTreeAsset deadButtonsTemplate;
    // Creates buttons based on file Above
    private VisualElement deadButtons;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        doc = GetComponent<UIDocument>();
        hud = HUDManager.GetComponent<HUDBehavior>();
        
        // Hides the pause screen on start
        doc.rootVisualElement.visible = false;
        LockCursor();
        
        Time.timeScale = 1f;

        // Wrappers & Templates
        buttonsWrapper = doc.rootVisualElement.Q<VisualElement>("Buttons");
        levelSelectButtons = levelSelectButtonsTemplate.CloneTree();
        deadButtons = deadButtonsTemplate.CloneTree();

        // Main Menu buttons
        continueButton = doc.rootVisualElement.Q<Button>("ContinueButton");
        continueButton.clicked += ContinueButtonOnClicked;
        restartButton = doc.rootVisualElement.Q<Button>("RestartButton");
        restartButton.clicked += RestartLevel;
        levelSelectButton = doc.rootVisualElement.Q<Button>("LevelSelectButton");
        levelSelectButton.clicked += LevelSelectButtonOnClicked;
        mainMenuButton = doc.rootVisualElement.Q<Button>("MainMenuButton");
        mainMenuButton.clicked += MainMenuButtonOnClicked;

        // Level Select Buttons
        var backButton = levelSelectButtons.Q<Button>("BackButton"); 
        backButton.clicked += BackButtonOnClicked;
        
        // Dead Buttons
        var deadMainMenuButton = deadButtons.Q<Button>("MainMenuButton");
        deadMainMenuButton.clicked += MainMenuButtonOnClicked;
        var deadRestartButton = deadButtons.Q<Button>("RestartButton");
        deadRestartButton.clicked += RestartLevel;
    }
    private void Update()
    {
        bool pauseButtonPressed = InputManager.instance.PauseJustPressed;

        if (pauseButtonPressed)
        {
            if (gameState == GameState.running)
            {
                Pause();
            }
            else if (gameState == GameState.paused)
            {
                Unpause();
            }
        }
    }
    
    #region Main Menu Buttons

    private void ContinueButtonOnClicked()
    {
        Unpause();
    }

    private void LevelSelectButtonOnClicked()
    { 
        buttonsWrapper.Clear(); 
        buttonsWrapper.Add(levelSelectButtons);
    }
    
    private void MainMenuButtonOnClicked() 
    {
        SceneManager.LoadScene("StartingScreen");
    }

    #endregion

    #region Level Select Buttons

    private void BackButtonOnClicked() 
    {
        buttonsWrapper.Clear();
        buttonsWrapper.Add(continueButton);
        buttonsWrapper.Add(levelSelectButton);
        buttonsWrapper.Add(mainMenuButton); 
    }
    #endregion
    
    
    #region Pause / Unpause
    
    public void Pause()
    {
        Debug.Log("PauseManger paused game");
        Time.timeScale = 0f;
        doc.rootVisualElement.visible = true;
        UnlockCursor();
        hud.DisableHUD();
        gameState = GameState.paused;
    }

    public void Unpause()
    {
        Time.timeScale = 1f;
        doc.rootVisualElement.visible = false;
        LockCursor();
        hud.EnableHUD();
        gameState = GameState.running;
    }
    
    #endregion
    
    #region Cursor Actions

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    #endregion

    public void DeathScreen()
    {
        //Time.timeScale = 0f;
        //doc.rootVisualElement.visible = true;
        //UnlockCursor();
        //hud.DisableHUD();
        //buttonsWrapper.Clear(); 
        //buttonsWrapper.Add(deadButtons);
        gameState = GameState.dead;
        RestartLevel();
    }

    void RestartLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
