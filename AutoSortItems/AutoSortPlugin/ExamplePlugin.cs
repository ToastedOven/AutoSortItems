using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ExamplePlugin
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ExamplePlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Nunchuk";
        public const string PluginName = "AutoSortItems";
        public const string PluginVersion = "1.0.0";


        public void Awake()
        {
            Log.Init(Logger);
        }
    }
}
