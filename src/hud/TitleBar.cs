using BitBuster.component;
using BitBuster.entity;
using BitBuster.entity.player;
using BitBuster.items;
using BitBuster.resource;
using Godot;

namespace BitBuster.hud;

public partial class TitleBar : VSplitContainer
{
	private AnimationPlayer _animationPlayer;

	private Player _player;
	private EntityStats _playerStats;

	private Label _title;
	private Label _subtitle;
	
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_playerStats = ((Player)GetTree().GetFirstNodeInGroup("player")).EntityStats;
		_player = (Player)GetTree().GetFirstNodeInGroup("player");

		_title = GetNode<Label>("Title");
		_subtitle = GetNode<Label>("Subtitle");
		
		_playerStats.ItemAdded += OnItemAdded;
		_player.Died += OnPlayerDied;
	}

	private void SetLabels(string title, string subtitle)
	{
		_title.Text = title;
		_subtitle.Text = subtitle;
		_animationPlayer.Play("effect_display");
	}
	
	private void OnItemAdded(Item item)
	{
		SetLabels(item.ItemName, item.ItemDescription);
	}

	private void OnPlayerDied()
	{
		SetLabels("Busted...", "Remember to submit tickets!");
	}
}
