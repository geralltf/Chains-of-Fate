using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class PlayerButtonsInventorySet : MonoBehaviour
    {
        public PlayerButtons PlayerButtonsParentView;

        public void DamagingSpellButton_OnClick()
        {
            Debug.Log("Damaging spell");
        }
        
        public void BackButton_OnClick()
        {
            this.gameObject.SetActive(false);
            PlayerButtonsParentView.view.SetActive(true);
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