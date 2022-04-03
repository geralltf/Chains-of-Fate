using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class BlockBarUI : MonoBehaviour
    {
        public GameObject view;
        public Transform shieldStart;
        public Transform shieldEnd;
        public BarShield shieldSprite;
        public float animationSpeed = 0.1f;
        public float selectionTime = 5.0f;
        public float guessCooldown = 1.5f;
        public bool animating = false;
        public CharacterBase defendingCharacter;
        public CharacterBase attackingCharacter;
        public event Action onWonEvent;
        public event Action onLostEvent;

        private TweenerCore<Vector3, Vector3, VectorOptions> currentTween;
        
        public void SetVisibility(bool visibility)
        {
            if (view != null)
            {
                if (visibility)
                {
                    StartAnim();
                }
                else
                {
                    StopAnim();
                }
                
                view.SetActive(visibility);
            }
        }

        public void Guess_ButtonClicked()
        {
            BarHitSquare hit = shieldSprite.currentTarget;
            
            if (hit != null)
            {
                Debug.Log("guess hit! block " + hit.defenseBlockPerentage.ToString() + "%");
            }
        }
        
        public void ResetAnim()
        {
            shieldSprite.transform.position = shieldStart.transform.position;
        }

        public void StartAnim()
        {
            StartAnim_LeftToRight();
        }
        
        public void StartAnim_LeftToRight()
        {
            animating = true;
            ResetAnim();
            
            float time = selectionTime * animationSpeed * 0.5f;
                
            currentTween = shieldSprite.transform.DOMove(shieldEnd.transform.position, time, false);
            currentTween.onComplete = () =>
            {
                StartAnim_RightToLeft();
                    
                //Debug.Log("anim left to right done");
            };
        }
        
        public void StartAnim_RightToLeft()
        {
            float time = selectionTime * animationSpeed * 0.5f;
                
            currentTween = shieldSprite.transform.DOMove(shieldStart.transform.position, time, false);
            currentTween.onComplete = () =>
            {
                StartAnim_LeftToRight();
                    
                //Debug.Log("anim right to left done");
            };
        }
        
        public void StopAnim()
        {
            if (currentTween != null)
            {
                currentTween.Kill();
            }
            
            animating = false;
            ResetAnim();
        }
        
        private void OnEnable()
        {
            StartAnim();
        }

        private void OnDisable()
        {
            StopAnim();
        }
    }
}
