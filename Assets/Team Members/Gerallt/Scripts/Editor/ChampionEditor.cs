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
                if (GUILayout.Button("Level Up!"))
                {
                    CharacterBase character = target as CharacterBase;
                
                    if (character != null)
                    {
                        character.LevelUp(character.Level + 1, LevelingManager.Instance.maxLevels);
                    }
                }
            }
            
            DrawDefaultInspector();
        }

    }
}
    
