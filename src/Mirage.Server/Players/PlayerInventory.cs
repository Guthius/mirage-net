using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Repositories;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Players;

public sealed class PlayerInventory
{
    private readonly Dictionary<int, PlayerInventorySlot> _slots = [];
    private readonly Player _player;

    public int Size { get; }
    public PlayerEquipment Equipment { get; }

    public PlayerInventory(Player player, IRepository<ItemInfo> itemRepository)
    {
        _player = player;

        var invalidSlots = new List<int>();

        foreach (var (slotIndex, slot) in player.Character.Inventory.Slots)
        {
            var itemInfo = itemRepository.Get(slot.ItemId);
            if (itemInfo is null)
            {
                invalidSlots.Add(slotIndex);
                continue;
            }

            _slots.Add(slotIndex, new PlayerInventorySlot
            {
                Item = itemInfo,
                Quantity = slot.Quantity,
                Durability = slot.Durability
            });
        }

        foreach (var slot in invalidSlots)
        {
            player.Character.Inventory.Slots.Remove(slot);
        }

        Size = player.Character.Inventory.Size;
        Equipment = new PlayerEquipment(player, itemRepository);
    }

    public void SendToPlayer()
    {
        _player.Send(new UpdateInventoryCommand(Size));

        foreach (var (slot, slotInfo) in _slots)
        {
            _player.Send(new UpdateInventorySlotCommand(slot,
                slotInfo.Item.Type,
                slotInfo.Item.Sprite,
                slotInfo.Item.Name,
                slotInfo.Quantity));
        }

        Equipment.SendToPlayer();
    }

    private void Clear(int slotIndex)
    {
        if (!_slots.Remove(slotIndex))
        {
            return;
        }

        _player.Send(new ClearInventorySlotCommand(slotIndex));
    }

    public bool Add(ItemInfo itemInfo, int quantity, int? durability)
    {
        if (itemInfo.Type == ItemType.Currency)
        {
            return AddCurrency(itemInfo, quantity);
        }

        var freeSlots = Size - _slots.Count;
        if (freeSlots < quantity)
        {
            return false;
        }

        for (var slotIndex = 0; slotIndex < Size; slotIndex++)
        {
            if (_slots.ContainsKey(slotIndex))
            {
                continue;
            }

            durability ??= itemInfo.Durability;

            _slots[slotIndex] = new PlayerInventorySlot
            {
                Item = itemInfo,
                Quantity = 1,
                Durability = durability.Value
            };

            _player.Send(new UpdateInventorySlotCommand(slotIndex, itemInfo.Type, itemInfo.Sprite, itemInfo.Name, quantity));

            quantity--;
            if (quantity <= 0)
            {
                break;
            }
        }

        return true;
    }

    private bool AddCurrency(ItemInfo itemInfo, int quantity = 1)
    {
        foreach (var (slotIndex, slot) in _slots)
        {
            if (slot.Item.Id != itemInfo.Id)
            {
                continue;
            }

            slot.Quantity += quantity;

            _player.Send(new UpdateInventorySlotCommand(slotIndex, itemInfo.Type, itemInfo.Sprite, itemInfo.Name, slot.Quantity));
            return true;
        }

        for (var slotIndex = 0; slotIndex < Size; slotIndex++)
        {
            if (_slots.ContainsKey(slotIndex))
            {
                continue;
            }

            _slots[slotIndex] = new PlayerInventorySlot
            {
                Item = itemInfo,
                Quantity = quantity
            };

            _player.Send(new UpdateInventorySlotCommand(slotIndex, itemInfo.Type, itemInfo.Sprite, itemInfo.Name, quantity));
            return true;
        }

        return false;
    }

    public bool Remove(string itemId, int quantity = 1)
    {
        if (!Contains(itemId, quantity))
        {
            return false;
        }

        foreach (var (slot, slotInfo) in _slots.Where(x => x.Value.Item.Id == itemId))
        {
            if (quantity < slotInfo.Quantity)
            {
                slotInfo.Quantity -= quantity;

                _player.Send(new UpdateInventorySlotQuantityCommand(slot, slotInfo.Quantity));
                break;
            }

            Clear(slot);

            quantity -= slotInfo.Quantity;
        }

        return true;
    }

    public void Drop(int slotIndex, int quantity)
    {
        if (quantity < 1)
        {
            return;
        }

        var slot = _slots.GetValueOrDefault(slotIndex);
        if (slot is null)
        {
            return;
        }

        if (slot.Item.Type == ItemType.Currency)
        {
            quantity = Math.Clamp(quantity, 1, slot.Quantity);

            _player.Map.SpawnItem(slot.Item, _player.Character.X, _player.Character.Y, quantity);

            slot.Quantity -= quantity;
            if (slot.Quantity > 0)
            {
                _player.Send(new UpdateInventorySlotQuantityCommand(slotIndex, slot.Quantity));
            }
            else
            {
                Clear(slotIndex);
            }

            _player.Map.SendMessage($"{_player.Character.Name} drops {quantity} {slot.Item.Name}.", ColorCode.Yellow);
            return;
        }

        Clear(slotIndex);

        _player.Map.SpawnItem(slot.Item,
            _player.Character.X,
            _player.Character.Y, 1);

        _player.Map.SendMessage(slot.Item.IsEquipment
                ? $"{_player.Character.Name} drops a {slot.Item.Name} {slot.Durability}/{slot.Item.Durability}."
                : $"{_player.Character.Name} drops a {slot.Item.Name}.",
            ColorCode.Yellow);
    }

    public bool Contains(string itemId, int quantity = 1)
    {
        foreach (var slotInfo in _slots.Values.Where(x => x.Item.Id == itemId))
        {
            quantity -= slotInfo.Quantity;
            if (quantity <= 0)
            {
                return true;
            }
        }

        return false;
    }

