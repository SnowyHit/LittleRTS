using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Buildings;
using AgentSystem;
using Generic.Enums;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public Transform ProductionMenuContent;
        public Transform DescriptionMenuContent;
        public Transform MilitaryMenuContent;

        public List<ProductionItem> productItems;

        public Action<Building> onProductionItemClicked;
        public Action onResetClick;
        public Action<Building , Agent> onMilitaryItemClicked;
        public GameObject ProductionPrefab;
        public GameObject DescriptionPrefab;
        public GameObject MilitaryUnitPrefab;
        public List<Building> BuildingsOnInformation = new List<Building>();
        public List<Agent> AgentsOnInformation = new List<Agent>();
        public List<GameObject> InformationMenuObjects = new List<GameObject>();
        // Start is called before the first frame update
        void Start()
        {
            FillProductionMenu();
        }

        public void UpdateDescriptionPanel(Building buildingToUpdateWith)
        {
            if(buildingToUpdateWith.Type == BuildingType.Barracks)
            {
                FillMilitaryProductionMenu(buildingToUpdateWith);
            }
            BuildingsOnInformation.Add(buildingToUpdateWith);
        }
        public void UpdateDescriptionPanel(Agent agentToUpdateWith)
        {
            AgentsOnInformation.Add(agentToUpdateWith);
        }
        public void FillProductionMenu()
        {
            //Use ObjectPooling Here
            foreach (Building building in GameManager.Instance.AvailableBuildings)
            {
                ProductionItem tempProdItem = Instantiate(ProductionPrefab ,ProductionMenuContent).GetComponent<ProductionItem>();
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
        public void FillMilitaryProductionMenu(Building referenceBuilding)
        {
            foreach (string id in ((Barracks)referenceBuilding).ProducableAgentIDs)
            {
                Agent tempAgent = GameManager.Instance.AgentManagerRef.GetAgentFromAllAgents(id);
                if(tempAgent != null)
                {
                    MilitaryUnitItem tempMilitaryUnitItem = Instantiate(MilitaryUnitPrefab , MilitaryMenuContent).gameObject.GetComponent<MilitaryUnitItem>();
                    tempMilitaryUnitItem.Image.sprite = tempAgent.AgentImage;
                    tempMilitaryUnitItem.Name.text = tempAgent.AgentName;
                    tempMilitaryUnitItem.BuildingRef = referenceBuilding;
                    tempMilitaryUnitItem.AgentRef = tempAgent;
                    tempMilitaryUnitItem.button.onClick.AddListener(()=>{
                        MilitaryItemClicked(tempMilitaryUnitItem);
                    });
                    InformationMenuObjects.Add(tempMilitaryUnitItem.gameObject);
                }                
            }
        }
        public void FillInformationMenu()
        {
            foreach (Building building in BuildingsOnInformation)
            {
                InformationItem tempInformationItem = Instantiate(DescriptionPrefab , DescriptionMenuContent).gameObject.GetComponent<InformationItem>();
                tempInformationItem.Image.sprite = building.BuildingImage;
                tempInformationItem.Name.text = building.BuildingName;
                tempInformationItem.ChangeHealthBar(building.MaxHealthPoint, building.HealthPoint);
                building.onHealthPointChanged += tempInformationItem.ChangeHealthBar;
                tempInformationItem.onDestroyed += ()=>{
                    building.onHealthPointChanged -= tempInformationItem.ChangeHealthBar;
                };
                InformationMenuObjects.Add(tempInformationItem.gameObject);
            }
            foreach (Agent agent in AgentsOnInformation)
            {
                InformationItem tempInformationItem = Instantiate(DescriptionPrefab , DescriptionMenuContent).gameObject.GetComponent<InformationItem>();
                tempInformationItem.Image.sprite = agent.AgentImage;
                tempInformationItem.Name.text = agent.AgentName;
                if(agent is Soldier)
                {
                    Soldier soldierRef = (Soldier)agent;
                    tempInformationItem.ChangeHealthBar(soldierRef.MaxhealthPoint, soldierRef.HealthPoint);
                    soldierRef.onHealthChanged += tempInformationItem.ChangeHealthBar;
                    tempInformationItem.onDestroyed += ()=>{
                        soldierRef.onHealthChanged -= tempInformationItem.ChangeHealthBar;
                    };
                }
                InformationMenuObjects.Add(tempInformationItem.gameObject);
            }
        }
        public void ResetInformationMenu()
        {
            //Use ObjectPooling Here
            foreach (GameObject item in InformationMenuObjects)
            {
                Destroy(item);
            }
            BuildingsOnInformation.Clear();
            AgentsOnInformation.Clear();
        }
        public void MilitaryItemClicked(MilitaryUnitItem clickedItem)
        {
            onMilitaryItemClicked?.Invoke(clickedItem.BuildingRef , clickedItem.AgentRef);
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

