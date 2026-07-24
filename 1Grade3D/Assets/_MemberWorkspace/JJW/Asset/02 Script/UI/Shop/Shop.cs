using _MemberWorkspace.JJW.Asset._02_Script.Events;
using GameLib.EventChannelSystem;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Shop
{
    public class Shop : MonoBehaviour
    {
        [SerializeField] private EventChannelSO eventChannel;
        [SerializeField] private GameObject shopPanel;
        private void OnEnable()
        {
            eventChannel.AddListener<ShopOpenEvent>(HandleShopOpen);
        }

        private void OnDisable()
        {
            eventChannel.RemoveListener<ShopOpenEvent>(HandleShopOpen);
        }

        private void HandleShopOpen(ShopOpenEvent evt)
        {
            shopPanel.gameObject.SetActive(true);
        }
    }
}