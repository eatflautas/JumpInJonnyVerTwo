using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    private UIDocument doc;

    // Wrappers
    private VisualElement buttonsWrapper;

    // Main Menu Buttons
    private Button newGameButton;
    private Button levelSelectButton;
    private Button quitButton;

    // Level Select Buttons
    [SerializeField] private VisualTreeAsset levelSelectButtonsTemplate;
    // Creates buttons based on file Above
    private VisualElement levelSelectButtons;

    private void Awake()
    {
        doc = GetComponent<UIDocument>();
        Time.timeScale = 1f;

        // Wrappers & Templates
        buttonsWrapper = doc.rootVisualElement.Q<VisualElement>("Buttons");
        levelSelectButtons = levelSelectButtonsTemplate.CloneTree();

        // Main Menu buttons
        newGameButton = doc.rootVisualElement.Q<Button>("NewGameButton");
        newGameButton.clicked += NewGameButtonOnClicked;
        levelSelectButton = doc.rootVisualElement.Q<Button>("LevelSelectButton");
        levelSelectButton.clicked += LevelSelectButtonOnClicked;
        quitButton = doc.rootVisualElement.Q<Button>("QuitButton");
        quitButton.clicked += QuitButtonOnClicked;

        // Level Select Buttons
        var backButton = levelSelectButtons.Q<Button>("BackButton"); 
        backButton.clicked += BackButtonOnClicked;

    }

    #region Main Menu Buttons

    private void NewGameButtonOnClicked()
    { 
        SceneManager.LoadScene("Sandbox");
    }

    private void LevelSelectButtonOnClicked()
    { 
        buttonsWrapper.Clear(); 
        buttonsWrapper.Add(levelSelectButtons);
    }
    
    private void QuitButtonOnClicked() 
    {
        Application.Quit();
    }

    #endregion

    #region Level Select Buttons

    private void BackButtonOnClicked() 
    {
        buttonsWrapper.Clear();
        buttonsWrapper.Add(newGameButton);
        buttonsWrapper.Add(levelSelectButton);
        buttonsWrapper.Add(quitButton); 
    }
    #endregion
}

