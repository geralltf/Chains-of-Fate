using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class DefenseStat : IntStatBase
    {
        [SerializeField] private string statName = "DEF";
        
        public new string StatName => statName;
    }
}