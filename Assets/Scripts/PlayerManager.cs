using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public Transform player;
    public bool PlayerDied;
    public float yDeath = -20f;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position.y < yDeath)
        {
            PlayerDied = true;
        }

        if (PlayerDied)
        {
            UIManager.instance.DeathScreen();
        }
    }
}
