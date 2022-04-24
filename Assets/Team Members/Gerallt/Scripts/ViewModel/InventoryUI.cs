using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ChainsOfFate.Gerallt
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameObject view;
        [SerializeField] private GameObject content;
        [SerializeField] private GameObject itemViewPrefab;
        [SerializeField] private TextMeshProUGUI textCharacterName;
        [SerializeField] private Button buttonFilterWeapons;
        [SerializeField] private Button buttonFilterSpells;
        [SerializeField] private Button buttonFilterItems;
        [SerializeField] private Button buttonFilterAll;
        [SerializeField] private FilterBy filterBy = FilterBy.All;
        [SerializeField] private int xSpaceBetweenItem = 60;
        [SerializeField] private int ySpaceBetweenItems = 60;
        [SerializeField] private int xStart = 0;
        [SerializeField] private int yStart = 0;
        [SerializeField] private int numberOfColumns = 1;
        [SerializeField] private float toggleInSeconds = 0.3f;
        
        private Champion currentPlayer;
        private bool isToggling = false;
        
        public enum FilterBy
        {
            Weapons,
            Spells,
            Items,
            All
        }
        
        public void ItemButton_OnClick(IDescriptive item)
        {
            Debug.Log("TODO: Inventory Item use " + item.GetName() + " " + item.GetDescription());
        }

        public void PopulateView()
        {
            ClearItems();
            
            List<IDescriptive> allItems = currentPlayer.GetInventory();

            switch (filterBy)
            {
                case FilterBy.All:
                    break;
                case FilterBy.Weapons:
                    allItems = allItems.Where(item => item is WeaponBase).ToList();
                    break;
                case FilterBy.Spells:
                    allItems = allItems.Where(item => item is SpellBase).ToList();
                    break;
                case FilterBy.Items:
                    allItems = allItems.Where(item => item is ItemBase).ToList();
                    break;
            }
            
            int i = 0;
            
            for (int test = 0; test < 100; test++)
            {
                foreach (IDescriptive item in allItems)
                {
                    GameObject itemUIInstance = Instantiate(itemViewPrefab, content.transform);

                    Button button = itemUIInstance.GetComponentInChildren<Button>();
                    TextMeshProUGUI buttonText = itemUIInstance.GetComponentInChildren<TextMeshProUGUI>();
                    Image buttonImage = itemUIInstance.GetComponentInChildren<Image>();
                    
                    buttonText.text = item.GetName() + " " + i;
                    buttonImage.color = item.GetTint();
                    
                    button.onClick.AddListener(() =>
                    {
                        ItemButton_OnClick(item);
                    });
                    
                    i++;
                }
            }

            textCharacterName.text = currentPlayer.CharacterName;
        }

        public void ClearItems()
        {
            for (int i = 0; i < content.transform.childCount; i++)
            {
                Transform child = content.transform.GetChild(i);
                
                Destroy(child.gameObject);
            }
        }

        public void ToggleVisibility()
        {
            if (!isToggling)
            {
                isToggling = true;
                StartCoroutine(ToggleVisibilityCoroutine());
            }
        }

        public void SetVisibility(bool visibility)
        {
            view.SetActive(visibility);

            if (visibility)
            {
                PopulateView();
            }
        }

        private IEnumerator ToggleVisibilityCoroutine()
        {
            yield return new WaitForSeconds(toggleInSeconds);
            SetVisibility(!view.activeInHierarchy);
            isToggling = false;
        }

        private void Start()
        {
            currentPlayer = GameManager.Instance.GetPlayer();

            SetVisibility(false);
            
            buttonFilterWeapons.onClick.AddListener(() =>
            {
                filterBy = FilterBy.Weapons;
                PopulateView();
            });
            
            buttonFilterSpells.onClick.AddListener(() =>
            {
                filterBy = FilterBy.Spells;
                PopulateView();
            });
            
            buttonFilterItems.onClick.AddListener(() =>
            {
                filterBy = FilterBy.Items;
                PopulateView();
            });
            
            buttonFilterAll.onClick.AddListener(() =>
            {
                filterBy = FilterBy.All;
                PopulateView();
            });
        }

        private void Update()
        {
            if (InputSystem.GetDevice<Keyboard>().iKey.isPressed) // TODO: Use proper new input action binding
            {
                ToggleVisibility();
            }
        }
    }
}