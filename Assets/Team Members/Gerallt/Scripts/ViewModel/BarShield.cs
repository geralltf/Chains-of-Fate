using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class BarShield : MonoBehaviour
    {
        public BlockBarUI ParentView;

        //public event Action onHitSquare;

        public BarHitSquare currentTarget = null;
        
        private void OnCollisionEnter(Collision collision)
        {
            GameObject go = collision.gameObject;
            BarHitSquare hit = go.GetComponent<BarHitSquare>();

            if (hit != null)
            {
                currentTarget = hit;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            GameObject go = other.gameObject;
            BarHitSquare hit = go.GetComponent<BarHitSquare>();

            if (hit != null)
            {
                currentTarget = null;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            transform.rotation = quaternion.identity; //HACK
        }
    }

}