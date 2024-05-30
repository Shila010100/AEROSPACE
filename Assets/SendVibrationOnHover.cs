using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SendVibrationOnHover : MonoBehaviour
{
    private WebSocketConnectionManager webSocketManager;
    private float lastSentTime = 0.0f;
    private float sendInterval = 0.5f; // Interval in seconds, adjust as needed

    void Start()
    {
        // Find the WebSocketConnectionManager in the scene
        webSocketManager = FindObjectOfType<WebSocketConnectionManager>();

        // Get the XRBaseInteractable component and subscribe to the hover event
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(HandleHoverEntered); // Ensure no duplicates
            interactable.hoverEntered.AddListener(HandleHoverEntered);
        }
    }

    private void HandleHoverEntered(HoverEnterEventArgs arg)
    {
        // Check if enough time has elapsed since the last sent vibration command
        if (Time.time - lastSentTime > sendInterval)
        {
            lastSentTime = Time.time; // Update last sent time
            SendVibrationCommand();
        }
    }

    private void SendVibrationCommand()
    {
        if (webSocketManager != null && webSocketManager.ws != null && webSocketManager.ws.IsAlive)
        {
            byte[] intensities = { 255, 100, 100 }; // Example intensities
            webSocketManager.ws.SendAsync(intensities, completed =>
            {
                if (completed)
                {
                    Debug.Log("Vibration data sent successfully: " + string.Join(",", intensities));
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