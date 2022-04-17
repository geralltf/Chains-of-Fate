using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ChainsOfFate.Gerallt
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public float outofboundsBounceForce = 10.0f;
        public float boundaryMinDistance = 10.0f;
        public float loadRange = 10.0f;
        public float unloadRange = 15.0f;
        public float boundaryRange = 3.0f;
        public float loadingAnimationSpeed = 1.0f;
        internal bool levelLoadingLock = false;
        
        public CombatUI combatUI;
        public GameObject levelLoadingIndicatorUI;
        public TextMeshProUGUI levelLoadingUIText;
        public float levelLoadingTime = 5.0f;

        private bool shownIndicator = false;
        private bool animatingIndicator = false;
        
        public void ShowCombatUI()
        {
            combatUI.gameObject.SetActive(true);
        }
        
        public void HideCombatUI()
        {
            combatUI.gameObject.SetActive(false);
        }

        public void ShowLevelLoadingIndicator(string sceneName)
        {
            levelLoadingIndicatorUI.SetActive(true);
            
            levelLoadingUIText.text = sceneName + "...";

            if (!animatingIndicator)
            {
                StartCoroutine(ShowIndicator());
            }
        }
        
        public void HideLevelLoadingIndicator()
        {
            if (!shownIndicator)
            {
                StartCoroutine(HideIndicator());
            }
        }
        
        IEnumerator ShowIndicator()
        {
            animatingIndicator = true;
            float alpha = 0;
            float maxAlpha = 1.0f;
            Image image = levelLoadingIndicatorUI.GetComponentInChildren<Image>();
            Color before = image.color;
            float time = 0;
            while (animatingIndicator)
            {
                alpha = Mathf.Lerp(alpha, maxAlpha, loadingAnimationSpeed * Time.deltaTime);
                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                image.material.color = image.color;
                
                //if (time >= loadingAnimationSpeed)
                if(alpha >= maxAlpha -0.1f)
                {
                    animatingIndicator = false;
                }

                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            image.color = before;
            image.material.color = image.color;
            //
        }
        
        IEnumerator HideIndicator()
        {
            shownIndicator = true;
            yield return new WaitForSeconds(levelLoadingTime);
            levelLoadingIndicatorUI.SetActive(false);
            shownIndicator = false;
        }
        
        public virtual void Awake()
        {
            Instance = this;
            
            levelLoadingIndicatorUI.SetActive(false);
        }
    }
}