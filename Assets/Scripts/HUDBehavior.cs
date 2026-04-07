using UnityEngine;
using UnityEngine.UIElements;

public class HUDBehavior : MonoBehaviour
{
    public static HUDBehavior instance;
    private UIDocument UIDoc;
    
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        UIDoc = GetComponent<UIDocument>();
    }

    public void EnableHUD()
    {
        UIDoc.rootVisualElement.visible = true;
    }

    public void DisableHUD()
    {
        UIDoc.rootVisualElement.visible = false;
    }
}
