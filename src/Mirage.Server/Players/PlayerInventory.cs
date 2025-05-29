using Mirage.Shared.Data;

namespace Mirage.Server.Players;

public sealed class PlayerInventory(CharacterInfo character)
{
    private readonly Dictionary<int, PlayerInventorySlot> _slots = [];

    private void Clear(int slot)
    {
        if (_slots.Remove(slot))
        {
            var s = character.Inventory.FirstOrDefault(x => x.Slot == slot);
            if (s is not null)
            {
                character.Inventory.Remove(s);
            }

            // TODO: Tell client to clear the inv slot...
        }
    }

    public bool Give(ItemInfo itemInfo, int quantity)
    {
        return false;
    }
    
    public bool Take(string itemId, int quantity)
    {
        if (!Contains(itemId, quantity))
        {
            return false;
        }

        foreach (var (y, x) in _slots.Where(x => x.Value.ItemId == itemId))
        {
            if (quantity > x.Quantity)
            {
                Clear(y);
                quantity -= x.Quantity;
                continue;
            }

            x.Quantity -= quantity;
            // TODO: Send qty update to client...
        }

        return true;
    }

    public bool Contains(string itemId, int quantity = 1)
    {
        foreach (var x in _slots.Values.Where(x => x.ItemId == itemId))
        {
            quantity -= x.Quantity;
            if (quantity <= 0)
            {
                return true;
            }
        }

        return false;
    }
}