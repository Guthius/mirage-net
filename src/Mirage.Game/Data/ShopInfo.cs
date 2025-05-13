using Mirage.Game.Constants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mirage.Game.Data;

[BsonIgnoreExtraElements]
public sealed record ShopInfo
{
    [BsonElement("id"), BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; } = string.Empty;

    [BsonElement("join_say"), BsonRepresentation(BsonType.String)]
    public string JoinSay { get; set; } = string.Empty;

    [BsonElement("leave_say"), BsonRepresentation(BsonType.String)]
    public string LeaveSay { get; set; } = string.Empty;

    [BsonElement("fixes_items"), BsonRepresentation(BsonType.Boolean)]
    public bool FixesItems { get; set; }

    [BsonElement("trades")]
    public ShopTradeInfo[] Trades { get; set; } = CreateTradeItems();

    private static ShopTradeInfo[] CreateTradeItems()
    {
        var tradeItems = new ShopTradeInfo[Limits.MaxShopTrades + 1];

        for (var slot = 0; slot < tradeItems.Length; slot++)
        {
            tradeItems[slot] = new ShopTradeInfo();
        }

        return tradeItems;
    }
}