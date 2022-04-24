using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private int xSpaceBetweenItem = 60;
        [SerializeField] private int ySpaceBetweenItems = 60;
        [SerializeField] private int xStart = 0;
        [SerializeField] private int yStart = 0;
        [SerializeField] private int numberOfColumns = 1;
        [SerializeField] private float toggleInSeconds = 0.3f;
        
        private Champion currentPlayer;
        private bool isToggling = false;
        
        public void ItemButton_OnClick(IDescriptive item)
        {
            Debug.Log("TODO: Inventory Item use " + item.GetName() + " " + item.GetDescription());
        }

        public void PopulateView()
        {
            List<IDescriptive> allItems = currentPlayer.GetInventory();

            int i = 0;
            
            for (int test = 0; test < 100; test++)
            {
                foreach (IDescriptive item in allItems)
                {
                    GameObject itemUIInstance = Instantiate(itemViewPrefab, content.transform);
                    //itemUIInstance.transform.localPosition = GetPosition(i); // Auto laid out by Grid Layout Group in Content area within Scroll View

                    Button button = itemUIInstance.GetComponentInChildren<Button>();
                    TextMeshProUGUI buttonText = itemUIInstance.GetComponentInChildren<TextMeshProUGUI>();
                    Image buttonImage = itemUIInstance.GetComponentInChildren<Image>();
                    
                    buttonText.text = item.GetName();
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
                ClearItems();
                
                PopulateView();
            }
        }
        
        private Vector3 GetPosition(int i)
        {
            float x = xStart + (xSpaceBetweenItem * (i % numberOfColumns));
            float y = yStart + (-ySpaceBetweenItems * (i / numberOfColumns));
            
            return new Vector3(x, y,0f);
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