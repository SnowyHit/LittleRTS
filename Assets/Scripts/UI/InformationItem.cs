using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Buildings;
using System;

namespace UI
{
    public class InformationItem : MonoBehaviour
    {
        public Image Image;
        public TMPro.TMP_Text Name;
        public Image HealthBarImage;
        public TMPro.TMP_Text Health;
        public Action onDisabled;
        public void ChangeHealthBar(float MaxValue , float currentValue)
        {
            HealthBarImage.fillAmount = currentValue/MaxValue;
            Health.text = MaxValue + "/" + currentValue;
        }
        private void OnDisable()
        {
            onDisabled?.Invoke();
        }
    }
}
