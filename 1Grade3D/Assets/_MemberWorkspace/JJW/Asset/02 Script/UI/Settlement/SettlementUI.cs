using System.Collections;
using System.Collections.Generic;
using _MemberWorkspace.JJW.Asset._02_Script.Events;
using _MemberWorkspace.JJW.Asset._02_Script.Item;
using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using _MemberWorkspace.JJW.Asset._02_Script.UIUtility;
using DG.Tweening;
using GameLib.EventChannelSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Settlement
{
    public class SettlementUI : MonoBehaviour
    {
        [SerializeField] private EventChannelSO eventChannel;
        [SerializeField] private GameObject collectedItemUIPrefab;
        [SerializeField] private Transform itemListContainer;
        [SerializeField] private CanvasGroup settlementUIPanel;
        [SerializeField] private TextMeshProUGUI totalMoneyText;
        
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float countUpDuration = 0.3f;
        [SerializeField] private float itemRevealInterval = 0.3f;
        [SerializeField] private float charInterval = 0.05f;
        
        private int _currentMoney;
        private Tween _moneyTween;
        private Coroutine _drawRoutine;
        private Coroutine _typeRoutine;

        private void Start()
        {
            settlementUIPanel.alpha = 0;
        }

        private void OnEnable()
        {
            eventChannel.AddListener<SettlementEvent>(HandleSettlement);
        }

        private void OnDisable()
        {
            eventChannel.RemoveListener<SettlementEvent>(HandleSettlement);
        }



        private void HandleSettlement(SettlementEvent evt)
        {
            ResetSettlement();

            settlementUIPanel.DOFade(1, fadeDuration).OnComplete(() =>
            {
                _drawRoutine = StartCoroutine(DrawCollectedItemRoutine(evt.Items));
            });
        }

        private void ResetSettlement()
        {
            if (_drawRoutine != null) StopCoroutine(_drawRoutine);
            if (_typeRoutine != null) StopCoroutine(_typeRoutine);
            _moneyTween?.Kill();
            settlementUIPanel.DOKill();

            settlementUIPanel.alpha = 0;

                        _currentMoney = 0;
            totalMoneyText.text = "0";

            for (int i = itemListContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(itemListContainer.GetChild(i).gameObject);
            }
        }

        private IEnumerator DrawCollectedItemRoutine(List<ItemSO> items)
        {
            foreach (var item in items)
            {
                CreateItemUI(item);
                yield return new WaitForSeconds(itemRevealInterval);
            }

            int finalMoney = _currentMoney + GameData.Instance.CurrentMoney;
            string finalText = $"{_currentMoney} + {GameData.Instance.CurrentMoney} = {finalMoney}원";
            _typeRoutine = StartCoroutine(TypewriterText.Play(totalMoneyText, finalText, charInterval));
            GameData.Instance.CurrentMoney = finalMoney;
        }

        private void CreateItemUI(ItemSO item)
        {
            GameObject instance = Instantiate(collectedItemUIPrefab, itemListContainer);
            CollectedItemUI ui = instance.GetComponent<CollectedItemUI>();

            ui.NameText.text = item.Name;
            ui.PriceText.text = item.Price.ToString();
            ui.ItemIcon.sprite = item.Icon;

            int previousMoney = _currentMoney;
            _currentMoney += item.Price;

            AnimateMoneyCount(previousMoney, _currentMoney);
        }
        
        private void AnimateMoneyCount(int from, int to)
        {
            _moneyTween?.Kill();
            _moneyTween = CountUpText.Animate(totalMoneyText, from, to, countUpDuration);
        }
    }
}