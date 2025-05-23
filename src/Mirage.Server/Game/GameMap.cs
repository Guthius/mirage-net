﻿using Mirage.Game.Constants;
using Mirage.Game.Data;
using Mirage.Net;
using Mirage.Net.Protocol.FromServer;
using Mirage.Server.Game.Managers;
using Mirage.Server.Net;

namespace Mirage.Server.Game;

public sealed class GameMap
{
    private readonly MapItemInfo?[] _items = new MapItemInfo?[Limits.MaxMapItems + 1];
    private readonly GameNpc[] _npcs;

    public MapInfo Info { get; private set; }
    public bool PlayersOnMap { get; set; }
    public bool[,] DoorOpen { get; } = new bool[Limits.MaxMapWidth + 1, Limits.MaxMapHeight + 1];
    public long DoorTimer { get; set; }
    
    public GameMap(MapInfo mapInfo)
    {
        Info = mapInfo;

        _npcs = new GameNpc[Limits.MaxMapNpcs + 1];

        for (var slot = 1; slot <= Limits.MaxMapNpcs; slot++)
        {
            var npcId = Info.NpcIds[slot];
            var npcInfo = NpcManager.Get(npcId);

            _npcs[slot] = new GameNpc(slot, this, npcInfo);
        }

        RespawnNpcs();
    }

    public GameNpc? GetNpc(int slot)
    {
        if (slot is <= 0 or > Limits.MaxMapNpcs)
        {
            return null;
        }

        return _npcs[slot];
    }

    public MapItemInfo? GetItem(int slot)
    {
        if (slot is <= 0 or > Limits.MaxMapItems)
        {
            return null;
        }
        
        return _items[slot];
    }

    public MapItemInfo? GetItemAt(int x, int y)
    {
        return _items.OfType<MapItemInfo>().FirstOrDefault(item => item.X == x && item.Y == y);
    }

    public void Update()
    {
        var tickCount = Environment.TickCount;

        if (!PlayersOnMap)
        {
            return;
        }

        CloseDoors(tickCount);

        for (var i = 1; i <= Limits.MaxMapNpcs; i++)
        {
            _npcs[i].Update(tickCount);
        }
    }

    public void UpdateInfo(MapInfo mapInfo)
    {
        Info = mapInfo;

        MapManager.Update(Info.Id, Info);

        RespawnNpcs();
    }

    public void RespawnItems()
    {
        for (var slot = 1; slot <= Limits.MaxMapItems; slot++)
        {
            _items[slot] = null;
        }

        for (var y = 0; y <= Limits.MaxMapHeight; y++)
        {
            for (var x = 0; x <= Limits.MaxMapWidth; x++)
            {
                if (Info.Tiles[x, y].Type != TileType.Item)
                {
                    continue;
                }

                var itemInfo = ItemManager.Get(Info.Tiles[x, y].Data1);
                if (itemInfo is null)
                {
                    continue;
                }

                if (itemInfo.Type == ItemType.Currency && Info.Tiles[x, y].Data2 <= 0)
                {
                    SpawnItem(x, y, Info.Tiles[x, y].Data1, 1);
                }
                else
                {
                    SpawnItem(x, y, Info.Tiles[x, y].Data1, Info.Tiles[x, y].Data2);
                }
            }
        }

        Send(new MapItemData(_items));
    }

    public void RespawnNpcs()
    {
        for (var slot = 1; slot <= Limits.MaxMapNpcs; slot++)
        {
            var npcId = Info.NpcIds[slot];
            if (npcId <= 0)
            {
                _npcs[slot].Reset();
                continue;
            }

            var npcInfo = NpcManager.Get(npcId);
            if (npcInfo is null)
            {
                _npcs[slot].Reset();
                continue;
            }

            _npcs[slot].Reset(npcInfo);
            _npcs[slot].Respawn();
        }

        Send(new MapNpcData(GetNpcData()));
    }

    private int FindOpenMapItemSlot()
    {
        for (var i = 1; i < Limits.MaxMapItems; i++)
        {
            if (_items[i] is null)
            {
                return i;
            }
        }

        return 0;
    }

    public bool SpawnItem(int x, int y, int itemId, int quantity, int? durability = null)
    {
        if (itemId is <= 0 or > Limits.MaxItems)
        {
            return false;
        }

        var itemInfo = ItemManager.Get(itemId);
        if (itemInfo is null)
        {
            return false;
        }

        var slot = FindOpenMapItemSlot();

        SpawnItemSlot(slot, x, y, itemInfo, quantity, durability ?? itemInfo.Data1);

        return true;
    }

    private void SpawnItemSlot(int slot, int x, int y, ItemInfo itemInfo, int quantity, int durability)
    {
        if (slot is <= 0 or > Limits.MaxMapItems)
        {
            return;
        }

        if (itemInfo.Type is < ItemType.Weapon or > ItemType.Shield)
        {
            durability = 0;
        }

        var item = new MapItemInfo(itemInfo.Id, quantity, durability, x, y);

        _items[slot] = item;

        Send(new SpawnItem(slot, itemInfo.Id, item.Value, item.Dur, item.X, item.Y));
    }

    public void ClearItem(int slot)
    {
        if (slot is <= 0 or > Limits.MaxMapItems)
        {
            return;
        }

        _items[slot] = null;

        Send(Mirage.Net.Protocol.FromServer.SpawnItem.Cleared(slot));
    }

    public void CloseDoors(int tickCount)
    {
        if (tickCount <= DoorTimer + 5000)
        {
            return;
        }

        for (var y = 0; y <= Limits.MaxMapHeight; y++)
        {
            for (var x = 0; x <= Limits.MaxMapWidth; x++)
            {
                if (!DoorOpen[x, y])
                {
                    continue;
                }

                DoorOpen[x, y] = false;

                Send(new MapKey(x, y, false));
            }
        }
    }

    public MapItemInfo?[] GetItemData()
    {
        return _items;
    }
    
    public MapNpcInfo[] GetNpcData()
    {
        return _npcs.Skip(1)
            .Select(npc => new MapNpcInfo(
                NpcId: npc.Alive ? npc.Info.Id : 0,
                X: npc.X,
                Y: npc.Y,
                Direction: npc.Direction))
            .ToArray();
    }

    public IEnumerable<GameNpc> AliveNpcs()
    {
        return _npcs.Skip(1).Where(npc => npc.Alive);
    }

    public void Send<TPacket>(TPacket packet) where TPacket : IPacket<TPacket>
    {
        var bytes = PacketSerializer.GetBytes(packet);

        foreach (var player in GameState.OnlinePlayers())
        {
            if (player.Map.Info.Id != Info.Id)
            {
                continue;
            }

            Network.SendData(player.Id, bytes);
        }
    }

    public void Send<TPacket>(int excludePlayerId, TPacket packet) where TPacket : IPacket<TPacket>
    {
        var bytes = PacketSerializer.GetBytes(packet);

        foreach (var player in GameState.OnlinePlayers())
        {
            if (player.Id != excludePlayerId && player.Map.Info.Id != Info.Id)
            {
                continue;
            }

            Network.SendData(player.Id, bytes);
        }
    }

    public void SendMessage(string message, int color)
    {
        Send(new MapMessage(message, color));
    }
}