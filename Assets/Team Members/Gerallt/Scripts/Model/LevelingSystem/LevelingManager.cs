using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class LevelingManager : SingletonBase<LevelingManager>
    {
        [Tooltip("The maximum number of levels the leveling manager will level up a character to")]
        public int maxLevels = 100;
        
        public event LevelUpDelegate OnLevelUpEvent;
        public delegate void LevelUpDelegate(CharacterBase character, int oldLevel, int newLevel, int maxLevel);

        public void LevelUp(CharacterBase character)
        {
            int oldLevel = character.Level;
            int newLevel = (character.Level + 1);

            if (newLevel > maxLevels)
            {
                //newLevel = maxLevels;
                // No change, so no need to run OnLevelUpEvent
            }
            else
            {
                // Level Up the character. 
                // The character itself knows how to level up its own stats and knows to call RaiseLevelUp()
                character.LevelUp(newLevel, maxLevels);
            }
        }

        public void RaiseLevelUp(CharacterBase character, int oldLevel, int newLevel, int maxLevel)
        {
            OnLevelUpEvent?.Invoke(character, oldLevel, newLevel, maxLevels);
        }
    }
}