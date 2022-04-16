using System.Collections;
using TMPro;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        public GameObject levelLoadingIndicatorUI;
        public TextMeshProUGUI levelLoadingUIText;
        public float levelLoadingTime = 2.0f;

        private bool shownIndicator = false;


        public void ShowLevelLoadingIndicator(string sceneName)
        {
            levelLoadingIndicatorUI.SetActive(true);
            levelLoadingUIText.text = sceneName + "...";
        }
        
        public void HideLevelLoadingIndicator()
        {
            if (!shownIndicator)
            {
                StartCoroutine(HideIndicator());
            }
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