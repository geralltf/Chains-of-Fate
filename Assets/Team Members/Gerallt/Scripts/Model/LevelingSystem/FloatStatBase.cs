using System;
using DG.Tweening;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public abstract class FloatStatBase : MonoBehaviour, IStat
    {
        [SerializeField] private float value;
        public float minValue;
        
        [Tooltip("The maximum value you see in the UI")]
        public float maxValue;
        
        [Tooltip("The absolute maximum value this stat can be leveled up to")]
        public float absoluteMax;
        
        public float defaultValue;
        
        public AnimationCurve levelingCurve;

        protected float DefaultMaxValue;
        private CharacterBase owner;
        
        public event Action<float, FloatStatBase> OnValueChanged;
        
        public string StatName
        {
            get => this.name;
        }
        
        public float Value
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
        
        public void RaiseOnValueChanged(float newValue)
        {
            OnValueChanged?.Invoke(newValue, this);
        }
        
        public virtual bool LevelUp(int newLevel, int maxLevels, bool debugOutput = false)
        {
            bool hasChanged = LevelUp(newLevel / (float) maxLevels, debugOutput);
            
            if (hasChanged && debugOutput)
            {
                Debug.Log("Level " + newLevel + "/" + maxLevels + " [" + StatName + "] " + maxValue + "/" + absoluteMax);
            }

            return hasChanged;
        }
        
        public virtual bool LevelUp(float ratio, bool debugOutput = false)
        {
            float animatedValue = levelingCurve.Evaluate(ratio);

            float range = minValue + (animatedValue * absoluteMax);
            
            // Leveling up changes only the maximum value
            float oldValue = maxValue;
            
            maxValue += range;
            
            if (maxValue > absoluteMax)
            {
                maxValue = absoluteMax;
            }
            
            bool hasChanged = !Mathf.Approximately(oldValue, maxValue);

            return hasChanged;
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