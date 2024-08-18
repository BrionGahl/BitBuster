using BitBuster.component;
using BitBuster.entity.enemy;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.hud;

public partial class BossHealthBar : ProgressBar
{
	private GlobalEvents _globalEvents;

	private HealthComponent _bossHealthComponent;
	
	private Label _bossName;
	private ProgressBar _deltaBar;
	private Timer _timer;
	private TextureRect _overlay;
	
	public override void _Ready()
	{
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		
		Visible = false;

		_bossName = GetNode<Label>("BossName");
		_deltaBar = GetNode<ProgressBar>("DeltaBar");
		_timer = GetNode<Timer>("Timer");
		_overlay = GetNode<TextureRect>("Overlay");
		
		_timer.Timeout += OnDeltaTimeout;
		
		_globalEvents.BossRoomEnter += OnBossRoomEnter;
		_globalEvents.BossKilled += OnBossKilled;
	}

	private void OnHealthChange(float value)
	{
		if (_bossHealthComponent == null)
			return;

		MaxValue = _bossHealthComponent.MaxHealth;
		Value = _bossHealthComponent.CurrentHealth;
		_timer.Start(0.333);
	}
	
	private void OnDeltaTimeout()
	{
		_deltaBar.MaxValue = _bossHealthComponent.MaxHealth;
		_deltaBar.Value = Value;
	}
	
	private void OnBossRoomEnter(Enemy enemy)
	{
		Logger.Log.Information("Boss Room Entered...");

		_bossName.Text = enemy.EntityStats.Name;
		_bossHealthComponent = enemy.HealthComponent;
		
		MaxValue = _bossHealthComponent.MaxHealth;
		Value = _bossHealthComponent.CurrentHealth;
		MinValue = 0;
		
		_deltaBar.MaxValue = _bossHealthComponent.MaxHealth;
		_deltaBar.Value = _bossHealthComponent.CurrentHealth;
		_deltaBar.MinValue = 0;
		
		Visible = true;
		
		_bossHealthComponent.HealthChange += OnHealthChange;
		_timer.Start(0.125);
	}

	private void OnBossKilled()
	{
		_timer.Stop();
		
		_bossHealthComponent = null;
		Visible = false;
	}
	
	public override void _ExitTree()
	{
		_globalEvents.BossRoomEnter -= OnBossRoomEnter;
		_globalEvents.BossKilled -= OnBossKilled;	
	}
}
