using System.Collections.Generic;
using _MemberWorkspace.JJW.Asset._02_Script.Events;
using _MemberWorkspace.JJW.Asset._02_Script.Item;
using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using GameLib.EventChannelSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _MemberWorkspace.JJW.Asset._02_Script
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private EventChannelSO eventChannel;
        [SerializeField] private List<ItemSO> testItems;

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                RaiseTestSettlement();
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                RaiseMoneyTest();
            }

            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                DownMoney();
            }
        }

        public void RaiseTestSettlement()
        {
            if (testItems == null || testItems.Count == 0)
            {
                return;
            }

            eventChannel.RaiseEvent(new SettlementEvent(testItems));
        }

        private void RaiseMoneyTest()
        {
            GameData.Instance.CurrentMoney+=100;
        }
        private void DownMoney()
        {
            GameData.Instance.CurrentMoney-=100;
        }

        [ContextMenu("GOTOMainMenu")]
        public void ChangeSceneMainMenu()
        {
            SceneFlowManager.Instance.GoToScene(SceneType.MainMenu);
        }

        [ContextMenu("GOTOInGameScene")]
        public void ChangeSceneInGameMenu()
        {
            SceneFlowManager.Instance.GoToScene(SceneType.InGame);
        }
    }
}