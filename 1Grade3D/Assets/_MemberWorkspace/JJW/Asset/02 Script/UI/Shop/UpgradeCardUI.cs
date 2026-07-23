using System.Diagnostics.Tracing;
using _MemberWorkspace.JJW.Asset._02_Script.Managers;
using GameLib.EventChannelSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _MemberWorkspace.JJW.Asset._02_Script.UI.Shop
{
    public class UpgradeCardUI : MonoBehaviour
    {
        [SerializeField] private UpgradeCardSO upgradeCardSO;
        [SerializeField] private MoneyChecker moneyChecker;
        [SerializeField] private EventChannelSO eventChannelSO;
        
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image upgradeIcon;

        private void Start()
        {
            descriptionText.text = upgradeCardSO.LevelDatas[0].description;
            priceText.text = upgradeCardSO.LevelDatas[0].cost.ToString();
            levelText.text = GameData.Instance.CurrentLevel.ToString();
        }

        private void Refresh()//상점 UI띄울떄 초기화해주기
        {
            if(GameData.Instance.CurrentLevel >= 5) return;
                
            descriptionText.text = upgradeCardSO.LevelDatas[GameData.Instance.CurrentLevel - 1].description;//설명
            priceText.text = upgradeCardSO.LevelDatas[GameData.Instance.CurrentLevel - 1].cost.ToString();//가격
            levelText.text = GameData.Instance.CurrentLevel.ToString();//카드레벨
        }

        public void Buy()
        {
            if(!moneyChecker.CheckMoney(upgradeCardSO.LevelDatas[GameData.Instance.CurrentLevel -1].cost))
                return;
            switch (upgradeCardSO.Type)
            {
                case UpgradeType.MaxWeight:
                    GameData.Instance.CurrentMaxWeightLevel++;
                    eventChannelSO.RaiseEvent(PlayerEvents.MaxWeightUpgradeEvent.Init(upgradeCardSO.LevelDatas[GameData.Instance.CurrentLevel - 1].amount));
                    break;
                case UpgradeType.RunSpeed:
                    GameData.Instance.CurrentRunSpeedLevel++;
                    eventChannelSO.RaiseEvent(PlayerEvents.RunSpeedUpgradeEvent.Init(upgradeCardSO.LevelDatas[GameData.Instance.CurrentLevel - 1].amount));
                    break;
            }
            
        }
    }
}