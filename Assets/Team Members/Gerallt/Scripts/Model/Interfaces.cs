using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public interface IFleeAction
    {
        /// <summary>
        /// If the character is able to flee.
        /// (Character usually rolls a dice to determine if to flee)
        /// </summary>
        /// <returns></returns>
        bool Flee();
    }

    public interface IAttackAction
    {
        void Attack(CharacterBase target); // TODO: Supply equipped weapon WeaponBase
    }

    public interface IResolveAction
    {
        void Encourage(CharacterBase targetCharacter);
        void Taunt(CharacterBase targetCharacter);
    }

    public interface IDefendAction
    {
        void Defend();
    }
}
