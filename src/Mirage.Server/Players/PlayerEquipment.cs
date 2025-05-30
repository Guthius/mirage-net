using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Repositories;
using Mirage.Shared.Constants;
using Mirage.Shared.Data;

namespace Mirage.Server.Players;

public sealed class PlayerEquipment
{
    private readonly Player _player;
    private readonly IRepository<ItemInfo> _itemRepository;

    public PlayerEquipmentSlot? Weapon { get; set; }
    public PlayerEquipmentSlot? Armor { get; set; }
    public PlayerEquipmentSlot? Helmet { get; set; }
    public PlayerEquipmentSlot? Shield { get; set; }

    public PlayerEquipment(Player player, IRepository<ItemInfo> itemRepository)
    {
        _player = player;
        _itemRepository = itemRepository;

        Weapon = BuildSlot(player.Character.Inventory.Equipment.Weapon);
        Armor = BuildSlot(player.Character.Inventory.Equipment.Armor);
        Helmet = BuildSlot(player.Character.Inventory.Equipment.Helmet);
        Shield = BuildSlot(player.Character.Inventory.Equipment.Shield);
    }

    private PlayerEquipmentSlot? BuildSlot(EquipmentSlotInfo? slotInfo)
    {
        if (slotInfo is null)
        {
            return null;
        }

        var itemInfo = _itemRepository.Get(slotInfo.ItemId);
        if (itemInfo is null)
        {
            return null;
        }

        return new PlayerEquipmentSlot
        {
            Item = itemInfo,
            Durability = slotInfo.Durability
        };
    }

    public void SendToPlayer()
    {
        _player.Send(new UpdateEquipmentCommand(ToSlot(Weapon), ToSlot(Armor), ToSlot(Helmet), ToSlot(Shield)));

        static UpdateEquipmentCommand.Slot? ToSlot(PlayerEquipmentSlot? slot)
        {
            if (slot is null)
            {
                return null;
            }

            return new UpdateEquipmentCommand.Slot(
                slot.Item.Sprite,
                slot.Item.Name,
                slot.Item.Damage,
                slot.Item.Protection);
        }
    }

    public void ReduceDurability(EquipmentType equipmentType)
    {
        var equipmentSlot = equipmentType switch
        {
            EquipmentType.Weapon => Weapon,
            EquipmentType.Armor => Armor,
            EquipmentType.Helmet => Helmet,
            EquipmentType.Shield => Shield,
            _ => null
        };

        if (equipmentSlot is null)
        {
            return;
        }

        equipmentSlot.Durability--;
        switch (equipmentSlot.Durability)
        {
            case <= 0:
                _player.Tell($"Your {equipmentSlot.Item.Name} has broken!", ColorCode.Red);
                ClearSlot(equipmentType);
                return;

            case <= 5:
                _player.Tell($"Your {equipmentSlot.Item.Name} is about to break!", ColorCode.Yellow);
                break;
        }
    }

    public void ClearSlot(EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                Weapon = null;
                break;

            case EquipmentType.Armor:
                Armor = null;
                break;

            case EquipmentType.Helmet:
                Helmet = null;
                break;

            case EquipmentType.Shield:
                Shield = null;
                break;
        }

        SendToPlayer();
    }

    public void Clear()
    {
        Weapon = Armor = Helmet = Shield = null;

        SendToPlayer();
    }
}