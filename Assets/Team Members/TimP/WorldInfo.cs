using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldInfo : MonoBehaviour
{
    private Transform player;
    private Vector3 centrePoint;
    public float loadRange = 40;
    public string leftScene, rightScene, upScene, downScene;
    public bool leftSceneLoaded, rightSceneLoaded, upSceneLoaded, downSceneLoaded;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        centrePoint = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckPosition();
    }

    void CheckPosition()
    {
        if (player.hasChanged)
        {
            if (player.position.x > centrePoint.x + loadRange)
            {
                if (player.position.x > centrePoint.x + (loadRange * 2))
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                }

                if (!rightSceneLoaded)
                {
                    if (rightSceneLoaded != true)
                    {
                        rightSceneLoaded = true;
                        if (rightScene != null) SceneManager.LoadScene(rightScene, LoadSceneMode.Additive);
                    }
                }
            }

            if (player.position.x < centrePoint.x - loadRange)
            {
                if (player.position.x < centrePoint.x - (loadRange * 2))
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                }

                if (!leftSceneLoaded && leftSceneLoaded != true)
                {
                    leftSceneLoaded = true;
                    if (leftScene != null) SceneManager.LoadScene(leftScene, LoadSceneMode.Additive);
                }
            }

            if (player.position.y > centrePoint.y + loadRange)
            {
                if (player.position.y > centrePoint.y + (loadRange * 2))
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                }

                if (!downSceneLoaded && downSceneLoaded != true)
                {
                    downSceneLoaded = true;
                    if (rightScene != null) SceneManager.LoadScene(downScene, LoadSceneMode.Additive);
                }
            }

            if (player.position.y < centrePoint.y - loadRange)
            {
                if (player.position.y < centrePoint.y - (loadRange * 2))
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                }

                if (!upSceneLoaded && upSceneLoaded != true)
                {
                    upSceneLoaded = true;
                    if (leftScene != null) SceneManager.LoadScene(upScene, LoadSceneMode.Additive);
                }
            }

        }
    }
}
