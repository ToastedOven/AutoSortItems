using System;
using System.Collections.Generic;
using System.Text;
using RiskOfOptions;
using RiskOfOptions.Options;

namespace AutoSortPlugin
{
    public static class ROO
    {
        public static void rooooooo()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(AutoSortPlugin.SeperateScrap, false));
            ModSettingsManager.AddOption(new CheckBoxOption(AutoSortPlugin.SortByTier, false));
            ModSettingsManager.AddOption(new CheckBoxOption(AutoSortPlugin.DescendingTier, false));
            ModSettingsManager.AddOption(new CheckBoxOption(AutoSortPlugin.SortByStackSize, false));
            ModSettingsManager.AddOption(new CheckBoxOption(AutoSortPlugin.DescendingStackSize, false));
        }
    }
}
