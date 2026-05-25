using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private readonly Dictionary<string, InventoryEntry> items = new Dictionary<string, InventoryEntry>();

    public bool HasItem(string itemName)
    {
        return string.IsNullOrEmpty(itemName) || items.ContainsKey(Normalize(itemName));
    }

    public void AddItem(string itemName, string displayName, int quantity = 1)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            return;
        }

        string normalized = Normalize(itemName);
        if (!items.ContainsKey(normalized))
        {
            items.Add(normalized, new InventoryEntry(string.IsNullOrEmpty(displayName) ? itemName : displayName));
        }

        items[normalized].Count += Mathf.Max(1, quantity);
        HUDController.Instance?.SetInventory(GetInventoryText());
    }

    public bool ConsumeItem(string itemName)
    {
        if (!HasItem(itemName))
        {
            return false;
        }

        string normalized = Normalize(itemName);
        items[normalized].Count--;
        if (items[normalized].Count <= 0)
        {
            items.Remove(normalized);
        }

        HUDController.Instance?.SetInventory(GetInventoryText());
        return true;
    }

    private string GetInventoryText()
    {
        if (items.Count == 0)
        {
            return "No items";
        }

        string text = "";
        foreach (InventoryEntry item in items.Values)
        {
            if (text.Length > 0)
            {
                text += "\n";
            }

            text += "- " + item.DisplayName;
            if (item.Count > 1)
            {
                text += " x" + item.Count;
            }
        }

        return text;
    }

    private static string Normalize(string itemName)
    {
        return itemName.Trim().ToLowerInvariant();
    }

    private class InventoryEntry
    {
        public readonly string DisplayName;
        public int Count;

        public InventoryEntry(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
