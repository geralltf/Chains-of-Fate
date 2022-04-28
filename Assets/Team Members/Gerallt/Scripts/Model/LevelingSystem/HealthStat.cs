using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class HealthStat : IntStatBase
    {
        [SerializeField] private string statName = "HP";
        
        public new string StatName => statName;
    }
}