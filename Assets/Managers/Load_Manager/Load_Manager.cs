using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Load_Manager : MonoBehaviour
{
    [SerializeField] GameObject GameManagerObject;

    private float timeToLoadingScreen = 3f;

    private void Update()
    {
        timeToLoadingScreen -= Time.deltaTime;

        if (timeToLoadingScreen < 0 && this.gameObject != null)
        {
            GameManagerObject.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}
