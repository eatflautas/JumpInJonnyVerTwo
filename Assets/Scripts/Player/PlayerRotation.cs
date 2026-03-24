using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    private Camera mainCamera;

    public bool firstPersonMode = true;

    void Start()
    {
        // Finds the Main Camera
        mainCamera = Camera.main;
    }
    void FixedUpdate()
    {
        if (firstPersonMode)
        {
            // Rotates the player to the camera every physics update
            RotatePlayer();
        }
    }

    void RotatePlayer()
    {
        // If the main camera exists,
        if (mainCamera != null)
        {
            // track these variables
            Vector3 cameraForward = mainCamera.transform.forward;
            cameraForward.y = 0f; // and dont change the y rotation.

            // If the camera rotation isn't the same anymore,
            if (cameraForward != Vector3.zero)
            {
                // check it again
                Quaternion updatedRotation = Quaternion.LookRotation(cameraForward);
                // and match the player's rotation to it.
                transform.rotation = updatedRotation;
            }
        }
    }
}
