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
        public SpriteRenderer SceneBackgroundSpriteRenderer;

        public event Action<CombatUI> onSceneDestroyed;

        public event Action<CombatUI> onCloseCombatUI;
        
        public event Action onSceneLoaded;
        public bool isLoaded;
        
        [Header("For Game Testing, add your own party members and enemies here")]
        public bool isTestMode; // If test mode, will use the test party members
        public List<GameObject> testEnemies;
        public List<GameObject> testPartyMembers;
        public GameObject testPlayer;

        public void RaiseCloseCombatUI()
        {
            onCloseCombatUI?.Invoke(this);
        }
        
        private void Awake()
        {
            combatGameManager.OnFleeEvent += CombatGameManagerOnOnFleeEvent;
            onSceneLoaded?.Invoke();
        }

        private void CombatGameManagerOnOnFleeEvent(CharacterBase current, bool canFlee, bool unloadCombatUI)
        {
            if (canFlee && unloadCombatUI)
            {
                RaiseCloseCombatUI();
            }
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

            if (!isTestMode)
            {
                CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
                //Camera _camera = cameraFollow.GetComponent<Camera>();
                Vector3 offset = cameraFollow.GetCenterWorldPosition();

                GameObject sceneBg = SceneBackgroundSpriteRenderer.gameObject;
            
                sceneBg.transform.position = offset;
                sceneBg.transform.rotation = cameraFollow.transform.rotation;
            }

            combatGameManager.SetUpQueue(enemies, partyMembers, currentPlayer);
        }
    }

}
