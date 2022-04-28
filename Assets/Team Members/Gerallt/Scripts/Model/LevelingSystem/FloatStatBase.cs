using System;
using DG.Tweening;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public abstract class FloatStatBase : MonoBehaviour, IStat
    {
        [SerializeField] private float value;
        public float minValue;
        public float maxValue;
        public int defaultValue;
        public AnimationCurve levelingCurve;
        public event Action<float, FloatStatBase> OnValueChanged;
        
        public float Value
        {
            get => value;
            set
            {
                RaiseOnValueChanged(value);
                this.value = value;
            }
        }
        
        public void RaiseOnValueChanged(float newValue)
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
            
            value = minValue + (animatedValue * maxValue);
        }
        
        public virtual void Awake()
        {
            value = defaultValue;
        }
    }
}