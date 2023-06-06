using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Data;

namespace UI.Items
{
    public class RedIcon : MonoBehaviour, Initializable<GameInfo>
    {
        [SerializeField] private SavedData savedData;
        [Space]
        [SerializeField] private CheckTypes checkType;
        [SerializeField] private GameObject icon;


        public void Initialize(GameInfo info)
        {
            bool active = false;

            switch(checkType)
            {
                case CheckTypes.Shop:
                    active = CheckBaseShop(info.Components.Money, info.Components.Gems) ||
                             CheckPremiumShop(info.Components.Money, info.Components.Gems);
                    break;
                case CheckTypes.Events:
                    active = CheckEvents();
                    break;
                case CheckTypes.Puzzle:
                    active = CheckPuzzle();
                    break;
                case CheckTypes.ShopBase:
                    active = CheckBaseShop(info.Components.Money, info.Components.Gems);
                    break;
                case CheckTypes.ShopPremium:
                    active = CheckPremiumShop(info.Components.Money, info.Components.Gems);
                    break;
                default:
                    active = false;
                    break;
            }

            icon.SetActive(active);
        }

        private bool CheckPremiumShop(IMoneyController coins, IMoneyController gems)
        {
            CostInfo info = savedData.CannonCost(ShopTypes.Premium);
            if (info != null)
            {
                if (gems.Money >= info.Cost)
                {
                    return true;
                }
            }

            info = savedData.BulletsCost(ShopTypes.Premium);
            if (info != null)
            {
                if (gems.Money >= info.Cost)
                {
                    return true;
                }
            }

            return false;
        }
        private bool CheckBaseShop(IMoneyController coins, IMoneyController gems)
        {
            CostInfo info = savedData.CannonCost(ShopTypes.Base);
            if (info != null)
            {
                if (coins.Money >= info.Cost)
                {
                    return true;
                }
            }

            info = savedData.BulletsCost(ShopTypes.Base);
            if (info != null)
            {
                if (coins.Money >= info.Cost)
                {
                    return true;
                }
            }

            return false;
        }
        private bool CheckPuzzle()
        {
            List<SavedData.PuzzleInfo> puzzleInfos = savedData.GetPuzzles();
            foreach(SavedData.PuzzleInfo info in puzzleInfos)
            {
                if (info.Avaliable && !info.Done)
                    return true;
            }
            return false;
        }
        private bool CheckEvents()
        {
            List<SavedData.EventInfo> events = savedData.GetEvents();
            foreach (SavedData.EventInfo info in events)
            {
                if (info.Avaliable && !info.Done)
                    return true;
            }
            return false;
        }

        private enum CheckTypes
        {
            Shop,
            Events,
            Puzzle,
            ShopBase,
            ShopPremium,
        }
    }
}
