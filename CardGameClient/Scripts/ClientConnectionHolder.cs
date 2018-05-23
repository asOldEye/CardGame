using UnityEngine;
using ConnectionProtocol;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

public class ClientConnectionHolder : MonoBehaviour
{
    [SerializeField] GameObject WrongConnection;
    [SerializeField] Text WrongConnectionMessage;

    static public ClientConnectionHolder ThisConnection { get; private set; }

    [SerializeField] public string address = "127.0.0.1";
    [SerializeField] public int port = 8888;
    [SerializeField] [Range(10, 10000)] public int bufferSize = 8192;
    ClientConnection connection;

    public bool Connected { get; private set; } = false;

    Task connect;

    public UnityEvent OnWrongConnection;
    public UnityEvent OnConnected;
    public UnityEvent OnDisconnected;

    public UnityAction<object> OnObjectReceived;

    public void Awake()
    {
        ThisConnection = this;
        connection = new ClientConnection(false, bufferSize);
    }

    public void Connect()
    {
        try
        {
            connect = Task.Run(() => new Action<string, int>(connection.Connect)(address, port));
        }
        catch (Exception e)
        {
            if (OnWrongConnection != null) OnWrongConnection.Invoke();
            WrongConnection.SetActive(true);
            WrongConnectionMessage.text = e.Message;
        }
    }
    private void FixedUpdate()
    {
        if (connect != null)
        {
            var e = connect.Exception;
            if (e != null)
            {
                if (OnWrongConnection != null) OnWrongConnection.Invoke();
                WrongConnection.SetActive(true);
                WrongConnectionMessage.text = e.InnerException.Message;
                connect = null;
            }
        }
        if (Connected != (Connected = connection.Connected))
        {
            if (Connected)
            {
                if (OnConnected != null)
                    OnConnected.Invoke();
            }
            else if (OnDisconnected != null)
                OnDisconnected.Invoke();
        }

        while (connection.ReceivedObjects.Count > 0)
            if (OnObjectReceived != null)
                OnObjectReceived.Invoke(connection.ReceivedObjects.Dequeue());
    }

    public void Send(object obj)
    {
        if (connection.Connected)
            connection.Send(obj);
    }
    public void Disconnect()
    {
        if (connection.Connected)
            connection.Disconnect();
    }
}