using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using UnityEngine;

public class Grunt : EnemyNPC
{
    /// <summary>
    /// Grunts have a ‘speed type’ that represents how likely it is that they avoid damage.
    /// 40% (Normal)
    /// 25%
    /// 15%
    /// 10%
    /// 7%
    /// 3%
    /// </summary>
    public enum SpeedType
    {
        StNormal = 40, // 40%
        St25Percent = 25, // 25%
        St15Percent = 15, // 15%
        St10Percent = 10, // 10%
        St07Percent = 7, // 7%
        St03Percent = 3  // 3%
    }
    
    /// <summary>
    /// Grunts have a ‘speed type’ that represents how likely it is that they avoid damage.
    /// 40% (Normal)
    /// 25%
    /// 15%
    /// 10%
    /// 7%
    /// 3%
    /// </summary>
    public SpeedType speedType = SpeedType.StNormal;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
