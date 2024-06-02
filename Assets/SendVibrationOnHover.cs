using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Threading.Tasks;

public class SendVibrationOnHover : MonoBehaviour
{
    private WebSocketConnectionManager webSocketManager;
    private float lastSentTime = 0.0f;
    private float sendInterval = 0.017f; // Interval in seconds, adjust as needed

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

    private void HandleHoverEntered(HoverEnterEventArgs arg)
    {
        if (arg.interactorObject is XRBaseInteractor interactor)
        {
            string interactorName = interactor.name;
            Debug.Log("Interactor Name: " + interactorName);

            if (Time.time - lastSentTime > sendInterval)
            {
                lastSentTime = Time.time; // Update last sent time

                if (interactorName.Contains("Index")) // Assuming naming includes the finger name
                {
                    SendVibrationCommand(new byte[] { 0, 255, 0 }); // Index finger
                }
                else if (interactorName.Contains("Middle"))
                {
                    SendVibrationCommand(new byte[] { 255, 0, 0 }); // Middle finger
                }
                else if (interactorName.Contains("Thumb"))
                {
                    SendVibrationCommand(new byte[] { 0, 0, 255 }); // Thumb
                }
            }
        }
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
