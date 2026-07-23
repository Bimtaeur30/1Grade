using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using _MemberWorkspace.JJW.Asset._02_Script.UIUtility;
using TMPro;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Money
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class MoneyTextUpdater : MonoBehaviour
    {
        [SerializeField] private float countUpDuration = 0.3f;

        private TextMeshProUGUI _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            _text.text = GameData.Instance.CurrentMoney + "원";
        }

        private void OnEnable()
        {
            GameData.Instance.OnMoneyValueChanged += UpdateText;
        }

        private void OnDisable()
        {
            GameData.Instance.OnMoneyValueChanged -= UpdateText;
        }

        private void UpdateText(int before, int after)
        {
            CountUpText.Animate(_text, before, after, countUpDuration, "{0}원");
        }
    }
}