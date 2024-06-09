using UnityEngine;
using System.Collections.Concurrent;
using System.Threading;

public class CommandManager : MonoBehaviour
{
    public static CommandManager Instance { get; private set; }
    private ConcurrentQueue<byte[]> commandQueue = new ConcurrentQueue<byte[]>();
    private Thread commandThread;
    private volatile bool isRunning = true;

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
        commandThread = new Thread(ProcessCommands);
        commandThread.Start();
    }

    private void ProcessCommands()
    {
        Debug.Log("Command processing thread started.");
        while (isRunning)
        {
            if (commandQueue.TryDequeue(out byte[] command))
            {
                if (WebSocketConnectionManager.Instance != null && WebSocketConnectionManager.Instance.ws.IsAlive)
                {
                    WebSocketConnectionManager.Instance.SendCommand(command); // Directly send command using WebSocket manager
                    Debug.Log("Processing a dequeued command.");
                }
                else
                {
                    Debug.LogError("WebSocket not available or disconnected.");
                }
            }
            Thread.Sleep(1); // Manage CPU usage
        }
    }

    public void EnqueueCommand(byte[] command)
    {
        float enqueueTime = Time.realtimeSinceStartup;
        commandQueue.Enqueue(command);
        Debug.Log($"Command enqueued at {enqueueTime * 1000f} ms.");
    }

    void OnDestroy()
    {
        isRunning = false;
        if (commandThread != null)
        {
            commandThread.Join();
            Debug.Log("Command processing thread stopped.");
        }
    }
}
