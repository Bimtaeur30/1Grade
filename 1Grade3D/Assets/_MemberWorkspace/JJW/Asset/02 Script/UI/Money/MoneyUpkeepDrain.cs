using System;
using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using GameLib.EventChannelSystem;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Money
{
    public class MoneyUpkeepDrain : MonoBehaviour
    {
        [SerializeField] private float drainInterval = 5f;
        [SerializeField] private int amountPerLevel = 1;
        [SerializeField] private EventChannelSO eventChannel;
        
        private void OnEnable()
        {
            eventChannel.AddListener<TurnEndEvent>(HandleTurnEnd);
        }

        private void OnDisable()
        {
            eventChannel.RemoveListener<TurnEndEvent>(HandleTurnEnd);
        }
        

        //임시긴 한데 지금 5초에 1씩깎이고 턴종료할때마다 1씩 더 깎이게 늘어남
        private float _timer;

        private void Update()
        {
            if(GameFlowManager.Instance.CurrentState != GameState.Exploring) return;//플레이중에만 시간이 흘러야하니까
            
            _timer += Time.deltaTime;

            if (_timer < drainInterval) return;

            _timer -= drainInterval;
            Drain();
        }

        private void HandleTurnEnd(TurnEndEvent evt)
        {
            _timer = 0f;   
        }

        private void Drain()
        {
            if(GameData.Instance == null) return;
            int drainAmount = amountPerLevel * GameData.Instance.CurrentTurnLevel;
            GameData.Instance.CurrentMoney = Mathf.Max(0, GameData.Instance.CurrentMoney - drainAmount);
        }
    }
}