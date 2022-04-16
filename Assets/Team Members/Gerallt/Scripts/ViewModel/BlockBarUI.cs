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
        public CombatUI parentView;
        public GameObject view;
        public Transform shieldStart;
        public Transform shieldEnd;
        public BarShield shieldSprite;
        public float animationSpeed = 0.1f;
        public float selectionTime = 5.0f;
        public float loseTime = 10.0f;
        public float guessCooldown = 1.5f;
        public bool animating = false;
        public CharacterBase defendingCharacter;

        public delegate void WonDelegate(float blockPercentage, bool doCounterAttack);
        
        public event WonDelegate onWonEvent;
        public event Action onLostEvent;

        public AnimationCurve animationCurve;

        private TweenerCore<Vector3, Vector3, VectorOptions> currentTween;
        
        public void SetVisibility(bool visibility)
        {
            if (view != null)
            {
                Camera _camera = FindObjectOfType<Camera>();
                Vector3 screenCenter = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, _camera.nearClipPlane);
                Vector3 offset = _camera.ScreenToWorldPoint(screenCenter);
                offset.z = 0.1f;
                gameObject.transform.position = offset;
                var angles = gameObject.transform.eulerAngles;
                angles.x = 90;
                gameObject.transform.eulerAngles = angles;
                
                if (visibility)
                {
                    StartAnim();

                    StartCoroutine(TimeUntilLost());
                }
                else
                {
                    StopAnim();
                }
                
                view.SetActive(visibility);
            }
        }

        private IEnumerator TimeUntilLost()
        {
            yield return new WaitForSeconds(loseTime);

            StopAnim();
            
            onLostEvent?.Invoke();
        }

        public void Guess_ButtonClicked()
        {
            BarHitSquare hit = shieldSprite.currentTarget;
            
            if (hit != null)
            {
                Debug.Log("guess hit! block " + hit.defenseBlockPerentage.ToString() + "%" + (hit.isCounterAttack?"Counter attack" : ""));
                
                onWonEvent?.Invoke(hit.defenseBlockPerentage, hit.isCounterAttack);
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
                
            currentTween = shieldSprite.transform.DOLocalMove(shieldEnd.transform.localPosition, time, false);
            currentTween.onComplete = () =>
            {
                StartAnim_RightToLeft();
                    
                //Debug.Log("anim left to right done");
            };
            currentTween.SetEase(animationCurve);
        }
        
        public void StartAnim_RightToLeft()
        {
            float time = selectionTime * animationSpeed * 0.5f;
                
            currentTween = shieldSprite.transform.DOLocalMove(shieldStart.transform.localPosition, time, false);
            currentTween.onComplete = () =>
            {
                StartAnim_LeftToRight();
                    
                //Debug.Log("anim right to left done");
            };
            currentTween.SetEase(animationCurve);
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
