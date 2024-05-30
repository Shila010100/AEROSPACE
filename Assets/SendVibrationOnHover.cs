using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SendVibrationOnHover : MonoBehaviour
{
    private WebSocketConnectionManager webSocketManager;

    void Start()
    {
        // Find the WebSocketConnectionManager in the scene
        webSocketManager = FindObjectOfType<WebSocketConnectionManager>();

        // Get the XRBaseInteractable component and subscribe to the hover event
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(HandleHoverEntered);
        }
    }

    private void HandleHoverEntered(HoverEnterEventArgs arg)
    {
        SendVibrationCommand();
    }

    private void SendVibrationCommand()
    {
        if (webSocketManager != null && webSocketManager.ws != null && webSocketManager.ws.IsAlive)
        {
            byte[] intensities = { 255, 100, 100 }; // Example intensities
            webSocketManager.ws.Send(intensities);
            Debug.Log("Sent vibration command: " + string.Join(",", intensities));
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