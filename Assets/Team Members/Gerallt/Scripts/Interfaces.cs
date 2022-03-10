using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public interface IFleeable
    {
        void Flee();
    }

    public interface IAttacking
    {
        void Attack(CharacterBase opposingCharacter);
    }

    public interface IResolvable
    {
        void Encourage(CharacterBase targetCharacter);
        void Taunt(CharacterBase targetCharacter);
    }
}
