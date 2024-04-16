using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnPlayer("Player1Race", spawnPoint1.position);
        }
        else
        {
            SpawnPlayer("Player2Race", spawnPoint2.position);
        }
    }

    public GameObject SpawnPlayer(string race, Vector3 spawnPoint)
    {
        return PhotonNetwork.Instantiate("Player", spawnPoint, Quaternion.identity);
    }
}
