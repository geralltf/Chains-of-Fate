using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace ChainsOfFate.Gerallt
{
    public class FriendlyNPC : MonoBehaviour
    {
        [SerializeField] private string defaultGreeting = "Hi!";
        [SerializeField] private List<string> greetingList;
        [SerializeField] private float movementSpeed = 3.7f;
        [SerializeField] private float minDistanceToPlayer = 0.1f;
        [SerializeField] private SpriteRenderer friendlySpriteRenderer;

        [SerializeField] private float greetingDisappearTime = 5.0f;
        [SerializeField] private GameObject viewGreeting;
        [SerializeField] private TextMeshPro textMeshProGreeting;

        [SerializeField] private GameObject viewGreetInteractButtons;

        public bool isPartyMember = false;
        public Transform playerTarget;
        public NpcState state = NpcState.Idle;

        private Champion champion;
        private PlayerSensor playerSensor;
        private Rigidbody2D rb;
        private bool isGreeting = false;
        private bool inDialogue = false;
        private bool canExitDialogue = false;
        private bool canEnterDialogue = true;
        private float spawnZ; //HACK: 

        public enum NpcState
        {
            Idle,
            GreetPlayer,
            FollowingPlayer,
            TalkingToPlayer
        }

        public void MoveTowardsPlayer()
        {
            Vector2 pos = transform.position;

            Vector2 playerPos = playerTarget.position;
            float distanceToPlayer = Vector2.Distance(pos, playerPos);
            Vector2 directionToPlayer = -(pos - playerPos).normalized;
            Vector2 posDelta = directionToPlayer * movementSpeed * Time.fixedDeltaTime;

            if (distanceToPlayer > minDistanceToPlayer)
            {
                pos.x += posDelta.x;
                pos.y += posDelta.y;

                rb.MovePosition(pos);
        
                UpdateSprite(posDelta);
            }
        }

        public void GreetPlayer()
        {
            if (!isGreeting)
            {
                StartCoroutine(GreetingCoroutine());
            }
        }

        public void SetGreetingVisibility(bool visibility)
        {
            if (visibility)
            {
                // Pick a greeting!
                string greeting = defaultGreeting;
                
                if (greetingList.Count > 0)
                {
                    greeting = greetingList[Random.Range(0, greetingList.Count - 1)];
                }
                textMeshProGreeting.text = greeting;
            }
            viewGreeting.SetActive(visibility);
        }

        public void SetGreetInteractionVisibility(bool visibility)
        {
            viewGreetInteractButtons.SetActive(visibility);
        }
        
        private IEnumerator GreetingCoroutine()
        {
            isGreeting = true;
            SetGreetingVisibility(true);
            SetGreetInteractionVisibility(true);
            yield return new WaitForSeconds(greetingDisappearTime);
            SetGreetingVisibility(false);
            SetGreetInteractionVisibility(false);
            isGreeting = false;
            // if (state != NpcState.TalkingToPlayer || state != NpcState.FollowingPlayer)
            // {
            //     state = NpcState.Idle;
            // }
        }
        
        private void UpdateSprite(Vector2 pos)
        {
            if (pos.x < 0)
            {
                friendlySpriteRenderer.flipX = true;
            }
            else
            {
                friendlySpriteRenderer.flipX = false;
            }

            friendlySpriteRenderer.transform.rotation = Quaternion.identity;
        }
        
        private void Awake()
        {
            playerSensor = GetComponentInChildren<PlayerSensor>();
            rb = GetComponent<Rigidbody2D>();
            champion = GetComponent<Champion>();
            
            spawnZ = transform.position.z; // HACK: 

            SetGreetingVisibility(false);
            SetGreetInteractionVisibility(false);
        }

        private void Update()
        {
            if (playerSensor.DetectedPlayer == null)
            {
                state = NpcState.Idle;

                if (inDialogue)
                {
                    StartCoroutine(EndDialogueCoroutine());
                }
            }

            if (!isPartyMember)
            {
                if (playerSensor.DetectedPlayer != null)
                {
                    switch (state)
                    {
                        case NpcState.Idle:
                            state = NpcState.GreetPlayer;

                            GreetPlayer();
                            break;
                        case NpcState.GreetPlayer:
                            break;
                        case NpcState.FollowingPlayer:
                            break;
                        case NpcState.TalkingToPlayer:
                            break;
                    }
                }
            }

            if (state is NpcState.GreetPlayer or NpcState.TalkingToPlayer)
            {
                if (InputSystem.GetDevice<Keyboard>().eKey.isPressed) // TODO: Use proper input system action event
                {
                    if (!inDialogue && canEnterDialogue)
                    {
                        StartCoroutine(StartDialogueCoroutine());
                    }
                    else if (inDialogue && canExitDialogue)
                    {
                        StartCoroutine(EndDialogueCoroutine());
                    }
                }
            }
            
            if(state == NpcState.FollowingPlayer)
            {
                if (playerTarget != null)
                {
                    MoveTowardsPlayer();
                }
            }
        }

        private IEnumerator StartDialogueCoroutine()
        {
            inDialogue = true;
            canExitDialogue = false;
            state = NpcState.TalkingToPlayer;
            
            DialogueSystemUI.Instance.Show(champion);

            yield return new WaitForSeconds(DialogueSystemUI.Instance.closeAllowTime);

            canExitDialogue = true;
        }
        
        private IEnumerator EndDialogueCoroutine()
        {
            canEnterDialogue = false;
            DialogueSystemUI.Instance.Hide();
            yield return new WaitForSeconds(DialogueSystemUI.Instance.openAllowTime);
            inDialogue = false;
            canEnterDialogue = true;

            if (!isPartyMember)
            {
                state = NpcState.Idle;
            }
            else
            {
                state = NpcState.FollowingPlayer;
            }
        }
        
        private void FixedUpdate()
        {
            Vector3 oldPos = transform.position;
            oldPos.z = spawnZ; // HACK: 
            transform.position = oldPos;
        }

        public void AddAsPartyMember(Champion player)
        {
            if (!isPartyMember)
            {
                isPartyMember = true;
                playerTarget = playerSensor.DetectedPlayer.transform;

                if (!player.partyMembers.Contains(champion))
                {
                    player.partyMembers.Add(champion);
                }

                DontDestroyOnLoad(gameObject);
            }
        }
    }
}