using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using _MemberWorkspace.JJW.Asset._02_Script.UIUtility;
using TMPro;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Money
{
    public class MoneyGainPunchEffect : MonoBehaviour
    {
        [SerializeField] private float punchScale = 0.4f;
        [SerializeField] private float punchDuration = 0.3f;

        private TextMeshProUGUI _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            GameData.Instance.OnMoneyValueChanged += HandleMoneyChanged;
        }

        private void OnDisable()
        {
            GameData.Instance.OnMoneyValueChanged -= HandleMoneyChanged;
        }

        private void HandleMoneyChanged(int before, int after)
        {
            if (after <= before) return;
            TextPunch.Play(_text, punchScale, punchDuration);
        }
    }
}