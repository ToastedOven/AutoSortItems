using BepInEx;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Linq;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using MonoMod.RuntimeDetour;

namespace ExamplePlugin
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class AutoSortPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Nunchuk";
        public const string PluginName = "AutoSortItems";
        public const string PluginVersion = "1.0.0";

        public static ConfigEntry<bool> SeperateScrap;
        public static ConfigEntry<bool> SortByTier;
        public static ConfigEntry<bool> DescendingTier;
        public static ConfigEntry<bool> SortByStackSize;
        public static ConfigEntry<bool> DescendingStackSize;


        public static AutoSortPlugin instance;
        RoR2.UI.ItemInventoryDisplay display;
        List<List<ItemIndex>> itemTierLists = new List<List<ItemIndex>>();
        List<ItemIndex> scrapList = new List<ItemIndex>();
        List<ItemIndex> noTierList = new List<ItemIndex>();
        private static Hook overrideHook;

        void InitHooks()
        {
            var targetMethod = typeof(RoR2.UI.ItemInventoryDisplay).GetMethod(nameof(RoR2.UI.ItemInventoryDisplay.UpdateDisplay), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(AutoSortPlugin).GetMethod(nameof(UpdateDisplayOverride), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overrideHook = new Hook(targetMethod, destMethod, this);
        }
        public void Awake()
        {
            instance = this;
            Log.Init(Logger);
            SeperateScrap = instance.Config.Bind<bool>("Settings", "Seperate Scrap", true, "Sort's by Scrap");
            SortByTier = instance.Config.Bind<bool>("Settings", "Tier Sort", true, "Sort's by Tier");
            DescendingTier = instance.Config.Bind<bool>("Settings", "Descending Tier Sort", true, "Sort's by Tier Descending");
            SortByStackSize = instance.Config.Bind<bool>("Settings", "Stack Size Sort", true, "Sort's by Stack Size");
            DescendingStackSize = instance.Config.Bind<bool>("Settings", "Descending Stack Size Sort", true, "Sort's by Stack Size Descending");

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                ModSettingsManager.AddOption(new CheckBoxOption(SeperateScrap, false));
                ModSettingsManager.AddOption(new CheckBoxOption(SortByTier, false));
                ModSettingsManager.AddOption(new CheckBoxOption(DescendingTier, false));
                ModSettingsManager.AddOption(new CheckBoxOption(SortByStackSize, false));
                ModSettingsManager.AddOption(new CheckBoxOption(DescendingStackSize, false));
            }
            SeperateScrap.SettingChanged += SettingsChanged;
            SortByTier.SettingChanged += SettingsChanged;
            DescendingTier.SettingChanged += SettingsChanged;
            SortByStackSize.SettingChanged += SettingsChanged;
            DescendingStackSize.SettingChanged += SettingsChanged;


            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += ContentManager_onContentPacksAssigned;
            InitHooks();
        }
        private void UpdateDisplayOverride(Action<RoR2.UI.ItemInventoryDisplay> orig, RoR2.UI.ItemInventoryDisplay self)
        {
            display = self;
            var temp = self.itemOrder;
            self.itemOrder = SortItems(self.itemOrder, self.itemOrderCount, self);
            orig(self);
            self.itemOrder = temp;
        }

        int noTierNum;
        private void ContentManager_onContentPacksAssigned(HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            foreach (var tierList in RoR2.ContentManagement.ContentManager.itemTierDefs)
            {
                if (tierList.tier.ToString() == "NoTier")
                {
                    noTierNum = itemTierLists.Count;
                }
                itemTierLists.Add(new List<ItemIndex>());
            }
        }

        private void SettingsChanged(object sender, EventArgs e)
        {
            try
            {
                if (display)
                {
                    display.UpdateDisplay();
                }
            }
            catch (Exception)
            {
            }
        }

        ItemIndex[] SortItems(ItemIndex[] items, int count, RoR2.UI.ItemInventoryDisplay display)
        {
            foreach (var tierList in itemTierLists)
            {
                tierList.Clear();
            }
            scrapList.Clear();
            noTierList.Clear();
            ItemIndex[] newArray = new ItemIndex[count];
            for (int i = 0; i < count; i++)
            {
                if (SeperateScrap.Value && (ItemCatalog.GetItemDef(items[i]).ContainsTag(ItemTag.Scrap) || ItemCatalog.GetItemDef(items[i]).ContainsTag(ItemTag.PriorityScrap)))
                {
                    scrapList.Add(items[i]);
                }
                else if (SortByTier.Value)
                {
                    if (ItemCatalog.GetItemDef(items[i]).tier == ItemTier.NoTier)
                    {
                        noTierList.Add(items[i]);
                    }
                    else
                    {
                        itemTierLists[(int)ItemCatalog.GetItemDef(items[i]).tier].Add(items[i]);
                    }
                }
                else
                {
                    newArray[i] = items[i];
                }
            }
            items = newArray;

            if (SortByTier.Value)
            {
                for (int i = 0; i < itemTierLists.Count; i++)
                {
                    itemTierLists[i] = new List<ItemIndex>(itemTierLists[i].OrderBy((item) => (
                    (int)item)
                    + ((DescendingStackSize.Value ? -1 : 1) * (SortByStackSize.Value ? 1 : 0) * display.itemStacks[(int)item] * 20000)).ToArray());
                }
                int num = 0;
                if (SeperateScrap.Value)
                {
                    for (int i = 0; i < scrapList.Count; i++)
                    {
                        items[num] = scrapList[i];
                        num++;
                    }
                }
                if (DescendingTier.Value)
                {
                    for (int i = itemTierLists.Count - 1; i > -1; i--)
                    {
                        for (int x = 0; x < itemTierLists[i].Count; x++)
                        {
                            items[num] = itemTierLists[i][x];
                            num++;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < itemTierLists.Count; i++)
                    {
                        for (int x = 0; x < itemTierLists[i].Count; x++)
                        {
                            items[num] = itemTierLists[i][x];
                            num++;
                        }
                    }
                }
                for (int i = 0; i < noTierList.Count; i++)
                {
                    items[num] = noTierList[i];
                    num++;
                }
            }
            else
            {
                items = items.OrderBy((item) =>
                +((int)item)
                + ((DescendingStackSize.Value ? -1 : 1) * (SortByStackSize.Value ? 1 : 0) * display.itemStacks[(int)item] * 20000)
                ).Distinct().ToArray();
                if (SeperateScrap.Value)
                {
                    int num = 0;
                    for (int i = count - scrapList.Count; i < count; i++)
                    {
                        items[i] = scrapList[num++];
                    }
                }
            }
            return items;
        }
    }
}
