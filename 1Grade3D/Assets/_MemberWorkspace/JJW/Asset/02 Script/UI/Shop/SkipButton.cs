using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Shop
{
    public class SkipButton : MonoBehaviour
    {
        [SerializeField] private ShopManager shopManager;

        public void OnClick()
        {
            shopManager.Skip();
        }
    }
}
