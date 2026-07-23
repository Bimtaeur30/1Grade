using System.Collections.Generic;
using _MemberWorkspace.JJW.Asset._02_Script.Events;
using _MemberWorkspace.JJW.Asset._02_Script.Item;
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
        }

        public void RaiseTestSettlement()
        {
            if (testItems == null || testItems.Count == 0)
            {
                return;
            }

            eventChannel.RaiseEvent(new SettlementEvent(testItems));
        }
    }
}