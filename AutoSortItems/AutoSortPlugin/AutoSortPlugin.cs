using BepInEx;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ExamplePlugin
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class AutoSortPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Nunchuk";
        public const string PluginName = "AutoSortItems";
        public const string PluginVersion = "1.0.0";

        List<int> Tier1 = new List<int>();
        List<int> Tier2 = new List<int>();
        List<int> Tier3 = new List<int>();
        List<int> Lunar = new List<int>();
        List<int> Boss = new List<int>();
        List<int> NoTier = new List<int>();
        List<int> VoidTier1 = new List<int>();
        List<int> VoidTier2 = new List<int>();
        List<int> VoidTier3 = new List<int>();
        List<int> VoidBoss = new List<int>();
        List<int> AssignedAtRuntime = new List<int>();
        List<int> TestList = new List<int>();

        public void Awake() /////////////////////////////////////////////////////////TODO: Cleanup code, fix mithrix returning items visually is jank, sort within tiers?
        {                   /////////////////////////////////////////////////////////Also rune if you see this, make sure to change the build action in the csproj
            Log.Init(Logger);
            On.RoR2.UI.ItemInventoryDisplay.UpdateDisplay += (orig, self) =>
            {
                //Debug.Log("Pre sort");
                //foreach (var item in self.itemOrder)
                //{
                //    Debug.Log($"----------   {item}, {ItemCatalog.GetItemDef(item).nameToken}");
                //}
                SortItems(self.itemOrder);
                //Debug.Log("Post sort");
                //foreach (var item in self.itemOrder)
                //{
                //    Debug.Log($"----------   {item}, {ItemCatalog.GetItemDef(item).nameToken}");
                //}
                orig(self);
            };
        }
        ItemIndex[] SortItems(ItemIndex[] items)
        {
            ClearLists();
            foreach (var item in items)
            {
                int yes = (int)ItemCatalog.GetItemDef(item).tier;
                if (yes > 5 && yes < 9)
                {
                    yes -= 6;
                }
                if (yes == 9)
                {
                    yes = 4;
                }
                TestList.Add(yes);
                //switch (ItemCatalog.GetItemDef(item).tier)
                //{
                //    case ItemTier.Tier1:
                //        Tier1.Add((int)item);
                //        break;
                //    case ItemTier.Tier2:
                //        Tier2.Add((int)item);
                //        break;
                //    case ItemTier.Tier3:
                //        Tier3.Add((int)item);
                //        break;
                //    case ItemTier.Lunar:
                //        Lunar.Add((int)item);
                //        break;
                //    case ItemTier.Boss:
                //        Boss.Add((int)item);
                //        break;
                //    case ItemTier.NoTier:
                //        NoTier.Add((int)item);
                //        break;
                //    case ItemTier.VoidTier1:
                //        VoidTier1.Add((int)item);
                //        break;
                //    case ItemTier.VoidTier2:
                //        VoidTier2.Add((int)item);
                //        break;
                //    case ItemTier.VoidTier3:
                //        VoidTier3.Add((int)item);
                //        break;
                //    case ItemTier.VoidBoss:
                //        VoidBoss.Add((int)item);
                //        break;
                //    case ItemTier.AssignedAtRuntime:
                //        AssignedAtRuntime.Add((int)item);
                //        break;
                //    default:
                //        break;
                //}
            }
            Array.Sort(TestList.ToArray(), items);
            Array.Reverse(TestList.ToArray());
            return items;
        }
        void ClearLists()
        {
            TestList.Clear();
            Tier1.Clear();
            Tier2.Clear();
            Tier3.Clear();
            Lunar.Clear();
            Boss.Clear();
            NoTier.Clear();
            VoidTier1.Clear();
            VoidTier2.Clear();
            VoidTier3.Clear();
            VoidBoss.Clear();
            AssignedAtRuntime.Clear();
        }
    }
}
