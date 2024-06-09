using UnityEngine;
using WebSocketSharp;

public class WebSocketConnectionManager : MonoBehaviour
{
    public static WebSocketConnectionManager Instance { get; private set; }
    private string serverAddress = "ws://192.168.4.1:80/"; // Adjusted for AP mode
    public WebSocket ws;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ConnectWebSocket();
    }

    private void ConnectWebSocket()
    {
        ws = new WebSocket(serverAddress);
        ws.OnOpen += OnOpenHandler;
        ws.OnMessage += OnMessageHandler;
        ws.OnError += OnErrorHandler;
        ws.OnClose += OnCloseHandler;

        Debug.Log("WSCM: Attempting to connect to WebSocket server at " + serverAddress);
        ws.ConnectAsync(); // Use asynchronous connection to avoid blocking the main thread
    }

    public void SendCommand(byte[] command)
    {
        if (ws != null && ws.IsAlive)
        {
            ws.SendAsync(command, completed =>
            {
                if (!completed)
                {
                    // Handle failed send if necessary, but no logging
                }
            });
        }
        else
        {
            // Handle WebSocket not connected or alive if necessary, but no logging
        }
    }

    private void OnOpenHandler(object sender, System.EventArgs e)
    {
        Debug.Log("WSCM: WebSocket connected");
    }

    private void OnMessageHandler(object sender, MessageEventArgs e)
    {
        //Debug.Log("WSCM: Received message from server: " + e.Data);
    }

    private void OnErrorHandler(object sender, ErrorEventArgs e)
    {
        //Debug.LogError("WSCM: WebSocket error: " + e.Message);
        Invoke("ConnectWebSocket", 5); // 5-second delay before trying to reconnect
    }

    private void OnCloseHandler(object sender, CloseEventArgs e)
    {
        Debug.Log($"WSCM: WebSocket closed, reason: {e.Reason}");
        if (!e.WasClean) // Check if the close was unexpected
        {
            Debug.Log("WSCM: Unexpected disconnect. Attempting to reconnect...");
            ConnectWebSocket();
        }
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.CloseAsync(); // Asynchronously close the WebSocket connection
        }
    }
}
