using UnityEngine;
using UnityEngine.UI;

public class HandSelectionManager : MonoBehaviour
{
    public SendVibrationOnHover vibrationScript; // Reference to the SendVibrationOnHover script

    // These are the buttons in your UI
    public Button leftHandButton;
    public Button rightHandButton;

    void Start()
    {
        if (leftHandButton == null || rightHandButton == null)
        {
            Debug.LogError("One or more buttons are not assigned in the inspector.");
            return;
        }

        // Assigning button click listeners
        leftHandButton.onClick.AddListener(() => SetHandMode(false)); // Set mode to left hand
        rightHandButton.onClick.AddListener(() => SetHandMode(true)); // Set mode to right hand

        Debug.Log("Button listeners assigned correctly.");
    }

    public void SetHandMode(bool isRightHand)
    {
        if (vibrationScript != null)
        {
            vibrationScript.SetHandMode(isRightHand); // Call the method on SendVibrationOnHover script
            Debug.Log("Hand mode set to: " + (isRightHand ? "Right" : "Left"));
        }
        else
        {
            Debug.LogError("VibrationScript reference not set on HandSelectionManager.");
        }
    }
}
