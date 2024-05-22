using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using WebSocketSharp;

public class WebSocketTest : MonoBehaviour
{
    private string _serverAddress = "ws://192.168.178.42:81/"; // Update with your ESP32 IP address
    private WebSocket ws;
    private XRSimpleInteractable simpleInteractable;

    // Public property to access and modify serverAddress in the Unity Editor
    public string ServerAddress
    {
        get { return _serverAddress; }
        set { _serverAddress = value; }
    }

    private void Start()
    {
        Debug.Log("Testing WebSocket request");
        ConnectWebSocket();
        SetupXRInteraction();
    }

    private void ConnectWebSocket()
    {
        ws = new WebSocket(_serverAddress);

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connected");
        };

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Received message from server: " + e.Data);
        };

        ws.OnError += (sender, e) =>
        {
            Debug.LogError("WebSocket error: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket closed");
        };

        ws.Connect();
    }

    private void SetupXRInteraction()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();

        // Subscribe to poke events
        simpleInteractable.onSelectEntered.AddListener(OnPokeEnter);
        simpleInteractable.onSelectExited.AddListener(OnPokeExit);
    }

    private void OnPokeEnter(XRBaseInteractor interactor)
    {
        SendVibrationCommand();
    }

    private void OnPokeExit(XRBaseInteractor interactor)
    {
        // No action needed when poke interaction ends
    }

    public void SendVibrationCommand()
    {
        if (ws != null && ws.IsAlive)
        {
            byte[] intensities = { 255, 0, 0 }; // Example intensities
            ws.Send(intensities);
            Debug.Log("Sent binary message to server: " + string.Join(",", intensities));
        }
    }

    private void OnDestroy()
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }
    }
}
