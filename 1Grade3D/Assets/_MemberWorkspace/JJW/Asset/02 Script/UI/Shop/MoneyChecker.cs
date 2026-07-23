using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Shop
{
    public class MoneyChecker : MonoBehaviour
    {
        public UnityEvent onNotEnoughMoney;

        public bool CheckMoney(int price)
        {
            if (GameData.Instance.CurrentMoney >= price)
                return true;
            onNotEnoughMoney?.Invoke();//돈부족하다고 창띄여주고 연출도 해줌
            return false;
        }
    }
}