using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class CombatUI : MonoBehaviour
    {
        public CombatGameManager combatGameManager;
        public GameObject view;
        public BlockBarUI blockBarUI;

        public event Action<CombatUI> onSceneDestroyed;
        public event Action onSceneLoaded;
        public bool isLoaded;
        
        [Header("For Game Testing, add your own party members and enemies here")]
        public bool isTestMode; // If test mode, will use the test party members
        public List<GameObject> testEnemies;
        public List<GameObject> testPartyMembers;
        public GameObject testPlayer;

        private void Awake()
        {
            onSceneLoaded?.Invoke();
        }

        private void OnDestroy()
        {
            onSceneDestroyed?.Invoke(this);
        }

        private void Start()
        {
            if (isTestMode)
            {
                SetCurrentParty(testEnemies, testPartyMembers, testPlayer);
            }
        }
        
        public void SetCurrentParty(List<GameObject> enemies, List<GameObject> partyMembers, GameObject currentPlayer)
        {
            Debug.Log("Got list of current enemies, party members, and current player");

            combatGameManager.SetUpQueue(enemies, partyMembers, currentPlayer);
        }
    }

}
