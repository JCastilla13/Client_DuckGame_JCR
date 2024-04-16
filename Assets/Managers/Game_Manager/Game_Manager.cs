using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    private Color player2Color = Color.yellow;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnPlayer("Player1Race", spawnPoint1.position);
        }
        else
        {
            GameObject player2 = SpawnPlayer("Player2Race", spawnPoint2.position);
            Character character = player2.GetComponent<Character>();
            if (character != null)
            {
                character.SetPlayerColor(player2Color);
            }
        }
    }

    public GameObject SpawnPlayer(string race, Vector3 spawnPoint)
    {
        return PhotonNetwork.Instantiate("Player", spawnPoint, Quaternion.identity);
    }
}
