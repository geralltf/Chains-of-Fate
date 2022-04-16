using System;
using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ChainsOfFate.Gerallt
{
    public class EnemyNPC : CharacterBase, IFleeAction, IAttackAction, IResolveAction, IDefendAction
    {
        public EnemyState appliedState = EnemyState.NotSet;
        private bool canFlee = false;
        
        public enum EnemyState : int
        {
            NotSet = 0,
            Defend = 1,
            Flee = 2,
            Attack = 3,
            ResolveEncourage = 4,
            ResolveTaunt = 5,
            Count = 6
        }
        
        /// <summary>
        /// Decides a new move to make randomly and applies the move.
        /// </summary>
        public void DecideMove(CombatGameManager combatGameManager)
        {
            List<CharacterBase> champions = combatGameManager.turnsQueue.GetChampions();
            List<CharacterBase> agents = combatGameManager.turnsQueue.GetEnemies();
            CharacterBase target = champions[Random.Range(0, champions.Count - 1)];
            CharacterBase agentTarget = agents[Random.Range(0, agents.Count - 1)];
            float blockPercentage = Random.Range(0, 100); //TODO: Vary agent aptitude at blocking by skill stat
            
            EnemyState newState = (EnemyState)Random.Range(1, (int)EnemyState.Count);
            canFlee = false;
            
            switch (newState)
            {
                case EnemyState.Defend:
                    // Defend without activating Quick Time Event / block bar.
                    Defend(blockPercentage);
                    
                    // TODO: Schedule a counter attack
                    break;
                case EnemyState.Flee:
                    canFlee = Flee();
                    break;
                case EnemyState.Attack:
                    Attack(target, combatGameManager);
                    break;
                case EnemyState.ResolveEncourage:
                    Encourage(agentTarget);
                    break;
                case EnemyState.ResolveTaunt:
                    Taunt(target);
                    break;
                case EnemyState.Count:
                    throw new Exception("Invalid state selected 'Count'");
                case EnemyState.NotSet:
                    throw new Exception("Invalid state selected 'NotSet'");
            }

            appliedState = newState;

            // COMPLETE.
            StartCoroutine(DelayCompleteTurn(combatGameManager));
        }
        
        private IEnumerator DelayCompleteTurn(CombatGameManager combatGameManager)
        {
            yield return new WaitForSeconds(combatGameManager.havingTurnDelaySeconds);

            // COMPLETED TURN.
            
            // Just raise the completed event immediately.
            combatGameManager.RaiseEnemyCompletedTurn(this);

            bool skipNext = (appliedState == EnemyState.Flee) && !canFlee;
            
            // Notify the game manager that this agent has completed it's turn.
            combatGameManager.FinishedTurn(this, false, skipNext);
        }
        
        /// <summary>
        /// Defend against the specified attacker.
        /// </summary>
        /// <param name="attacker">
        /// The attacker to defend against.
        /// </param>
        public void Defend(float blockPercentage)
        {
            Debug.Log("ENEMY Test defend action, block " + blockPercentage.ToString() + "%");
            
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
            Debug.Log("ENEMY Test flee action");

            return Random.value > 0.5f;
        }

        public void Attack(CharacterBase target, CombatGameManager combatGameManager) // TODO: need to pass in equipped weapon WeaponBase
        {
            float weaponBaseDamage = 1.0f; // TODO: Get this from equipped weapon WeaponBase
            Debug.Log("ENEMY Test attack action");

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
            Debug.Log("ENEMY Test encourage action");
        }

        public void Taunt(CharacterBase targetCharacter)
        {
            Debug.Log("ENEMY Test taunt action");
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