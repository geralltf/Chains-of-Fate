using UnityEditor;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    [CustomEditor(typeof(Champion))]
    public class ChampionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                CharacterBase character = target as CharacterBase;

                if (character != null)
                {
                    if (GUILayout.Button("Level Up!"))
                    {
                        character.LevelUp(character.Level + 1, LevelingManager.Instance.maxLevels, true, false);
                    }
                    
                    if (GUILayout.Button("Simulate All Level Ups!"))
                    {
                        //for (int level = character.Level + 1; ; level++)
                        int savedLevel = character.Level;
                        int level = character.Level;
                        while(level < LevelingManager.Instance.maxLevels)
                        {
                            character.LevelUp(level + 1, LevelingManager.Instance.maxLevels, true, false);
                            
                            level = character.Level;
                        }

                        // Reset level and stats back to default values:
                        character.Level = savedLevel;
                        character.ResetStats();
                    }
                    
                    if (GUILayout.Button("Replenish stats!"))
                    {
                        character.ReplenishStats();
                    }
                }
            }

            DrawDefaultInspector();
        }
    }
}