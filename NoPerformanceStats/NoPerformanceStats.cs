﻿using HarmonyLib;
using MelonLoader;
using System;
using System.Collections;
using System.Reflection;
using VRC.SDKBase.Validation.Performance;

[assembly: MelonInfo(typeof(NoPerformanceStats.NoPerformanceStats), "NoPerformanceStats", "1.0.7", "ImTiara", "https://github.com/ImTiara/VRCMods")]
[assembly: MelonGame("VRChat", "VRChat")]

namespace NoPerformanceStats
{
    public class NoPerformanceStats : MelonMod
    {
        public static MelonLogger.Instance Logger;

        private static bool allowPerformanceScanner;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance(GetType().Name);

            MelonPreferences.CreateCategory(GetType().Name, "No Performance Stats");
            MelonPreferences.CreateEntry(GetType().Name, "DisablePerformanceStats", true, "Disable Performance Stats");

            ApplyPatches();

            MelonCoroutines.Start(UiManagerInitializer());
        }

        private void OnUiManagerInit()
        {
            OnPreferencesSaved();
        }

        public override void OnPreferencesSaved()
        {
            bool disablePerformanceStats = MelonPreferences.GetEntryValue<bool>(GetType().Name, "DisablePerformanceStats");

            if (disablePerformanceStats == allowPerformanceScanner)
            {
                allowPerformanceScanner = !disablePerformanceStats;

                Logger.Msg($"Performance stats will now be {(allowPerformanceScanner ? "shown" : "hidden and avatars should load quicker")}.");
            }
        }

        private void ApplyPatches()
        {
            try
            {
                Patch(
                    typeof(PerformanceScannerSet),
                    nameof(PerformanceScannerSet.Method_Public_IEnumerator_GameObject_AvatarPerformanceStats_MulticastDelegateNPublicSealedBoCoUnique_0),

                    typeof(NoPerformanceStats),
                    nameof(CalculatePerformance),

                    BindingFlags.Public | BindingFlags.Instance,
                    BindingFlags.NonPublic | BindingFlags.Static
                );
            }
            catch (Exception e) { Logger.Error("Failed to patch Performance Scanner: " + e); }
        }

        private static bool CalculatePerformance()
            => allowPerformanceScanner;

        private void Patch(Type targetClass, string target, Type detourClass, string detour, BindingFlags targetBindingFlags = BindingFlags.Public | BindingFlags.Instance, BindingFlags detourBindingFlags = BindingFlags.NonPublic | BindingFlags.Static)
            => HarmonyInstance.Patch(targetClass.GetMethod(target, targetBindingFlags), new HarmonyMethod(detourClass.GetMethod(detour, detourBindingFlags)), null, null);

        private IEnumerator UiManagerInitializer()
        {
            while (VRCUiManager.prop_VRCUiManager_0 == null) yield return null;
            OnUiManagerInit();
        }
    }
}