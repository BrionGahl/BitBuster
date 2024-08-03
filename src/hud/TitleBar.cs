using BitBuster.component;
using BitBuster.items;
using Godot;

namespace BitBuster.hud;

public partial class TitleBar : VSplitContainer
{
	private AnimationPlayer _animationPlayer;
	private StatsComponent _playerStats;

	private Label _title;
	private Label _subtitle;
	
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_playerStats = GetTree().GetFirstNodeInGroup("player").GetNode<StatsComponent>("StatsComponent");

		_title = GetNode<Label>("Title");
		_subtitle = GetNode<Label>("Subtitle");
		
		_playerStats.ItemAdded += OnItemAdded;
	}

	private void SetLabels(string title, string subtitle)
	{
		_title.Text = title;
		_subtitle.Text = subtitle;
	}
	
	private void OnItemAdded(Item item)
	{
		SetLabels(item.ItemName, item.ItemDescription);
		_animationPlayer.Play("effect_display");
	}
}
