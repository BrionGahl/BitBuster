namespace BitBuster.hud;

public partial class CardCounter: SimpleCounter
{
	protected override void OnStatChange()
	{
		Counter.Text = PlayerStats.KeyCardCount.ToString();
	}
}
