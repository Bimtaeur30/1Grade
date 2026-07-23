using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Shop
{
    public class MoneyChecker : MonoBehaviour
    {
        public UnityEvent OnNotEnoughMoney;

        public bool CheckMoney(int price)
        {
            if (GameData.Instance.CurrentMoney >= price)
                return true;
            return false;
        }
    }
}