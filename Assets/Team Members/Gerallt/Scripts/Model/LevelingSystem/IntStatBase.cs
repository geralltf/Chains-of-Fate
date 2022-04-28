using System;
using DG.Tweening;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public abstract class IntStatBase : MonoBehaviour, IStat
    {
        [SerializeField] private int value;
        public int minValue;
        
        [Tooltip("The maximum value you see in the UI")]
        public int maxValue;
        
        [Tooltip("The absolute maximum value this stat can be leveled up to")]
        public int absoluteMax;
        
        public int defaultValue;
        
        public AnimationCurve levelingCurve;

        protected int DefaultMaxValue;
        private CharacterBase owner;
        
        public event Action<int, IntStatBase> OnValueChanged;

        public string StatName
        {
            get => this.name;
        }

        public int Value
        {
            get => value;
            set
            {
                RaiseOnValueChanged(value);
                this.value = value;
            }
        }
        
        public object GetMaximum()
        {
            return maxValue;
        }

        public object GetAbsoluteMaximum()
        {
            return absoluteMax;
        }

        public CharacterBase GetOwner()
        {
            if (owner == null)
            {
                Transform curr = transform;
                bool searching = true;
                while (curr != null && searching)
                {
                    CharacterBase characterBase = curr.GetComponent<CharacterBase>();

                    if (characterBase != null)
                    {
                        owner = characterBase;
                        searching = false;
                    }
                    else
                    {
                        curr = curr.parent;
                    }
                }
            }
            return owner;
        }
        
        public void RaiseOnValueChanged(int newValue)
        {
            OnValueChanged?.Invoke(newValue, this);
        }
        
        public virtual bool LevelUp(int newLevel, int maxLevels, bool debugOutput = false)
        {
            return LevelUp(newLevel / (float) maxLevels, debugOutput);
        }
        
        public virtual bool LevelUp(float ratio, bool debugOutput = false)
        {
            float animatedValue = levelingCurve.Evaluate(ratio);
            
            float range = minValue + (animatedValue * absoluteMax);

            // Leveling up changes only the maximum value
            int oldValue = maxValue;
            
            maxValue += (int) range;

            if (maxValue > absoluteMax)
            {
                maxValue = absoluteMax;
            }
            
            return oldValue != maxValue;
        }

        public virtual void Reset()
        {
            maxValue = DefaultMaxValue;
        }

        public virtual void Replenish()
        {
            value = maxValue;
        }
        
        public virtual void Awake()
        {
            value = defaultValue;
            DefaultMaxValue = maxValue;
        }
    }
}