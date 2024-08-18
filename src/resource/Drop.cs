using BitBuster.item;
using BitBuster.world;
using Godot;

namespace BitBuster.resource;

public partial class Drop : Resource
{
    [Export] 
    public ItemType ItemType { get; set; }
    
    [Export]
    public int ItemId { get; set; }
    
    [Export]
    public float Chance { get; set; }

    [Export] public int MaxAmount { get; set; } = 1;
}