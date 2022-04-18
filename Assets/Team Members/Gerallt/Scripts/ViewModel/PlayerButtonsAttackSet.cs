using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChainsOfFate.Gerallt
{
    public class PlayerButtonsAttackSet : MonoBehaviour
    {
        public GameObject view;
        public GameObject weaponViewPrefab;
        public float itemSpacing = 60.0f;
        public float itemOffset = 0.0f;
        
        public PlayerButtons PlayerButtonsParentView;

        public void WeaponButton_OnClick(WeaponBase weapon)
        {
            CombatGameManager combatGameManager = CombatGameManager.Instance;
            
            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();
            IAttackAction attackAction = (IAttackAction)currentCharacter;

            if (attackAction != null)
            {
                CharacterBase target = combatGameManager.attackTarget;
                
                attackAction.Attack(target, weapon); 
                combatGameManager.RaiseAttackEvent(currentCharacter, target);
                
                combatGameManager.FinishedTurn(currentCharacter);
            }
        }

        public void BackButton_OnClick()
        {
            this.gameObject.SetActive(false);
            PlayerButtonsParentView.view.SetActive(true);
        }

        public void PopulateAttacks()
        {
            CombatGameManager combatGameManager = CombatGameManager.Instance;

            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();

            ClearView();

            if (currentCharacter != null)
            {
                int i = 0;
                foreach (WeaponBase weapon in currentCharacter.availableWeapons)
                {
                    GameObject weaponUIInstance = Instantiate(weaponViewPrefab, view.transform);
                    Vector3 pos = weaponUIInstance.transform.localPosition;

                    pos.x = (i * itemSpacing) + itemOffset;
                    pos.y = 0;
                    pos.z = 0;
                
                    weaponUIInstance.transform.localPosition = pos;

                    weaponUIInstance.GetComponentInChildren<TextMeshProUGUI>().text = weapon.GetName();
                    weaponUIInstance.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        WeaponButton_OnClick(weapon);
                    });
                    i++;
                }
            }
        }

        public void ClearView()
        {
            for (int i = 0; i < view.transform.childCount; i++)
            {
                Transform child = view.transform.GetChild(i);
                
                DestroyImmediate(child.gameObject);
            }
        }
        
        public void OnEnable()
        {

        }

        public void OnDisable()
        {
            
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