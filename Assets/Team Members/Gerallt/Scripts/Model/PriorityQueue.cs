using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChainsOfFate.Gerallt;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class PriorityQueue : MonoBehaviour
    {
        public List<CharacterBase> queue = new List<CharacterBase>();

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
        
        public void Sort()
        {
            //Sort all the characters by their speed priority.
            queue = queue.OrderBy(chr => chr.Speed).ToList();
        }

        public List<CharacterBase> ToList()
        {
            return queue;
        }

        public void Clear()
        {
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