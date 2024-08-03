namespace BitBuster.hud;

public partial class BombCounter : SimpleCounter
{
	protected override void OnStatChange()
	{
		Counter.Text = PlayerStats.BombCount.ToString();
	}
}
