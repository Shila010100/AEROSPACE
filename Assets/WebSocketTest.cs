using UnityEngine;
using WebSocketSharp;

public class WebSocketTest : MonoBehaviour
{
    public string serverAddress = "ws://192.168.178.42:80/"; // Replace with your WebSocket server address
    private WebSocket ws;

    private void Start()
    {
        Debug.Log("Testing WebSocket request");
        ConnectWebSocket();
    }

    private void ConnectWebSocket()
    {
        ws = new WebSocket(serverAddress);

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connected");
            SendVibrationCommand();
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

        try
        {
            Debug.Log("Attempting to connect to WebSocket server at " + serverAddress);
            ws.Connect();
            Debug.Log("WebSocket Connect called");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("WebSocket connection failed: " + ex.Message);
        }
    }

    private void SendVibrationCommand()
    {
        if (ws != null && ws.IsAlive)
        {
            byte[] intensities = { 255, 100, 100 }; // Example intensities
            ws.Send(intensities);
            Debug.Log("Sent binary message to server: " + string.Join(",", intensities));
        }
        else
        {
            Debug.LogError("WebSocket is not connected");
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
