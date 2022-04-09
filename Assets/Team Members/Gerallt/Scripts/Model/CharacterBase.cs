using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public abstract class CharacterBase : MonoBehaviour
    {
        public int hp = 100;
        public int arcana = 100;
        public int resolve = 0;
        public int defense = 0;
        public int wisdom = 0;
        public int speed = 0;
        public int strength = 0;
        public float movementSpeed = 1.0f;
        public int level = 0;
    }
}