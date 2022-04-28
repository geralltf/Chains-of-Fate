using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class WisdomStat : IntStatBase
    {
        [SerializeField] private string statName = "WIS";
        
        public new string StatName => statName;
    }
}