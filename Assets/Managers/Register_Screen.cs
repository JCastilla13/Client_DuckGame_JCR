using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Register_Screen : MonoBehaviour
{
    [SerializeField] private Button registerButton;
    [SerializeField] private Text registerText;
    [SerializeField] private Text passwordText;

    [SerializeField] private Dropdown raceDropdown;

    private void Awake()
    {
        //Definimos listener
        registerButton.onClick.AddListener(Clicked);
    }

    private void Clicked()
    {
        //Obtenemos la raza seleccionada del dropdown
        string race = raceDropdown.options[raceDropdown.value].text;
        //Se envia la informacion al Network Manager para registrarnos
        Network_Manager._NETWORK_MANAGER.ConnectToServerForRegister(registerText.text.ToString(), passwordText.text.ToString(), race);
    }

}

