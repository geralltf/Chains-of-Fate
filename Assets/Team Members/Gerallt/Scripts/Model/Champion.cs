using System;
using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ChainsOfFate.Gerallt
{
    public class Champion : PlayableCharacter, IFleeAction, IAttackAction, IResolveAction, IDefendAction
    {
        /// <summary>
        /// If the Champion is the main character.
        /// </summary>
        public bool isMainCharacter;

        /// <summary>
        /// Defend against the specified attacker.
        /// </summary>
        /// <param name="attacker">
        /// The attacker to defend against.
        /// </param>
        public void Defend(float blockPercentage)
        {
            Debug.Log("Test defend action, block " + blockPercentage.ToString() + "%");

            DefenceMove move = new DefenceMove();
            move.blockPercentage = blockPercentage;

            ApplyMove(move); // Schedule to apply the move later on when this character is attacked.
        }

        /// <summary>
        /// Checks if the champion can flee by doing a random dice roll.
        /// </summary>
        /// <returns>
        /// True, if allowed to flee.
        /// </returns>
        public bool Flee()
        {
            Debug.Log("Test flee action");

            return Random.value > 0.5f;
        }

        public void Attack(CharacterBase target, CombatGameManager combatGameManager) // TODO: need to pass in equipped weapon WeaponBase
        {
            if (target == null) // TODO: Implement target selection
            {
                // HACK: Just pick a random enemy for now
                var enemies = combatGameManager.turnsQueue.GetEnemies();
                target = enemies[Random.Range(0, enemies.Count -1)];
            }

            float weaponBaseDamage = 1.0f; // TODO: Get this from equipped weapon WeaponBase
            Debug.Log("Test attack action");

            // Calculate defense of target
            int numDefenseMoves = 0;
            float totalDefense = 0;
            List<AppliedMove> moves = target.GetMoves();
            if (moves.Count > 0)
            {
                for (int i = 0; i < moves.Count; i++)
                {
                    AppliedMove move = moves[i];
                    if (move is DefenceMove defenceMove)
                    {
                        numDefenseMoves++;
                        totalDefense += defenceMove.blockPercentage;
                    }
                }
            }

            target.ClearMoves(); // We have applied all the moves of the target.
            
            if (totalDefense == 0)
            {
                totalDefense = 1.0f; // Don't allow divide by zero.
            }

            Debug.Log("target has " + numDefenseMoves.ToString() + " defense moves, total block = " +
                      totalDefense.ToString());

            // Calculate damage.
            // Weapon Base Damage x (STR/DEF) = Total Damage
            // Where STR is the attackers strength score
            // Where DEF is the targets defense score

            int totalDamage = (int)(weaponBaseDamage * ((float)this.Strength / totalDefense));
            
            // APPLY DAMAGE to target
            target.ApplyDamage(totalDamage);
            
            // Raise Counter Attack event for the specified target, so the target can apply a counter attack.
            combatGameManager.RaiseCounterAttackEvent(this, target);
        }

        public void Encourage(CharacterBase targetCharacter)
        {
            Debug.Log("Test encourage action");
        }

        public void Taunt(CharacterBase targetCharacter)
        {
            Debug.Log("Test taunt action");
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}