using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Generic.Enums; 

namespace Buildings
{
    public class Building : MonoBehaviour
    {
        [SerializeField]
        private string buildingID;
        [SerializeField]
        private string buildingName;
        [SerializeField]
        private Sprite buildingImage;
        [SerializeField]
        public float MaxHealthPoint;
        [SerializeField]
        private float _healthPoint;
        [SerializeField]
        private BuildingType _type;
        public BuildingType Type {get{return _type;} protected set{_type = value ;}}
        public float HealthPoint {get{return _healthPoint;} protected set{_healthPoint = value ; onHealthPointChanged?.Invoke(MaxHealthPoint,value); CheckDestroyStatus(value);}}
        public string BuildingID{get{return buildingID;} private set{}}
        public string BuildingName{get{return buildingName;} private set{}}
        public Sprite BuildingImage{get{return buildingImage;} private set{}}
        public Vector2Int Dimensions;
        public Action<float , float> onHealthPointChanged;
        public Action<Building> onDestroyed;
        public Vector2Int baseGrid;
        public GameObject Prefab;

        private void Start()
        {
            MaxHealthPoint = HealthPoint;
        }        
        public void GetDamaged(float trueDamage)
        {
            HealthPoint -= trueDamage;
        }
        void CheckDestroyStatus(float currentHealth)
        {
            if(currentHealth <= 0)
            {
                Debug.Log("Ondestroyed Invoked");
                onDestroyed?.Invoke(this);
            }
        }
    }
}
