using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public abstract class CharacterBase : MonoBehaviour
    {
        public int HP = 100;
        public int Arcana = 0;
        public int Resolve = 0;
        public int Defense = 0;
        public int Wisdom = 0;
        public int Speed = 0;
        public int Strength = 0;
        public float MovementSpeed = 1.0f;
        public int Level = 0;

        //public event OnAttacked // TODO: If the character is attacked, update its stats.
        //public event OnHadTurn // TODO: If a character has a turn, propagate that change to the CombatGameManager 

        public virtual void LevelUp()
        {
            
        }
        
        public virtual void HaveTurn()
        {
            
        }
    }
}