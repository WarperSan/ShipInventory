﻿using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using ShipInventory.Objects;

namespace ShipInventory.Helpers;

public static class ItemManager
{
    /// <summary>
    /// List of the items stored in the ship's inventory
    /// </summary>
    private static IEnumerable<ItemData> storedItems = [];

    #region Getters

    /// <summary>
    /// Copies the items stored in the ship's inventory
    /// </summary>
    public static IEnumerable<ItemData> GetItems() => new List<ItemData>(
        storedItems.OrderBy(i => i.GetItemName()).ThenBy(i => i.SCRAP_VALUE)
    );

    public static IEnumerable<ItemData> GetInstances(ItemData data, int count)
    {
        // Take only one
        if (count == 1)
            return storedItems.Where(d => d.Equals(data)).Take(1);
        
        // Take all with id
        return storedItems.Where(d => d.ID == data.ID).Take(count);
    }

    public static int GetTotalValue() => storedItems.Sum(i => i.SCRAP_VALUE);

    #endregion
    #region Setters

    public static void SetItems(IEnumerable<ItemData> newItems, bool updateAll = false)
    {
        Logger.Debug($"Setting items from {storedItems.Count()} to {newItems.Count()}...");
        storedItems = newItems;
        
        ChuteInteract.Instance?.UpdateValue();
        
        if (updateAll)
            ChuteInteract.Instance?.RequestItemsAll();
    }

    /// <summary>
    /// Adds the given item to the cached inventory
    /// </summary>
    internal static void Add(ItemData data) => SetItems(storedItems.Append(data));

    /// <summary>
    /// Removes the given item from the cached inventory
    /// </summary>
    internal static void Remove(ItemData data)
    {
        var copy = storedItems.ToList();
        copy.Remove(data);
        SetItems(copy);
    }

    #endregion
    #region Store Item

    public static void StoreItem(GrabbableObject item)
    {
        if (ChuteInteract.Instance == null)
            return;

        ItemData data = new ItemData(item);
        
        item.OnBroughtToShip();
        
        // Send store request to server
        Logger.Debug("Sending new item to server...");
        ChuteInteract.Instance.StoreItemServerRpc(data);
    }

    #endregion
    #region Blacklist

    private static string[] BLACKLIST = [];
    internal static void UpdateBlacklist(string blacklistString)
    {
        BLACKLIST = blacklistString
            .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim().ToLower())
            .ToArray();
    }

    #endregion
    #region Trigger

    internal static void UpdateTrigger(InteractTrigger trigger, PlayerControllerB local)
    {
        // Holding nothing
        if (!local.isHoldingObject || local.currentlyHeldObjectServer == null)
        {
            trigger.interactable = false;
            trigger.disabledHoverTip = Lang.Get("NOT_HOLDING_ITEM");
            return;
        }

        #region Debug

        // Debug disable
        if (ShipInventory.Config.OverrideTrigger.Value == Config.OverrideMode.NEVER)
        {
            trigger.interactable = false;
            trigger.disabledHoverTip = "Disabled by HOST";
            return;
        }

        // Debug enable
        if (ShipInventory.Config.OverrideTrigger.Value == Config.OverrideMode.ALL)
        {
            trigger.interactable = true;
            return;
        }
        
        #endregion

        #region Permissions

        switch (ShipInventory.Config.ChutePermission.Value)
        {
            // No one
            case Config.PermissionLevel.NO_ONE:
            case Config.PermissionLevel.HOST_ONLY when !local.IsHost:
            case Config.PermissionLevel.CLIENTS_ONLY when local.IsHost:
                trigger.interactable = false;
                trigger.disabledHoverTip = Lang.Get("CHUTE_PERMISSION_MISSING");
                return;
            case Config.PermissionLevel.EVERYONE:
            default:
                break; // Nothing
        }

        #endregion

        // Not in orbit
        if (!StartOfRound.Instance.inShipPhase && ShipInventory.Config.RequireInOrbit.Value)
        {
            trigger.interactable = false;
            trigger.disabledHoverTip = Lang.Get("NOT_IN_ORBIT");
            return;
        }
        
        // Is Full
        if (storedItems.Count() == ShipInventory.Config.MaxItemCount.Value)
        {
            trigger.interactable = false;
            trigger.disabledHoverTip = Lang.Get("INVENTORY_FULL");
            return;
        }

        var item = local.currentlyHeldObjectServer;
        var properties = item.itemProperties;

        // If blacklisted
        if (BLACKLIST.Contains(properties.itemName.ToLower()))
        {
            trigger.interactable = false;
            trigger.disabledHoverTip = Lang.Get("ITEM_BLACKLISTED");
            return;
        }
        
        // If item not allowed
        if (properties.spawnPrefab == null || properties.spawnPrefab.GetComponent<RagdollGrabbableObject>() != null || item.itemUsedUp)
        {
            trigger.interactable = false;
            trigger.disabledHoverTip = Lang.Get("ITEM_NOT_ALLOWED");
            return;
        }
        
        trigger.interactable = local.isHoldingObject;
    }

    #endregion
}