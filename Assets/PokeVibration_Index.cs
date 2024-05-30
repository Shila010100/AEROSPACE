using UnityEngine;
using WebSocketSharp;

public class PokeVibration_Index : MonoBehaviour
{
    public string serverAddress = "ws://192.168.178.42:80/"; // Replace with your WebSocket server address
    private WebSocket ws;

    void Start()
    {
        
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
