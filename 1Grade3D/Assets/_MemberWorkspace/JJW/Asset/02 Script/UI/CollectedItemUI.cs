using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI
{
    public class CollectedItemUI : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI NameText { get;private set; }
        [field:SerializeField] public TextMeshProUGUI PriceText { get;private set; }
        [field:SerializeField] public Image ItemIcon { get;private set; }
    }
}