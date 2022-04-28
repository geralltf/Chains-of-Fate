using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class ResolveStat : IntStatBase
    {
        [SerializeField] private string statName = "RES";
        
        public new string StatName => statName;
    }
}