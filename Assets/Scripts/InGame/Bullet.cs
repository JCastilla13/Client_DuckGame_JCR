using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    private float speed = 10f;
    private Rigidbody2D rb;
    private bool characterLooksRight = true;

    private PhotonView pv; //Componente PhotonView para la sincronización de la red

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!characterLooksRight)
        {
            speed = -speed;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        rb.velocity = new Vector2(speed, 0f);
    }

    public void SetDirection(bool looksRight)
    {
        characterLooksRight = looksRight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Miramos si la bala instanciada es del jugador local
        if (!pv.IsMine)
        {
            return; //Salimos de la funcion si no es del jugador local
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Character>().Damage();
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
