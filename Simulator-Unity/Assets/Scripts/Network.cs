using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.ComponentModel;

public class Network : MonoBehaviour
{
    private Socket _clientSocket;
    private Thread _receiveThread;
    private byte[] _buffer;

    [SerializeField]
    private string serverIP;
    [SerializeField]
    private int serverPort;
    // Use this for initialization
    void Start()
    {
        _receiveThread = new Thread(Receive);
        Thread.Sleep(1000); // a 1 second sleep, to give the unity engine time to get the netowrk server running before we try to connect
        BeginConnect();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void OnDestroy()
    {
        if (_clientSocket != null && _clientSocket.Connected)
        {
            _clientSocket.Close();
        }
    }

    private void BeginConnect()
    {
        try
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse(serverIP), serverPort), new AsyncCallback(ConnectCallback), null);
        }
        catch (Exception ex)
        {
            LoggingSystem.log.Error("Conneting to" + serverIP + ":" + serverPort + " failed!\n" + ex.Message);
        }
    }

    private void ConnectCallback(IAsyncResult AR)
    {
        try
        {
            _clientSocket.EndConnect(AR);
            AppendToTextBox("[Connected]");
            _receiveThread.Start();
        }
        catch (Exception ex)
        {
            LoggingSystem.log.Error("Conneting to" + serverIP + ":" + serverPort + " failed!\n" + ex.Message);
        }
        LoggingSystem.log.Info("Conneted, " + serverIP + ":" + serverPort);
    }
    private void Receive()
    {
        _buffer = new byte[_clientSocket.ReceiveBufferSize];
        _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), null);
    }

    private void RecieveCallback(IAsyncResult AR)
    {
        try
        {
            int received = _clientSocket.EndReceive(AR);

            if (received == 0)                                  //
            {                                                   //
                AppendToTextBox("[Disconnected from server]");  //
                return;                                         // if we recieve 0 bytes, assume the client has disconnected
            }                                                   //

            Array.Resize(ref _buffer, received);
            string text = Encoding.ASCII.GetString(_buffer);
            AppendToTextBox("[Server] " + text);
            Array.Resize(ref _buffer, _clientSocket.ReceiveBufferSize);
            _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), null);
        }
        catch (Exception ex)
        {
            LoggingSystem.log.Error(ex.Message);
        }
    }
    private void SendCallback(IAsyncResult AR)
    {
        try
        {
            _clientSocket.EndSend(AR);
        }
        catch (Exception ex)
        {
            LoggingSystem.log.Error(ex.Message);
        }
    }

    private void AppendToTextBox(string text)
    {
        //MethodInvoker invoker = new MethodInvoker(delegate
        //{
        //    clientText.Text += text + "\r\n";
        //});

        //this.Invoke(invoker);
    }

    public void SendToServer(string msg)
    {
        try
        {
            byte[] buffer = Encoding.ASCII.GetBytes(msg);
            _clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);

            if (buffer.Length > 0)
            {
                string text = Encoding.ASCII.GetString(buffer);
               // AppendToTextBox("[Client] " + text);
                //clientSendText.Text = "";
            }
        }
        catch (SocketException)
        {
            //AppendToTextBox("[Disconnected from server]");
            //clientSendText.Text = "";
            //UpdateControls(false);
        } // server closed
        catch (Exception ex)
        {
            LoggingSystem.log.Error(ex.Message);
        }
    }
}
