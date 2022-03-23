using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class PlayerButtonsAttackSet : MonoBehaviour
    {
        public PlayerButtons PlayerButtonsParentView;

        public void SwordButton_OnClick()
        {
            Debug.Log("Sword attack");
        }
        
        public void DaggerButton_OnClick()
        {
            Debug.Log("Dagger attack");
        }
        
        public void BackButton_OnClick()
        {
            this.gameObject.SetActive(false);
            PlayerButtonsParentView.gameObject.SetActive(true);
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