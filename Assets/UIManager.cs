using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Buildings;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public Transform ProductionMenuContent;

        public List<ProductionItem> productItems;

        public Action<Building> onProductionItemClicked;
        public Action onResetClick;
        // Start is called before the first frame update
        void Start()
        {
            FillProductionMenu();
        }

        public void UpdateDescriptionPanel(Building buildingToUpdateWith)
        {
            Debug.Log(buildingToUpdateWith.HealthPoint);
            Debug.Log(buildingToUpdateWith.BuildingName);
        }
        public void FillProductionMenu()
        {
            foreach (Building building in GameManager.Instance.AvailableBuildings)
            {
                ProductionItem tempProdItem = Instantiate(building.Prefab ,ProductionMenuContent).GetComponent<ProductionItem>();
                tempProdItem.Image.sprite = building.BuildingImage;
                tempProdItem.Name.text = building.BuildingName;
                tempProdItem.BuildingRef = building;
                tempProdItem.button = tempProdItem.gameObject.GetComponent<Button>();
                tempProdItem.button.onClick.AddListener(()=>{
                    ProdItemClicked(tempProdItem);
                });
                productItems.Add(tempProdItem);
            }
        }

        public void ProdItemClicked(ProductionItem clicedProdItem)
        {
            if(clicedProdItem.Frame.color == Color.cyan)
            {
                ClearClickedItems();
                return;
            }
            foreach (var item in productItems)
            {
                if(item == clicedProdItem)
                {
                    clicedProdItem.Frame.color = Color.cyan;
                    continue;
                }
                item.Frame.color = Color.clear;
            }
            onProductionItemClicked?.Invoke(clicedProdItem.BuildingRef);
        }

        public void ClearClickedItems()
        {
            foreach (var item in productItems)
            {
                item.Frame.color = Color.clear;
            }
            onResetClick?.Invoke();
        }
        
    }
}

