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

        List<int> tierList = new List<int>();
        List<int> indexList = new List<int>();
        List<ItemIndex> tempList = new List<ItemIndex>();

        public void Awake() /////////////////////////////////////////////////////////TODO: Cleanup code, fix mithrix returning items visually is jank
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
                indexList.Add((int)item);

            Array.Sort(indexList.ToArray(), items);

            foreach (var item in items)
                tierList.Add((int)ItemCatalog.GetItemDef(item).tier);

            Array.Sort(tierList.ToArray(), items);
            return items;
        }
        void ClearLists()
        {
            tierList.Clear();
            indexList.Clear();
            tempList.Clear();
        }
    }
}
