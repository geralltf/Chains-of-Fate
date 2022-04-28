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
        
        public void RaiseOnValueChanged(int newValue)
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
            int oldValue = maxValue;
            
            maxValue += (int) range;

            return oldValue != maxValue;
        }

        public virtual void Awake()
        {
            value = defaultValue;
        }
    }
}