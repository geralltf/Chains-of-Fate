using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChainsOfFate.Gerallt;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private float animationSpeed = 1.0f;
        
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

            // Update UI.
            UpdateView();
        }

        public void RemoveAt(int index)
        {
            queue.RemoveAt(index);

            // Update UI.
            UpdateView();
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
            
            // Update UI.
            UpdateView();
        }

        public void Add(CharacterBase newCharacter)
        {
            queue.Add(newCharacter);
            
            // Update UI.
            UpdateView();
        }

        public void InsertBeforeTop(CharacterBase newTop)
        {
            int positionId = queue.IndexOf(newTop);
            
            queue.Insert(0, newTop);

            // Update UI.
            UpdateView();
        }

        public void InsertAfterTop(CharacterBase second)
        {
            int positionId = queue.IndexOf(second);
            
            queue.Insert(1, second);
            
            // Update UI.
            UpdateView();
        }

        public void InsertBeforeEnd(CharacterBase secondLast)
        {
            queue.Insert(Count - 2, secondLast);
            
            // Update UI.
            UpdateView();
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

            UpdateView();
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

            // Update UI.
            UpdateView();
        }

        private class ViewObj
        {
            public string characterName;
            public Vector3 position;
            public GameObject gameObject;
        }
        
        private void UpdateView()
        {
            // Visualise current state of queue

            List<ViewObj> oldPositions = new List<ViewObj>();

            // Destroy all node UI instances in content parent view
            for (int idx = 0; idx < contentParent.transform.childCount; idx++ )
            {
                Transform child = contentParent.transform.GetChild(idx);
                GameObject nodeInstance = child.gameObject;

                string characterName = nodeInstance.GetComponentInChildren<TextMeshProUGUI>().text;

                if (queue.Any(c => c.CharacterName == characterName))
                {
                    oldPositions.Add(new ViewObj()
                    {
                        characterName = characterName,
                        position = nodeInstance.GetComponent<RectTransform>().position
                    });
                }
                
                Destroy(nodeInstance);
            }
            
            for (int i = 0; i < queue.Count; i++)
            {
                CharacterBase character = queue[i];
                
                GameObject nodeInstance = Instantiate(nodePrefab, contentParent);
                nodeInstance.GetComponentInChildren<Image>().color = character.representation;
                nodeInstance.GetComponentInChildren<TextMeshProUGUI>().text = character.CharacterName;
                RectTransform rectTransform = nodeInstance.GetComponent<RectTransform>();

                Vector3 pos = rectTransform.position;
                pos.x = (i * 60.0f) + 0.0f; // Use this if you are not using the scroll rect to position elements.
                pos.y = 0.0f;
                rectTransform.position = pos;

                Vector3 newPosition = rectTransform.position;
                ViewObj oldPosition = oldPositions.FirstOrDefault(v => v.characterName == character.CharacterName);

                if (oldPosition != null)
                {
                    // Reset transform to old position
                    nodeInstance.GetComponent<RectTransform>().position = oldPosition.position; 
                    
                    StartCoroutine(AnimateItem(nodeInstance, newPosition));    
                }
            }
        }

        IEnumerator AnimateItem(GameObject go, Vector3 newPos)
        {
            float dist;
            bool animating = true;

            while (animating)
            {
                if (go == null)
                {
                    animating = false;
                }
                else
                {
                    RectTransform rectTransform = go.GetComponent<RectTransform>();
                    
                    dist = Vector3.Distance(rectTransform.localPosition, newPos);
                    
                    if (dist < 0.1f)
                    {
                        animating = false;
                    }
                    else
                    {
                        rectTransform.localPosition = Vector3.Slerp(rectTransform.localPosition, newPos, animationSpeed * Time.deltaTime);
                    }
                }
                
                yield return new WaitForEndOfFrame();
            }
            

        }
        
        private void Start()
        {
            UpdateView();
        }

        private void Update()
        {
        }
    }
}