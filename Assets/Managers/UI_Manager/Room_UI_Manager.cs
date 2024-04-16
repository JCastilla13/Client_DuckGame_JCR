using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room_UI_Manager : MonoBehaviour
{
    [SerializeField]
    private Button createButton;

    [SerializeField]
    private Button joinButton;

    [SerializeField]
    private Text createText;

    [SerializeField]
    private Text joinText;

    [SerializeField]
    private GameObject matchmakingUI;

    [SerializeField]
    private GameObject loadingScreen;

    private void Awake()
    {
        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);
    }

    //Funcion que crea salas
    private void CreateRoom()
    {
        Photon_Manager._PHOTON_MANAGER.CreateRoom(createText.text.ToString());
        matchmakingUI.SetActive(false);
        loadingScreen.SetActive(true);
    }

    //Funcion que une a salas
    private void JoinRoom()
    {
        Photon_Manager._PHOTON_MANAGER.JoinRoom(joinText.text.ToString());
    }
}
