using Mirage.Shared.Data;

namespace Mirage.Client.Inventory;

public sealed class InventoryStore
{
    private readonly Lock _slotsLock = new();
    private readonly Dictionary<int, InventorySlot> _slots = [];
    private Dictionary<int, InventorySlot> _slotsSnapshot = [];

    public int Size { get; set; }
    public IReadOnlyDictionary<int, InventorySlot> Slots => _slotsSnapshot;
    public EquipmentSlot? Weapon { get; set; }
    public EquipmentSlot? Armor { get; set; }
    public EquipmentSlot? Helmet { get; set; }
    public EquipmentSlot? Shield { get; set; }

    public void Reset()
    {
        Size = 0;

        lock (_slotsLock)
        {
            _slots.Clear();
            _slotsSnapshot = [];
        }
    }

    public void Clear(int slotIndex)
    {
        lock (_slotsLock)
        {
            if (_slots.Remove(slotIndex))
            {
                _slotsSnapshot = _slots.ToDictionary(x => x.Key, x => x.Value);
            }
        }
    }

    public void Update(int slotIndex, ItemType type, int sprite, string itemName, int quantity)
    {
        lock (_slotsLock)
        {
            _slots[slotIndex] = new InventorySlot(type, sprite, itemName)
            {
                Quantity = quantity
            };

            _slotsSnapshot = _slots.ToDictionary(x => x.Key, x => x.Value);
        }
    }

    public void UpdateQuantity(int slotIndex, int quantity)
    {
        lock (_slotsLock)
        {
            if (!_slots.TryGetValue(slotIndex, out var slot))
            {
                return;
            }

            slot.Quantity = quantity;

            _slotsSnapshot = _slots.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}