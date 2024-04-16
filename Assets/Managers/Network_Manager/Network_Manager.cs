using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using Photon.Pun;

public class Network_Manager : MonoBehaviour
{
    public static Network_Manager _NETWORK_MANAGER;

    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    private bool connected = false;

    const string host = "192.168.1.149";
    const int port = 6543;

    [SerializeField] private GameObject registerScreen;
    [SerializeField] private GameObject loginScreen;
    [SerializeField] private GameObject matchmakingScreen;

    [SerializeField] private GameObject registerError;
    [SerializeField] private GameObject loginError;

    private void Awake()
    {
        if (_NETWORK_MANAGER != null && _NETWORK_MANAGER != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _NETWORK_MANAGER = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void ManageData(string data)
    {
        if (data == "Ping")
        {
            Debug.Log("Recibo ping");
            writer.WriteLine("1");
            writer.Flush();
        }
    }

    private void Update()
    {
        if (connected)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                {
                    ManageData(data);
                }
            }
        }
    }

    public void ConnectToServerForLogin(string nick, string password)
    {
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            connected = true;

            writer.WriteLine("LOGIN/" + nick + "/" + password);
            writer.Flush();

            string response = reader.ReadLine();
            if (response.StartsWith("true"))
            {
                PhotonNetwork.LocalPlayer.NickName = nick;
                Debug.Log("Login exitoso.");
                string[] parts = response.Split('/');
                if (parts.Length >= 3)
                {
                    float speed = float.Parse(parts[1]);
                    float jumpForce = float.Parse(parts[2]);

                    //Almacena los valores de speed y jumpforce en PlayerPrefs
                    Character.SetPlayerSpeed(speed);
                    Character.SetPlayerJumpForce(jumpForce);
                }
                else
                {
                    Debug.Log("La respuesta del servidor no contiene los valores de speed y jumpforce.");
                }

                registerScreen.SetActive(false);
                loginScreen.SetActive(false);
                matchmakingScreen.SetActive(true);
                loginError.SetActive(false);
            }
            else if (response == "false")
            {
                loginError.SetActive(true);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
    void OnApplicationQuit()
    {
        if (connected)
        {
            writer.WriteLine("QUIT/" + PhotonNetwork.LocalPlayer.NickName);
            writer.Flush();
        }
    }

    public void ConnectToServerForRegister(string nick, string password, string race)
    {
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            connected = true;

            writer.WriteLine("REGISTER/" + nick + "/" + password + "/" + race);
            writer.Flush();

            string response = reader.ReadLine();
            if (response.StartsWith("true"))
            {
                Debug.Log("Nuevo registro");
                registerError.SetActive(false);

                //Extrae los valores de speed y jumpforce del mensaje
                string[] parts = response.Split('/');
                float speed = float.Parse(parts[1]);
                float jumpforce = float.Parse(parts[2]);

                //Almacena los valores de speed y jumpforce en PlayerPrefs
                PlayerPrefs.SetFloat("speed", speed);
                PlayerPrefs.SetFloat("jumpforce", jumpforce);
            }
            else if (response == "false")
            {
                Debug.Log("El usuario ya existe.");
                registerError.SetActive(true);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
}
