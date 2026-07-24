using System;
using System.Collections;
using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using DG.Tweening;
using GameLib.EventChannelSystem;
using TMPro;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DayCountUI : MonoBehaviour
    {
         [SerializeField] private EventChannelSO flowEventChannel;
         [SerializeField] private float fadeDuration;
         [SerializeField] private float middleWaitTime;
         
         private TextMeshProUGUI _textMeshProUGUI;

         private void Awake()
         {
             _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
             
             
             var color = _textMeshProUGUI.color;
             color.a = 0;
             _textMeshProUGUI.color = color;
         }

         private void OnEnable()
         {
             flowEventChannel.AddListener<TurnStartEvent>(OnTurnStartHandler);
         }

         private void OnDisable()
         {
             flowEventChannel.RemoveListener<TurnStartEvent>(OnTurnStartHandler);
             _textMeshProUGUI.DOKill();
         }

         private void OnTurnStartHandler(TurnStartEvent evt)
         {
             _textMeshProUGUI.text = GameData.Instance.CurrentTurnLevel + " 일차";
             StartCoroutine(FadeInAndOut());
         }

         private IEnumerator FadeInAndOut()
         {
             _textMeshProUGUI.DOFade(1, fadeDuration);
             yield return new WaitForSeconds(fadeDuration);

             yield return new WaitForSeconds(middleWaitTime);

             _textMeshProUGUI.DOFade(0, fadeDuration);
         }
    }
}