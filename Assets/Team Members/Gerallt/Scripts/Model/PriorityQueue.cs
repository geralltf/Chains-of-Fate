using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChainsOfFate.Gerallt;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    /// <summary>
    /// A circular queue that rotates a priority of characters based on their attributes.
    /// Dequeue never removes an item but usually reassigns it at the end of the queue.
    /// </summary>
    public class PriorityQueue : MonoBehaviour
    {
        [SerializeField] private List<CharacterBase> queue = new List<CharacterBase>();

        /// <summary>
        /// Characters that have had their turns for the current round.
        /// </summary>
        public List<CharacterBase> hadTurns = new List<CharacterBase>();

        public int Count => queue.Count;

        public CharacterBase Top()
        {
            return queue.FirstOrDefault();
        }

        public CharacterBase End()
        {
            return queue.LastOrDefault();
        }

        public CharacterBase Next()
        {
            if (Count < 2) return null;
            return queue[2];
        }

        public CharacterBase Dequeue()
        {
            var top = Top();
            if (top != null)
            {
                queue.RemoveAt(0);
                queue.Add(top); // Re-add character to end of queue.
            }

            return top;
        }

        public void Enqueue(CharacterBase characterBase)
        {
            queue.Add(characterBase);

            characterBase.OnStatChanged += CharacterBase_OnStatChanged;
        }

        public void RemoveTop()
        {
            queue.RemoveAt(0);
        }

        public void RemoveEnd()
        {
            queue.RemoveAt(Count - 1);
        }

        public void Add(CharacterBase newCharacter)
        {
            queue.Add(newCharacter);
        }

        public void InsertBeforeTop(CharacterBase newTop)
        {
            queue.Insert(0, newTop);
        }

        public void InsertAfterTop(CharacterBase second)
        {
            queue.Insert(1, second);
        }

        public void InsertBeforeEnd(CharacterBase secondLast)
        {
            queue.Insert(Count - 2, secondLast);
        }

        /// <summary>
        /// Skip all other characters until the predicate finds a character that meets the criteria to have its turn. 
        /// </summary>
        /// <param name="predicate"></param>
        public void SkipTo(Predicate<CharacterBase> predicate)
        {
            // Next challenger or champion takes a turn depending on the predicate by skipping to the next one later in the list 

            for (int i = 0; i < Count; i++)
            {
                CharacterBase character = queue[i];

                if (predicate(character) == false)
                {
                    // Reposition character to end of queue
                    queue.RemoveAt(i);
                    queue.Add(character); // Re-add character to end of queue.
                }
                else
                {
                    break;
                }
            }
        }

        public List<CharacterBase> GetChampions()
        {
            return queue.Where(chr => chr is Champion).ToList();
        }

        public List<CharacterBase> GetEnemies()
        {
            return queue.Where(chr => chr is EnemyNPC).ToList();
        }

        public void Sort(CharacterBase changedCharacter = null)
        {
            // TODO: if changedCharacter is specified it should be easier to sort

            //Sort all the characters by their speed priority.
            queue = queue.OrderByDescending(chr => chr.Speed).ToList();
        }

        public void SanityChecks(CharacterBase oldTop, CharacterBase oldEnd)
        {
            // SANITY CHECK #1
            if (Top() == oldTop)
            {
                // Can't have another turn when character just had a turn.
                RemoveTop();

                if (Count == 1)
                {
                    Add(oldTop);
                }
                else
                {
                    InsertAfterTop(oldTop);
                }
            }

            if (Count > 2)
            {
                // SANITY CHECK #2
                if (End() == oldEnd)
                {
                    // Can't not never let a character have a turn.
                    RemoveEnd();

                    if (Count - 2 < 0)
                    {
                        InsertBeforeTop(oldEnd);
                    }
                    else
                    {
                        InsertBeforeEnd(oldEnd);
                    }
                }
            }
        }

        public List<CharacterBase> ToList()
        {
            return queue;
        }

        private void CharacterBase_OnStatChanged(CharacterBase character, string propertyName, object newValue)
        {
            // if (propertyName == "Speed")
            // {
            //     // Sort queue again everytime a character has its speed changed.
            //     Sort(character);
            // }
        }

        public void Clear()
        {
            // Unsubscribe from character stat updates.
            foreach (CharacterBase character in queue)
            {
                character.OnStatChanged -= CharacterBase_OnStatChanged;
            }

            queue.Clear();

            //TODO: Update UI
        }

        private void Start()
        {
            // TODO: Visualise current state of queue
        }

        private void Update()
        {
        }
    }
}