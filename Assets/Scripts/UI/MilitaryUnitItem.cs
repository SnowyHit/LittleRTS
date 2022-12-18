using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Buildings;
using AgentSystem;

namespace UI
{
    public class MilitaryUnitItem : MonoBehaviour
    {
        public Image Image;
        public TMPro.TMP_Text Name;
        public Button button;
        public Building BuildingRef;
        public Agent AgentRef;
    }
}
