using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;

public class TestNetworkRequest : MonoBehaviour
{
    public string deviceIP = "192.168.4.1"; // The IP address of the M5StickC Plus for WiFi Direct
    public int port = 8080;

    private void Start()
    {
        Debug.Log("Testing network request");
        SendVibrationCommand(new byte[] { 255, 0, 0 });
    }

    public void SendVibrationCommand(byte[] intensities)
    {
        string message = $"GET /vibrate?intensities={intensities[0]},{intensities[1]},{intensities[2]} HTTP/1.1\r\nHost: {deviceIP}:{port}\r\nConnection: close\r\n\r\n";
        Debug.Log("Sending message: " + message); // Log the message

        try
        {
            Debug.Log("Attempting to connect to " + deviceIP + " on port " + port);
            using (TcpClient client = new TcpClient(deviceIP, port))
            {
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);
                Debug.Log("Message sent to server");

                byte[] responseData = new byte[256];
                int bytes = stream.Read(responseData, 0, responseData.Length);
                string response = Encoding.ASCII.GetString(responseData, 0, bytes);
                Debug.Log("Received response: " + response);
            }
        }
        catch (SocketException ex)
        {
            Debug.LogError("SocketException: " + ex.Message + "\n" + ex.StackTrace);
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex.Message + "\n" + ex.StackTrace);
        }
    }
}
