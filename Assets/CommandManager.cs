using UnityEngine;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;

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
        Debug.Log("CM: CommandManager thread started.");
    }

    private void ProcessCommands()
    {
        //Debug.Log("CM: Command processing thread started.");
        while (isRunning)
        {
            if (commandQueue.TryDequeue(out byte[] command))
            {
                if (command.Any(i => i > 0))  // Check if there's any intensity above zero
                {
                    if (WebSocketConnectionManager.Instance != null && WebSocketConnectionManager.Instance.ws.IsAlive)
                    {
                        WebSocketConnectionManager.Instance.SendCommand(command); // Directly send command using WebSocket manager
                        //Debug.Log($"CM: Processing a dequeued command with intensities: {string.Join(", ", command)}");
                    }
                    else
                    {
                        //Debug.LogError("CM: WebSocket not available or disconnected.");
                    }
                }
                else
                {
                    //Debug.Log("CM: Dequeued command with no intensity or failed to dequeue.");
                }
            }
            Thread.Sleep(1); // Manage CPU usage
        }
    }



    public void EnqueueCommand(byte[] command)
    {
        float enqueueTime = Time.realtimeSinceStartup;
        commandQueue.Enqueue(command);
        //Debug.Log($"CM: Command enqueued at {enqueueTime * 1000f} ms with intensities: {string.Join(", ", command)}");  // Ensure this logs correct values
    }

    void OnDestroy()
    {
        isRunning = false;
        commandThread.Join(); // Ensure the thread is properly closed
        Debug.Log("CM: Command processing thread terminated.");
    }
}
