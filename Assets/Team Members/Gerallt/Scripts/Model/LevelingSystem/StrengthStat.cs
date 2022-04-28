using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class StrengthStat : IntStatBase
    {
        [SerializeField] private string statName = "Strength";
        
        public new string StatName => statName;
    }
}