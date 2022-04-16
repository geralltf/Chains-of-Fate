using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(FindObjectOfType<ChainsOfFate.Gerallt.GameManager>().gameObject);
        DontDestroyOnLoad(FindObjectOfType<PlayerController>().gameObject);
        DontDestroyOnLoad(FindObjectOfType<CameraFollow>().gameObject);

        SceneManager.LoadScene("Main");
    }
}
