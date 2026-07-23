using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using UnityEngine;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Money
{
    public class MoneyUpkeepDrain : MonoBehaviour
    {
        [SerializeField] private float drainInterval = 5f;
        [SerializeField] private int amountPerLevel = 1;

        //임시긴 한데 지금 5초에 1씩깎이고 턴종료할때마다 1쓸 더 깎여
        private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer < drainInterval) return;

            _timer -= drainInterval;
            Drain();
        }

        private void Drain()
        {
            int drainAmount = amountPerLevel * GameData.Instance.CurrentTurnLevel;
            GameData.Instance.CurrentMoney = Mathf.Max(0, GameData.Instance.CurrentMoney - drainAmount);
        }
    }
}