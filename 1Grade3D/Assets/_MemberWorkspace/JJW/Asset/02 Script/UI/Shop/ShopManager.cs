using System.Collections;
using System.Collections.Generic;
using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Shop
{
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private List<UpgradeCardUI> cards; //왼쪽, 가운데, 오른쪽
        [SerializeField] private GameObject shopUI; 

        [Header("등장 연출")]
        [SerializeField] private float entranceInterval = 0.15f;
        [SerializeField] private float entranceDuration = 0.4f;
        [SerializeField] private float entranceDistance = 200f;

        [Header("구매 연출")]
        [SerializeField] private float unselectedFadeDuration = 0.3f;
        [SerializeField] private float selectedFadeDuration = 0.3f;
        [SerializeField] private float dropDistance = 100f;
        [SerializeField] private float dropDuration = 0.3f;

        private Coroutine _entranceRoutine;
        private bool _isClosing; 

        private void OnEnable()
        {
            _isClosing = false;
            ResetCards();
            CursorManager.Instance.SetCursorVisible(false);

            foreach (var card in cards)
            {
                card.OnBought += HandleCardBought;
            }

            _entranceRoutine = StartCoroutine(PlayEntranceRoutine());
        }

        private void OnDisable()
        {
            foreach (var card in cards)
            {
                card.OnBought -= HandleCardBought;
            }

            if (_entranceRoutine != null)
            {
                StopCoroutine(_entranceRoutine);
                _entranceRoutine = null;
            }
            
            if(CursorManager.Instance != null)
                CursorManager.Instance.SetCursorVisible(true);
        }

        private void ResetCards()
        {
            foreach (var card in cards)
            {
                card.gameObject.SetActive(true);
                card.ResetVisual();
            }
        }

        private IEnumerator PlayEntranceRoutine()
        {
            foreach (var card in cards)
            {
                card.PlayEntrance(entranceDistance, entranceDuration);
                yield return new WaitForSeconds(entranceInterval);
            }
        }

        private void HandleCardBought(UpgradeCardUI selectedCard)
        {
            if (_isClosing) return;
            _isClosing = true;

            foreach (var card in cards)
            {
                if (card == selectedCard) continue;
                card.PlayUnselectedFadeOut(unselectedFadeDuration);
            }

            selectedCard.PlaySelectedDrop(dropDistance, dropDuration);
        }

        public void Skip()
        {
            if (_isClosing) return;
            _isClosing = true;

            StartCoroutine(SkipRoutine());
        }

        private IEnumerator SkipRoutine()
        {
            foreach (var card in cards)
            {
                card.PlayUnselectedFadeOut(unselectedFadeDuration);
            }

            yield return new WaitForSeconds(unselectedFadeDuration);

            shopUI.SetActive(false);
        }
    }
}