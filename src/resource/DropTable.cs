using Godot;
using Godot.Collections;

namespace BitBuster.resource;

public partial class DropTable : Resource
{
	[Export] 
	public Array<Drop> DropsList { get; set; }
}
