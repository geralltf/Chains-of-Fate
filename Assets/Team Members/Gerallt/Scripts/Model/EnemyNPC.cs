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
        public bool canFlee = false;
        
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
            CharacterBase target = champions[Random.Range(0, champions.Count)];
            CharacterBase agentTarget = agents[Random.Range(0, agents.Count)];
            
            EnemyState newState = (EnemyState)Random.Range(1, (int)EnemyState.Count);
            canFlee = false;
            
            switch (newState)
            {
                case EnemyState.Defend:
                    Defend();
                    break;
                case EnemyState.Flee:
                    canFlee = Flee();
                    break;
                case EnemyState.Attack:
                    Attack(target);
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
        public void Defend()
        {
            Debug.Log("ENEMY Test defend action");
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

        public void Attack(CharacterBase target) // TODO: need to pass in equipped weapon WeaponBase
        {
            Debug.Log("ENEMY Test attack action");
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