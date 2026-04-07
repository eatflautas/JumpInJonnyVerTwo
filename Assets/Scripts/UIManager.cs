using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameState gameState;

    private UIDocument doc;
    
    public enum GameState
    {
        running,
        paused
    }
    
    // UI Variables:
    
    // Wrappers
    private VisualElement buttonsWrapper;

    // Pause Screen Buttons
    private Button continueButton;
    private Button levelSelectButton;
    private Button mainMenuButton;
    
    // Level Select Buttons
    [SerializeField] private VisualTreeAsset levelSelectButtonsTemplate;
    // Creates buttons based on file Above
    private VisualElement levelSelectButtons;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        doc = GetComponent<UIDocument>();
        // Hides the pause screen on start
        doc.rootVisualElement.visible = false;
        
        Time.timeScale = 1f;

        // Wrappers & Templates
        buttonsWrapper = doc.rootVisualElement.Q<VisualElement>("Buttons");
        levelSelectButtons = levelSelectButtonsTemplate.CloneTree();

        // Main Menu buttons
        continueButton = doc.rootVisualElement.Q<Button>("ContinueButton");
        continueButton.clicked += ContinueButtonOnClicked;
        levelSelectButton = doc.rootVisualElement.Q<Button>("LevelSelectButton");
        levelSelectButton.clicked += LevelSelectButtonOnClicked;
        mainMenuButton = doc.rootVisualElement.Q<Button>("MainMenuButton");
        mainMenuButton.clicked += MainMenuButtonOnClicked;

        // Level Select Buttons
        var backButton = levelSelectButtons.Q<Button>("BackButton"); 
        backButton.clicked += BackButtonOnClicked;
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
        gameState = GameState.paused;
    }

    public void Unpause()
    {
        Time.timeScale = 1f;
        doc.rootVisualElement.visible = false;
        gameState = GameState.running;
    }
    
    #endregion
    
}
