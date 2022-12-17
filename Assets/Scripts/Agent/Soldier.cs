using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Buildings;

namespace AgentSystem
{
    public class Soldier : Agent
    {
        private float _healthPoint;
        public float HealthPoint{get {return _healthPoint;} private set {_healthPoint = value; onHealthChanged?.Invoke(value);}}
        private float _attackPoint;
        public float AttackPoint{get {return _attackPoint;} set {_attackPoint = value;}}
        public Action<float> onHealthChanged;
        public Action<Agent> onDead;
        public void AttackBuilding(Building buildingToAttack)
        {
            buildingToAttack.GetDamaged(AttackPoint);
        }

        public void GetDamaged(float trueDamage)
        {
            HealthPoint -= trueDamage;
            if(HealthPoint <= 0)
            {
                onDead?.Invoke(this);
            }
        }
    }
}


