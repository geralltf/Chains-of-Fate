using UnityEditor;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    [CustomEditor(typeof(Grunt))]
    public class GruntEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                Grunt character = target as Grunt;

                if (character != null)
                {
                    if (GUILayout.Button("Level Up!"))
                    {
                        character.LevelUp(character.Level + 1, LevelingManager.Instance.maxLevels);
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
                }
            }

            DrawDefaultInspector();
        }
    }
}