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
        
        public void RaiseOnValueChanged(float newValue)
        {
            OnValueChanged?.Invoke(newValue, this);
        }
        
        public virtual bool LevelUp(int newLevel, int maxLevels)
        {
            return LevelUp(newLevel / (float) maxLevels);
        }
        
        public virtual bool LevelUp(float ratio)
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
            
            return !Mathf.Approximately(oldValue, maxValue);
        }

        public virtual void Reset()
        {
            maxValue = DefaultMaxValue;
        }
        
        public virtual void Awake()
        {
            value = defaultValue;
            DefaultMaxValue = maxValue;
        }
    }
}