using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class ArcanaStat : IntStatBase
    {
        [SerializeField] private string statName = "ARC";
        
        public new string StatName => statName;
    }
}