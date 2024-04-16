using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login_Screen : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    [SerializeField] private Text loginText;
    [SerializeField] private Text passwordText;

    private void Awake()
    {
        //Definimos listener
        loginButton.onClick.AddListener(Clicked);    
    }

    private void Clicked()
    {
        //Mando la info al network manager
        Network_Manager._NETWORK_MANAGER.ConnectToServerForLogin(loginText.text.ToString(), passwordText.text.ToString());
    }
}

