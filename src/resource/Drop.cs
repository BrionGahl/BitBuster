using BitBuster.world;
using Godot;

namespace BitBuster.resource;

public partial class Drop : Resource
{
    [Export] 
    public ItemType ItemType { get; set; }
    
    [Export]
    public int ItemIndex { get; set; }
    
    [Export]
    public float Chance { get; set; }
}