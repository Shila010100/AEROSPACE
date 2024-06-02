using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Threading.Tasks;

public class SendVibrationOnHover : MonoBehaviour
{
    private WebSocketConnectionManager webSocketManager;
    private float lastSentTime = 0.0f;
    private float sendInterval = 0.017f; // Interval in seconds, adjust as needed
    public bool isRightHandMode = true; // Default to right hand, toggle through UI

    void Start()
    {
        // Find the WebSocketConnectionManager in the scene
        webSocketManager = FindObjectOfType<WebSocketConnectionManager>();

        // Get the XRBaseInteractable component and subscribe to the hover event
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(HandleHoverEntered); // Ensure no duplicates
            interactable.hoverEntered.AddListener(HandleHoverEntered); // Corrected to only add listener to hoverEntered
        }
    }
    public void SetHandMode(bool isRightHand)
    {
        isRightHandMode = isRightHand;
        Debug.Log("Hand mode set to: " + (isRightHand ? "Right" : "Left"));
    }

    private void HandleHoverEntered(HoverEnterEventArgs arg)
    {
        if (arg.interactorObject is XRBaseInteractor interactor)
        {
            string interactorName = interactor.name;
            Debug.Log("Interactor Name: " + interactorName);

            // Check if the interactor belongs to the selected hand mode
            if (IsInteractorFromActiveHand(interactorName))
            {
                if (Time.time - lastSentTime > sendInterval)
                {
                    lastSentTime = Time.time; // Update last sent time

                    byte[] intensities = DetermineVibrationIntensities(interactorName);
                    SendVibrationCommand(intensities);
                }
            }
        }
    }

    private bool IsInteractorFromActiveHand(string interactorName)
    {
        if (isRightHandMode)
        {
            return interactorName.Contains("Right"); // Assuming interactor names include 'Right' for right hand
        }
        else
        {
            return interactorName.Contains("Left"); // Assuming interactor names include 'Left' for left hand
        }
    }

    private byte[] DetermineVibrationIntensities(string interactorName)
    {
        byte[] intensities = new byte[3]; // Initialize intensities array

        // Determine finger and assign the corresponding intensity based on hand mode
        if (interactorName.Contains("Index"))
        {
            intensities[1] = 255; // Index finger vibration intensity
        }
        else if (interactorName.Contains("Middle"))
        {
            intensities[isRightHandMode ? 0 : 2] = 255; // Middle finger vibration intensity swapped based on hand mode
        }
        else if (interactorName.Contains("Thumb"))
        {
            intensities[isRightHandMode ? 2 : 0] = 255; // Thumb finger vibration intensity swapped based on hand mode
        }
        return intensities;
    }

    private void SendVibrationCommand(byte[] intensities)
    {
        if (webSocketManager != null && webSocketManager.ws != null && webSocketManager.ws.IsAlive)
        {
            float sendTime = Time.realtimeSinceStartup;
            webSocketManager.ws.SendAsync(intensities, completed =>
            {
                if (completed)
                {
                    float receivedTime = Time.realtimeSinceStartup;
                    float latency = receivedTime - sendTime;
                    Debug.Log($"Vibration data sent successfully. Latency: {latency * 1000f} ms");
                }
                else
                {
                    Debug.LogError("Failed to send vibration data.");
                }
            });
        }
        else
        {
            Debug.LogError("WebSocket is not connected or WebSocketManager is not found.");
        }
    }

    private void OnDestroy()
    {
        // Clean up event listener
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(HandleHoverEntered);
        }
    }
}
