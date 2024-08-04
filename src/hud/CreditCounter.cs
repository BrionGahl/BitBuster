namespace BitBuster.hud;

public partial class CreditCounter: SimpleCounter
{
	protected override void OnStatChange()
	{
		Counter.Text = PlayerStats.CreditCount.ToString();
	}
}
