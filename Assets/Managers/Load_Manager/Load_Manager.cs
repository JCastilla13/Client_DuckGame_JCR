using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Load_Manager : MonoBehaviour
{
    [SerializeField] GameObject GameManagerObject;

    //Tiempo en que se muestra la pantalla de carga
    private float timeToLoadingScreen = 3f;

    private void Update()
    {
        timeToLoadingScreen -= Time.deltaTime;

        //Revisamos si el tiempo a terminado y si este objeto sigue estando
        if (timeToLoadingScreen < 0 && this.gameObject != null)
        {
            //Activamos el game manager que se encarga de spawnear los jugadores y destruimos este objeto
            GameManagerObject.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}
