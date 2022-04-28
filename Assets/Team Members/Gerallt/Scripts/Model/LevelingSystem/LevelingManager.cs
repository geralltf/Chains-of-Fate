using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class LevelingManager : SingletonBase<LevelingManager>
    {
        [Tooltip("The maximum number of levels the leveling manager will level up a character to")]
        public int maxLevels = 100;

        [Tooltip("The time it takes before the leveling up dialog disappears")]
        public float dialogShowTime = 2.0f;

        [SerializeField] private GameObject view;
        [SerializeField] private TextMeshProUGUI textMeshProUGUINewLevelText;
        [SerializeField] private string newLevelUpTextFormat = "{0} Level up from {1} to {2} level. Max level = {3}";
        private bool currentlyLevelingUp = false;
        
        public event LevelUpDelegate OnLevelUpEvent;
        public delegate void LevelUpDelegate(CharacterBase character, int oldLevel, int newLevel, int maxLevel, List<IStat> statsAffected);

        public void LevelUp(CharacterBase character, int numDefeatedEnemies, int totalEnemiesInArea)
        {
            //int oldLevel = character.Level;
            int newLevel = (character.Level + 1);

            if (newLevel > maxLevels)
            {
                //newLevel = maxLevels;
                // At max level so no change. No need to run OnLevelUpEvent
            }
            else
            {
                // Level Up the character. 
                // The character itself knows how to level up its own stats and knows to call RaiseLevelUp()
                character.LevelUp(newLevel, maxLevels);
            }
        }

        public void RaiseLevelUp(CharacterBase character, int oldLevel, int newLevel, int maxLevel, List<IStat> statsAffected)
        {
            OnLevelUpEvent?.Invoke(character, oldLevel, newLevel, maxLevels, statsAffected);
        }

        public void SetVisibility(bool visibility)
        {
            view.SetActive(visibility);
        }

        public override void Awake()
        {
            base.Awake();
            
            OnLevelUpEvent += OnOnLevelUpEvent;

            SetVisibility(false);
        }

        private void OnOnLevelUpEvent(CharacterBase character, int oldLevel, int newLevel, int maxLevel, List<IStat> statsAffected)
        {
            // Show level up for main character only.
            CharacterBase mainCharacter = GameManager.Instance.GetMainCharacter();

            if (character.ID == mainCharacter.ID)
            {
                if (!currentlyLevelingUp)
                {
                    StartCoroutine(LevelUpCoroutine(character, oldLevel, newLevel, maxLevel, statsAffected));
                }
            }
        }

        private IEnumerator LevelUpCoroutine(CharacterBase character, int oldLevel, int newLevel, int maxLevel, List<IStat> statsAffected)
        {
            currentlyLevelingUp = true;
            SetVisibility(true);
            textMeshProUGUINewLevelText.text = string.Format(newLevelUpTextFormat, character.name, oldLevel, newLevel, maxLevel);
            foreach (IStat stat in statsAffected)
            {
                object newMaximum = stat.GetMaximum();
                object absoluteMax = stat.GetAbsoluteMaximum();
                
                textMeshProUGUINewLevelText.text += "\n >" + stat.StatName + " max is now " + newMaximum + " out of " + absoluteMax;
            }
            yield return new WaitForSeconds(dialogShowTime);
            SetVisibility(false);
            currentlyLevelingUp = false;
        }
    }
}