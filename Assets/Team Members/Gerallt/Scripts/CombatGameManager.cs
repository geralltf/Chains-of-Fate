using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ChainsOfFate.Gerallt
{
    public class CombatGameManager : MonoBehaviour
    {
        /// <summary>
        /// The delay the agent waits before completing their turn.
        /// </summary>
        public float havingTurnDelaySeconds = 2.0f;
        public CharacterBase ActiveCharacter => GetCurrentCharacter();
        public CharacterBase attackTarget = null;
        public PriorityQueue turnsQueue;
        public bool shuffleTurns = false;
        public bool makeDeterministic = false;
        public int seed;
        
        public delegate void ActionableDelegate(CharacterBase current, CharacterBase target);
        public delegate void ResolveDelegate(CharacterBase current, CharacterBase target, bool encourage, bool taunt);
        public delegate void FleeDelegate(CharacterBase current);
        public delegate void ManagerInitilisedQueueDelegate(int enemiesAllocated, int partyMembersAllocated);

        public event ManagerInitilisedQueueDelegate OnManagerInitilisedQueueEvent;
        public event Action<CharacterBase> OnChampionHavingNextTurn;
        public event Action<EnemyNPC> OnEnemyHavingTurn;
        public event Action<EnemyNPC> OnEnemyCompletedTurn;
        
        public event Action<CharacterBase> OnHavingTurn;
        public event Action<CharacterBase, CharacterBase> OnTurnCompleted;
        
        public event CharacterBase.StatChangeDelegate OnStatChanged;
        public event ActionableDelegate OnDefendEvent;
        public event ActionableDelegate OnAttackEvent;
        public event ResolveDelegate OnResolveEncourageEvent;
        public event ResolveDelegate OnResolveTauntEvent;
        public event FleeDelegate OnFleeEvent;

        private void OnDestroy()
        {
            // Cleanup.
            var queue = turnsQueue.ToList();

            foreach (CharacterBase character in queue)
            {
                character.OnStatChanged -= Character_OnStatChanged;
            }
        }

        /// <summary>
        /// Sets up the turns queue using existing enemies, player, and party members.
        /// If shuffleTurns is enabled, these lists are shuffled randomly to make the game interesting
        ///
        /// Precondition: Enemies and Party Members must have a CharacterBase component in order for the scheduling to work. 
        /// </summary>
        public void SetUpQueue(List<GameObject> currentEnemies, List<GameObject> partyMembers, GameObject currentPlayer)
        {
            // Unsubscribe from old queue.
            var queueOld = turnsQueue.ToList();
            foreach (CharacterBase character in queueOld)
            {
                character.OnStatChanged -= Character_OnStatChanged;
            }
            turnsQueue.Clear();

            // Player always initially goes first.
            CharacterBase playerCharacter = currentPlayer.GetComponent<CharacterBase>();
            turnsQueue.Enqueue(playerCharacter);

            // Evenly distribute turns but enemy goes first.
            int i = 0;
            GameObject go;
            CharacterBase characterBase;
            bool allocatedEnemies = false;
            bool allocatedParty = false;
            bool distributingTurns = true;
            int enemiesAllocated = 0;
            int partyMembersAllocated = 0;

            if (shuffleTurns)
            {
                if (makeDeterministic)
                {
                    Random.InitState(seed); //TODO: This probably should be put in some general GameManager not here
                }
                ShuffleList(ref currentEnemies);
                ShuffleList(ref partyMembers);
            }
            
            while (distributingTurns)
            {
                if (i < currentEnemies.Count && currentEnemies.Count > 0)
                {
                    go = currentEnemies[i];
                    characterBase = go.GetComponent<CharacterBase>();

                    if (characterBase != null)
                    {
                        enemiesAllocated++;
                        
                        turnsQueue.Enqueue(characterBase);
                    }
                }
                else
                {
                    allocatedEnemies = true;
                }
                
                if (i < partyMembers.Count && partyMembers.Count > 0)
                {
                    go = partyMembers[i];
                    characterBase = go.GetComponent<CharacterBase>();
                    
                    if (characterBase != null)
                    {
                        partyMembersAllocated++;
                        
                        turnsQueue.Enqueue(characterBase);
                    }
                }
                else
                {
                    allocatedParty = true;
                }

                if (allocatedEnemies && allocatedParty)
                {
                    // Completed allocating turns.
                    distributingTurns = false;
                }
                i++;
            }

            // Subscribe to character stat changes. 
            var queue = turnsQueue.ToList();
            foreach (CharacterBase character in queue)
            {
                character.OnStatChanged += Character_OnStatChanged;
            }
            
            OnManagerInitilisedQueueEvent?.Invoke(enemiesAllocated, partyMembersAllocated);
            OnHavingTurn?.Invoke(playerCharacter);
        }
        
        

        public void FinishedTurn(CharacterBase character, bool skipToNextChallenger = false, bool skipToNextChampion = false)
        {
            CharacterBase oldTop = turnsQueue.Top();

            if (skipToNextChallenger || skipToNextChampion)
            {
                if (skipToNextChallenger)
                {
                    Debug.Log("Skipping to next challenger");
                
                    // Next challenger/enemy takes a turn via skipToNextChallenger
                    turnsQueue.SkipTo(chr=> chr is EnemyNPC);
                }

                if (skipToNextChampion)
                {
                    Debug.Log("Skipping to next champion");
                
                    // Next champion takes a turn via skipToNextChampion
                    turnsQueue.SkipTo(chr=> chr is Champion);
                }
            }
            else
            {
                // Remove the current character from the top of the queue and adds it back in at the end.
                turnsQueue.Dequeue();
            }
            
            turnsQueue.Sort();

            // SANITY CHECK
            if (turnsQueue.Top() == oldTop)
            {
                // Can't have another turn when character just had a turn.
                turnsQueue.queue.RemoveAt(0);

                if (turnsQueue.Count == 1)
                {
                    turnsQueue.queue.Add(oldTop);
                }
                else
                {
                    turnsQueue.queue.Insert(1, oldTop);
                }
            }
            
            CharacterBase currentCharacter = GetCurrentCharacter();
            OnTurnCompleted?.Invoke(character, currentCharacter);
            OnHavingTurn?.Invoke(currentCharacter);
            
            EnemyNPC agent = currentCharacter as EnemyNPC;

            if (agent != null)
            {
                // Agent.
                OnEnemyHavingTurn?.Invoke(agent);

                AgentHaveTurn(agent);
                
                // Agent later on calls FinishedTurn() again when it has finished internally.
            }
            else
            {
                // Champion.
                OnChampionHavingNextTurn?.Invoke(currentCharacter);
                
                Debug.Log("It's your turn!");
            }
        }

        private void AgentHaveTurn(EnemyNPC agent)
        {
            Debug.Log("Agent having a turn");

            agent.DecideMove(this);
        }
        
        public CharacterBase GetCurrentCharacter()
        {
            return turnsQueue.Top();
        }

        private void Character_OnStatChanged(CharacterBase character, string propertyName, object newValue)
        {
            OnStatChanged?.Invoke(character, propertyName, newValue);
        }
        
        public void RaiseEnemyCompletedTurn(EnemyNPC agent)
        {
            OnEnemyCompletedTurn?.Invoke(agent);
        }

        public void RaiseDefendEvent(CharacterBase current, CharacterBase target)
        {
            OnDefendEvent?.Invoke(current, target);
        }

        public void RaiseAttackEvent(CharacterBase current, CharacterBase target)
        {
            OnAttackEvent?.Invoke(current, target);
        }
        
        public void RaiseResolveEncourageEvent(CharacterBase current, CharacterBase target)
        {
            OnResolveEncourageEvent?.Invoke(current, target, true, false);
        }
        
        public void RaiseResolveTauntEvent(CharacterBase current, CharacterBase target)
        {
            OnResolveTauntEvent?.Invoke(current, target, false, true);
        }
        
        public void RaiseFleeEvent(CharacterBase current)
        {
            OnFleeEvent?.Invoke(current);
        }
        
        private static void ShuffleList<T>(ref List<T> list)  
        {  
            int count = list.Count;
            int newIndex;
            T tmp;
            
            while (count > 1) 
            {  
                count--;
                
                newIndex = Random.Range(0, count + 1);
                
                tmp = list[newIndex];
                list[newIndex] = list[count];
                list[count] = tmp;
            }
        }
    }
}