    public void Use(int slotIndex)
    {
        var slot = _slots.GetValueOrDefault(slotIndex);
        if (slot is null)
        {
            return;
        }

        switch (slot.Item.Type)
        {
            case ItemType.Armor:
            case ItemType.Weapon:
            case ItemType.Helmet:
            case ItemType.Shield:
                UseEquipment(slotIndex, slot);
                break;

            case ItemType.PotionAddHp:
            case ItemType.PotionAddMp:
            case ItemType.PotionAddSp:
            case ItemType.PotionSubHp:
            case ItemType.PotionSubMp:
            case ItemType.PotionSubSp:
                UsePotion(slotIndex, slot.Item.Type, slot.Item.PotionStrength);
                break;
        }
    }

    private void UseEquipment(int slotIndex, PlayerInventorySlot slot)
    {
        switch (slot.Item.Type)
        {
            case ItemType.Armor:
                if (_player.Character.Defense < slot.Item.RequiredDefense)
                {
                    _player.Tell(
                        $"Your defense is to low to wear this armor! Required DEF ({slot.Item.RequiredDefense})",
                        ColorCode.BrightRed);

                    return;
                }

                Equip(slotIndex, slot);
                break;

            case ItemType.Weapon:
                if (_player.Character.Strength < slot.Item.RequiredStrength)
                {
                    _player.Tell(
                        $"Your strength is to low to wear this armor! Required STR ({slot.Item.RequiredStrength})",
                        ColorCode.BrightRed);

                    return;
                }

                Equip(slotIndex, slot);
                break;

            case ItemType.Helmet:
                if (_player.Character.Speed < slot.Item.RequiredSpeed)
                {
                    _player.Tell(
                        $"Your speed coordination is to low to wear this helmet! Required SPEED ({slot.Item.RequiredSpeed})",
                        ColorCode.BrightRed);

                    return;
                }

                Equip(slotIndex, slot);
                break;

            case ItemType.Shield:
                Equip(slotIndex, slot);
                break;
        }
    }

    private void UsePotion(int slotIndex, ItemType itemType, int potionStrength)
    {
        Clear(slotIndex);

        switch (itemType)
        {
            case ItemType.PotionAddHp:
                _player.Character.Health += potionStrength;
                _player.SendVitals();
                break;

            case ItemType.PotionAddMp:
                _player.Character.Mana += potionStrength;
                _player.SendVitals();
                break;

            case ItemType.PotionAddSp:
                _player.Character.Stamina += potionStrength;
                _player.SendVitals();
                break;

            case ItemType.PotionSubHp:
                _player.Character.Health -= potionStrength;
                _player.SendVitals();
                break;

            case ItemType.PotionSubMp:
                _player.Character.Mana -= potionStrength;
                _player.SendVitals();
                break;

            case ItemType.PotionSubSp:
                _player.Character.Stamina -= potionStrength;
                _player.SendVitals();
                break;
        }
    }

    private void Equip(int slotIndex, PlayerInventorySlot slot)
    {
        var unequipped = slot.Item.Type switch
        {
            ItemType.Weapon => Equipment.Weapon,
            ItemType.Armor => Equipment.Armor,
            ItemType.Helmet => Equipment.Helmet,
            ItemType.Shield => Equipment.Shield,
            _ => null
        };

        switch (slot.Item.Type)
        {
            case ItemType.Weapon:
                Equipment.Weapon = new PlayerEquipmentSlot
                {
                    Item = slot.Item,
                    Durability = slot.Durability
                };
                break;

            case ItemType.Armor:
                Equipment.Armor = new PlayerEquipmentSlot
                {
                    Item = slot.Item,
                    Durability = slot.Durability
                };
                break;

            case ItemType.Helmet:
                Equipment.Helmet = new PlayerEquipmentSlot
                {
                    Item = slot.Item,
                    Durability = slot.Durability
                };
                break;

            case ItemType.Shield:
                Equipment.Shield = new PlayerEquipmentSlot
                {
                    Item = slot.Item,
                    Durability = slot.Durability
                };
                break;
        }

        if (unequipped is not null)
        {
            slot.Item = unequipped.Item;
            slot.Quantity = 1;
            slot.Durability = unequipped.Durability;

            _player.Send(new UpdateInventorySlotCommand(slotIndex,
                slot.Item.Type,
                slot.Item.Sprite,
                slot.Item.Name,
                slot.Quantity));
        }
        else
        {
            Clear(slotIndex);
        }

        Equipment.SendToPlayer();
    }

    public void UpdateCharacter()
    {
        _player.Character.Inventory.Size = Size;
        _player.Character.Inventory.Slots.Clear();

        foreach (var (slotIndex, slot) in _slots)
        {
            _player.Character.Inventory.Slots.Add(slotIndex, new InventorySlotInfo
            {
                ItemId = slot.Item.Id,
                Quantity = slot.Quantity,
                Durability = slot.Durability
            });
        }

        _player.Character.Inventory.Equipment.Weapon = MapEquipment(Equipment.Weapon);
        _player.Character.Inventory.Equipment.Armor = MapEquipment(Equipment.Armor);
        _player.Character.Inventory.Equipment.Helmet = MapEquipment(Equipment.Helmet);
        _player.Character.Inventory.Equipment.Shield = MapEquipment(Equipment.Shield);

        static EquipmentSlotInfo? MapEquipment(PlayerEquipmentSlot? slot)
        {
            if (slot is null)
            {
                return null;
            }

            return new EquipmentSlotInfo
            {
                ItemId = slot.Item.Id,
                Durability = slot.Durability
            };
        }
    }
}