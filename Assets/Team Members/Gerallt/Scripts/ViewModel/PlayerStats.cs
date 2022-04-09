using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ChainsOfFate.Gerallt
{
    public class PlayerStats : MonoBehaviour
    {
        public CombatGameManager combatGameManager;
        public GameObject view;
        public CharacterBase currentCharacter;

        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private Slider healthBar;

        [SerializeField] private Slider resolveBar;

        [SerializeField] private Slider arcanaBar;

        private void OnEnable()
        {
            combatGameManager.OnHavingTurn += CombatGameManager_OnHavingTurn;
            combatGameManager.OnTurnCompleted += CombatGameManager_OnTurnCompleted;
        }

        private void OnDisable()
        {
            combatGameManager.OnHavingTurn -= CombatGameManager_OnHavingTurn;
            combatGameManager.OnTurnCompleted -= CombatGameManager_OnTurnCompleted;
        }

        private void Update()
        {
            if (currentCharacter != null)
            {
                currentCharacter.UpdatePrimaryStats(); // HACK: Force update of stats to make it easy to see changes from inspector.
            }
        }

        private void CombatGameManager_OnHavingTurn(CharacterBase nextCharacter)
        {
            currentCharacter = nextCharacter;

            currentCharacter.OnStatChanged += CurrentCharacter_OnStatChanged;
            currentCharacter.UpdatePrimaryStats();
        }

        private void CombatGameManager_OnTurnCompleted(CharacterBase currCharacter, CharacterBase nextCharacter)
        {
            currentCharacter.OnStatChanged -= CurrentCharacter_OnStatChanged;

            currentCharacter = nextCharacter;
        }

        private void CurrentCharacter_OnStatChanged(CharacterBase character, string propertyName, object newValue)
        {
            if (character != currentCharacter)
                return;

            if (propertyName == "CharacterName")
            {
                characterNameText.text = (string) newValue;
            }

            if (propertyName == "HP")
            {
                healthBar.maxValue = character.maxHealth;
                healthBar.value = (int) newValue;
            }

            if (propertyName == "Arcana")
            {
                arcanaBar.maxValue = character.maxArcana;
                arcanaBar.value = (int) newValue;
            }

            if (propertyName == "Resolve")
            {
                resolveBar.maxValue = character.maxResolve;
                resolveBar.value = (int) newValue;
            }
        }
    }
}