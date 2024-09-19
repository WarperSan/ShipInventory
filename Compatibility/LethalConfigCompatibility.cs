﻿using System.Runtime.CompilerServices;
using CSync.Lib;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using ShipInventory.Helpers;
using UnityEngine;

namespace ShipInventory.Compatibility;

public static class LethalConfigCompatibility
{
    public const string LETHAL_CONFIG = "ainavt.lc.lethalconfig";
    private static bool? _enabled;

    public static bool enabled {
        get
        {
            _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LETHAL_CONFIG);
            return _enabled.Value;
        }
    }
    
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void AddConfigs(Config config)
    {
        LethalConfigManager.SetModIcon(Bundle.LoadAsset<Sprite>(Constants.MOD_ICON));
        LethalConfigManager.SetModDescription("Adds an inventory to the ship, allowing it to store items and retrieve them.");
        
         #region Chute
         
         LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(config.Blacklist.Entry, new TextInputFieldOptions {
            Name = Lang.Get("NAME_BLACKLIST"),
            TrimText = true,
            NumberOfLines = 10,
            RequiresRestart = false,
        }));
        
        LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(config.SpawnDelay.Entry, new FloatInputFieldOptions {
            Name = Lang.Get("NAME_SPAWN_DELAY"),
            Min = 0,
            Max = float.MaxValue,
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(config.RequireInOrbit.Entry, new BoolCheckBoxOptions {
            Name = Lang.Get("NAME_REQUIRES_IN_ORBIT"),
            RequiresRestart = false
        }));

        LethalConfigManager.AddConfigItem(new IntSliderConfigItem(config.StopAfter.Entry, new IntSliderOptions {
            Name = Lang.Get("NAME_STOP_AFTER"),
            Min = 1,
            Max = 1_969_420,
            RequiresRestart = false
        }));
        
        #endregion

        #region Inventory

        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(config.ActAsSafe.Entry, new BoolCheckBoxOptions {
            Name = Lang.Get("NAME_AS_SAFE"),
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new IntSliderConfigItem(config.MaxItemCount.Entry, new IntSliderOptions {
            Name = Lang.Get("NAME_MAX_ITEM_COUNT"),
            Min = 1,
            Max = 1_969_420,
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(config.PersistThroughFire.Entry, new BoolCheckBoxOptions {
            Name = Lang.Get("NAME_PERSIST_THROUGH_FIRE"),
            RequiresRestart = false
        }));

        #endregion
        
        #region Terminal

        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(config.ShowConfirmation.Entry, new BoolCheckBoxOptions {
            Name = Lang.Get("NAME_SHOW_CONFIRMATION"),
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(config.NoSteamID.Entry, new BoolCheckBoxOptions {
            Name = Lang.Get("NAME_NO_STEAM_ID"),
            RequiresRestart = false
        }));

        #endregion

        #region Debug
        
        LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<Config.OverrideMode>(config.OverrideTrigger.Entry, new EnumDropDownOptions {
            Name = Lang.Get("NAME_OVERRIDE_TRIGGER"),
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(config.LetAsItem.Entry, new BoolCheckBoxOptions {
            Name = Lang.Get("NAME_LET_AS_ITEM"),
            RequiresRestart = true
        }));

        #endregion
        
        LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(config.LangUsed, new TextInputFieldOptions {
            Description = "Language to use for the texts.\n\nThe translation has to be found next to the DLL.",
            RequiresRestart = true
        }));
        
        ConfigManager.Register(config); 
    }
}