using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChainsOfFate.Gerallt;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

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

        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private Transform contentParent;
        
        private List<GameObject> contentList = new List<GameObject>();
        
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
                RemoveTop();
                Add(top); // Re-add character to end of queue.
            }

            return top;
        }

        public void Enqueue(CharacterBase characterBase)
        {
            queue.Add(characterBase);

            characterBase.OnStatChanged += CharacterBase_OnStatChanged;

            // GameObject nodeInstance = Instantiate(nodePrefab, contentParent);
            //
            // nodeInstance.GetComponentInChildren<Image>().color = characterBase.representation;
            //
            // Champion characterBase2 = nodeInstance.AddComponent<Champion>();
            // characterBase2.Speed = characterBase.Speed;
            //
            // contentList.Add(nodeInstance);
        }

        public void RemoveAt(int index)
        {
            queue.RemoveAt(index);

            // var tmp = contentList[index];
            // contentList.RemoveAt(index); 
            // Destroy(tmp);
        }
        
        public void RemoveTop()
        {
            RemoveAt(0);
        }

        public void RemoveEnd()
        {
            //RemoveAt(Count - 1);
            int index = Count - 1;
            
            queue.RemoveAt(index);
            
            // var tmp = contentList[index];
            // contentList.RemoveAt(index); 
            // Destroy(tmp);
        }

        public void Add(CharacterBase newCharacter)
        {
            queue.Add(newCharacter);
            
            // GameObject nodeInstance = Instantiate(nodePrefab, contentParent);
            //
            // nodeInstance.GetComponentInChildren<Image>().color = newCharacter.representation;
            //
            // Champion characterBase2 = nodeInstance.AddComponent<Champion>();
            // characterBase2.Speed = newCharacter.Speed;
            //
            //
            // contentList.Add(nodeInstance);
        }

        public void InsertBeforeTop(CharacterBase newTop)
        {
            int positionId = queue.IndexOf(newTop);
            
            queue.Insert(0, newTop);

            // Transform top = contentList[0].transform;
            // top.SetSiblingIndex(1);
            //
            // Transform newTransform = contentList[positionId].transform;
            // newTransform.SetSiblingIndex(0);
        }

        public void InsertAfterTop(CharacterBase second)
        {
            int positionId = queue.IndexOf(second);
            
            queue.Insert(1, second);
            
            // Transform secTransform = contentList[positionId].transform;
            // secTransform.SetSiblingIndex(1);
            //
            // for (int i = 1; i < contentList.Count; i++)
            // {
            //     Transform oldTransform2 = contentList[i].transform;
            //     oldTransform2.SetSiblingIndex(i+1);
            // }
        }

        public void InsertBeforeEnd(CharacterBase secondLast)
        {
            queue.Insert(Count - 2, secondLast);
            
            // Transform secLastTransform = contentList[Count - 2].transform;
            // Transform lastTransform = contentList[Count - 1].transform;
            // int lastIndex = lastTransform.GetSiblingIndex();
            // secLastTransform.SetSiblingIndex(lastIndex);
            // lastTransform.SetSiblingIndex(lastIndex + 1);
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

            // contentList = contentList.OrderByDescending(go => go.GetComponent<Champion>().Speed).ToList();
            // int i = -1;
            // foreach (var item in contentList)
            // {
            //     item.transform.SetSiblingIndex(++i);
            // }
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