using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    private void Awake()
    {
        //Verificamos si es el cliente que crea la sala, es decir el cliente master
        if (PhotonNetwork.IsMasterClient)
        {
            //Spawneamos al jugador 1 si es el master client
            SpawnPlayer("Player1Race", spawnPoint1.position);
        }
        else
        {
            //Spawneamos al jugador 2 si no es el master client
            SpawnPlayer("Player2Race", spawnPoint2.position);
        }
    }

    public GameObject SpawnPlayer(string race, Vector3 spawnPoint)
    {
        //Devolvemos la instancia al jugador en la posicion de un objeto
        return PhotonNetwork.Instantiate("Player", spawnPoint, Quaternion.identity);
    }
}
