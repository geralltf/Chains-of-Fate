using System;
using DG.Tweening;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public abstract class IntStatBase : MonoBehaviour, IStat
    {
        [SerializeField] private int value;
        public int minValue;
        public int maxValue;
        public int defaultValue;
        public AnimationCurve levelingCurve;
        public event Action<int, IntStatBase> OnValueChanged;

        public int Value
        {
            get => value;
            set
            {
                RaiseOnValueChanged(value);
                this.value = value;
            }
        }
        
        public void RaiseOnValueChanged(int newValue)
        {
            OnValueChanged?.Invoke(newValue, this);
        }
        
        public virtual void LevelUp(int newLevel, int maxLevels)
        {
            LevelUp(newLevel / (float) maxLevels);
        }
        
        public virtual void LevelUp(float ratio)
        {
            float animatedValue = levelingCurve.Evaluate(ratio);
            
            float range = minValue + (animatedValue * maxValue);
            
            value = (int) range;
        }

        public virtual void Awake()
        {
            value = defaultValue;
        }
    }
}