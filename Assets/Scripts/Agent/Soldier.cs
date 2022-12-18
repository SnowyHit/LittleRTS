using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Buildings;

namespace AgentSystem
{
    public class Soldier : Agent
    {
        [SerializeField]
        public float MaxhealthPoint;
        [SerializeField]
        private float _healthPoint;
        public float HealthPoint { get { return _healthPoint; } private set { _healthPoint = value; onHealthChanged?.Invoke(MaxhealthPoint, value); } }
        [SerializeField]
        private float _attackPoint;
        [SerializeField]
        private float _attackSpeed;
        public float AttackPoint { get { return _attackPoint; } set { _attackPoint = value; } }
        public Action<float, float> onHealthChanged;
        public Action<Agent> onDead;
        public Vector2Int aimedLocation;
        private Coroutine attackBuildingCoroutine;
        private Coroutine attackUnitCoroutine;
        private void Start()
        {
            onStartMovement += ResetAttacks; //resetting attacks if attacking anyone when we startMoving.
            MaxhealthPoint = HealthPoint;
        }
        void ResetAttacks(Vector2Int location, Agent agent)
        {
            if (attackBuildingCoroutine != null)
                StopCoroutine(attackBuildingCoroutine);
            if (attackUnitCoroutine != null)
                StopCoroutine(attackUnitCoroutine);
        }
        /// <summary>
        /// Starts attacking given soldier every attackspeed seconds.
        /// </summary>
        /// <param name="buildingToAttack"></param>
        public void AttackBuildingRunner(Building buildingToAttack)
        {
            attackBuildingCoroutine = StartCoroutine(AttackBuilding(buildingToAttack));
        }

        /// <summary>
        /// Starts attacking given soldier every attackspeed seconds.
        /// </summary>
        /// <param name="soldierToAttack"></param>
        public void AttackSoldierRunner(Soldier soldierToAttack)
        {
            attackUnitCoroutine = StartCoroutine(AttackSoldierCoroutine(soldierToAttack));
        }
        IEnumerator AttackBuilding(Building buildingToAttack)
        {
            while (buildingToAttack)
            {
                yield return new WaitForSeconds(_attackSpeed);
                buildingToAttack?.GetDamaged(AttackPoint);
            }
        }
        IEnumerator AttackSoldierCoroutine(Soldier agentToAttack)
        {
            while (agentToAttack)
            {
                yield return new WaitForSeconds(_attackSpeed);
                agentToAttack?.GetDamaged(AttackPoint);
            }
        }
        public void GetDamaged(float trueDamage)
        {
            HealthPoint -= trueDamage;
            if (HealthPoint <= 0)
            {
                onDead?.Invoke(this);
            }
        }
    }
}


