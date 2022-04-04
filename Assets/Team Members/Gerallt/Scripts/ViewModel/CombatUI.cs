using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class CombatUI : MonoBehaviour
    {
        public List<GameObject> CurrentEnemies;
        public event Action onSceneDestroyed;
        public event Action onSceneLoaded;
        public bool isLoaded;

        public void Awake()
        {
            if (onSceneLoaded != null)
            {
                onSceneLoaded();
            }
        }

        public void OnDestroy()
        {
            if (onSceneDestroyed !=null)
            {
                onSceneDestroyed();
            }
        }

        public void SetCurrentEnemies(List<GameObject> enemies)
        {
            CurrentEnemies = enemies;
            
            Debug.Log("Got list of current enemies");
        }
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }

}
