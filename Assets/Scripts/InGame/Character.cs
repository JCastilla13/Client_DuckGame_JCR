using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Character : MonoBehaviourPun, IPunObservable
{
    private float speed;
    private float jumpForce;
    private Rigidbody2D rb;
    public SpriteRenderer sr;
    public Animator anim;
    private PhotonView pv;
    private Vector3 enemyPosition = Vector3.zero;

    private static float PlayerSpeed;
    private static float PlayerJumpForce;

    private float timeToShoot = 1f;
    private float lastTimeShoot;
    private bool cooldownReady = false;

    private int initHp = 3;
    private int playerHp = 3;

    private int playerLifes = 3;

    [SerializeField] private TextMesh playerNameText;
    
    [SerializeField] private TextMesh playerHpText;
    [SerializeField] private TextMesh playerLifesText;

    [SerializeField] private GameObject bulletSpawner;

    [SerializeField] private AudioSource shootCollisionSound;
    [SerializeField] private AudioSource shootFinalCollisionSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pv = rb.GetComponent<PhotonView>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        //Tasa de envio para la sincronizacion de red
        PhotonNetwork.SendRate = 20;
        //Tasa de serializacion para la sincronizacion de red
        PhotonNetwork.SerializationRate = 20;

        //Obtiene los valores de speed y jumpforce
        speed = PlayerPrefs.GetFloat("speed");
        jumpForce = PlayerPrefs.GetFloat("jumpforce");

        //Determinamos el color del jugador dependiendo del numero de jugador local
        int localPlayerNum = PhotonNetwork.LocalPlayer.ActorNumber;

        if (localPlayerNum == 1)
        {
            if (pv.IsMine)
            {
                sr.color = Color.yellow;
            }
            else
            {
                sr.color = Color.red;
            }
        }
        else
        {
            if (pv.IsMine)
            {
                sr.color = Color.red;
            }
            else
            {
                sr.color = Color.yellow;
            }
        }
    }

    private void Start()
    {
        //Asigna los valores de speed y jumpforce a las variables de instancia
        speed = PlayerSpeed;
        jumpForce = PlayerJumpForce;

        //Mostramos el nombre del jugador
        playerNameText.text = PhotonNetwork.CurrentRoom.Players[photonView.OwnerActorNr].NickName;
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            CheckInputs(); //Revisamos los inputs
            //Manejo de cooldown del disparo de bala
            if (cooldownReady)
            {
                lastTimeShoot -= Time.deltaTime;
            }
            if (lastTimeShoot < 0)
            {
                cooldownReady = false;
                lastTimeShoot = 0;
            }
        }
        else
        {
            //Replicamos la posicion del jugador enemigo de forma suave
            SmoothReplicate();
        }

        //UI de vida de los jugadores
        playerLifesText.text = "Lifes: " + playerLifes.ToString();
        playerHpText.text = "Health: " + playerHp.ToString();

    }

    private void FixedUpdate()
    {
        if (pv.IsMine)
        {
            //Obtenemos el input horizontal del jugador
            float horizontalInput = Input_Manager._INPUT_MANAGER.GetLeftAxisUpdate().x;

            //Se calcula la velocidad horizontal del jugador mediante los inputs y velocidad del jugador
            float horizontalVelocity = horizontalInput * speed * Time.fixedDeltaTime;

            //Aplicamos la velocidad horizontal al rigidbody para poder movernos
            rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);

            //Flipeamos el sprite del jugador y el spawn de la bala dependiendo de la direccion en la que mire
            if (horizontalVelocity > 0)
            {
                sr.flipX = false;
                bulletSpawner.transform.localPosition = new Vector3(1f, 0f, 0f);
            }
            else if (horizontalVelocity < 0)
            {
                sr.flipX = true;
                bulletSpawner.transform.localPosition = new Vector3(-1f, 0f, 0f);
            }

            //Animacion de movimiento del jugador
            anim.SetBool("IsMoving", horizontalInput != 0);
        }
    }

    //Verificamos los inputs del jugador
    private void CheckInputs()
    {
        //Verificamos si el personaje pulsa el boton de salto y esta en el suelo
        if (Input_Manager._INPUT_MANAGER.GetJumpButton() && Mathf.Approximately(rb.velocity.y, 0f))
        {
            //Saltamos aplicando fuerza y hacemos animacion de salto
            rb.AddForce(new Vector2(0f, jumpForce));
            anim.SetBool("IsJumping", true);
        }

        //Verificamos si el jugador esta cayendo
        if (rb.velocity.y < 0)
        {
            //Desactivamos la animacion de salto y activamos la de caida
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", true);
        }
        else
        {
            //Si deja de caer desactivamos la animacion
            anim.SetBool("IsFalling", false);
        }

        //Revisamos si pulsamos el boton de disparar y si el cooldown ha terminado
        if (Input_Manager._INPUT_MANAGER.GetShootButton() && lastTimeShoot <= 0 && !cooldownReady)
        {
            //Disparamos y manejamos el cooldown
            Shoot();
            lastTimeShoot = timeToShoot;
            cooldownReady = true;
        }
    }

    //Funcion de disparo
    private void Shoot()
    {
        //Instanciamos la bala en una posicion
        GameObject bullet = PhotonNetwork.Instantiate("Bullet", bulletSpawner.transform.position, Quaternion.identity);
        //Obtenemos referencia al script de la bala
        Bullet bulletRef = bullet.GetComponent<Bullet>();

        //Flipeamos la direccion de la bala dependiendo de la direccion en la que mire
        if (sr.flipX)
        {
            bulletRef.SetDirection(false);
        }
        else
        {
            bulletRef.SetDirection(true);
        }
    }

    //Funcion para recibir daño
    public void Damage()
    {
        //Si no eres el jugador local hacemos daño en la red
        if (!pv.IsMine)
        {
            pv.RPC("NetworkDamage", RpcTarget.All);
        }
    }

    //Funcion RPC para recibir daño en la red
    [PunRPC]
    private void NetworkDamage()
    {
        //Reducimos puntos de vida
        playerHp--;

        //Revisamos si la vida ha bajado a cero
        if (playerHp <= 0)
        {
            //Si los corazones se han terminado el juego termina y vuelves al matchmaking, si no se resta un corazon y reinicia la vida, el jugador derribado vuelve a aparecer
            //en el centro del mapa, tambien aparecen sonidos dependiendo de si pierdes vida o corazones
            if (playerLifes <= 0)
            {
                PhotonNetwork.LoadLevel("Matchmaking");
            }
            else
            {
                playerLifes--;
                playerHp = initHp;
                shootFinalCollisionSound.Play();
                transform.position = Vector3.zero;
            }
        }
        else
        {
            shootCollisionSound.Play();
        }
    }

    //Funcion para revisar la posicion del jugador enemigo de manera suave
    private void SmoothReplicate()
    {
        transform.position = Vector3.Lerp(transform.position, enemyPosition, Time.fixedDeltaTime * 20);
    }

    //Funcion para la sincronizacion de la red  
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Enviamos la posicion del jugador a traves de la red
            stream.SendNext(transform.position);
        }
        else if (stream.IsReading)
        {
            //Recibimos la posicion del enemigo a traves de la red
            enemyPosition = (Vector3)stream.ReceiveNext();
        }
    }

    //Sets de las variables de speed y jumpforce para poder setearlas en otros scripts

    public static void SetPlayerSpeed(float speed)
    {
        PlayerSpeed = speed;
    }

    public static void SetPlayerJumpForce(float jumpForce)
    {
        PlayerJumpForce = jumpForce;
    }
}
