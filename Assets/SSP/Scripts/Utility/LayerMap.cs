/// <summary>
/// レイヤー名を定数で管理するクラス
/// </summary>
public static class LayerMap
{
	public const int Default = 0;
	public const int TransparentFX = 1;
	public const int IgnoreRaycast = 2;
	public const int Water = 4;
	public const int UI = 5;
	public const int Attack = 8;
	public const int Invincible = 9;
	public const int DefaultMask = 1;
	public const int TransparentFXMask = 2;
	public const int IgnoreRaycastMask = 4;
	public const int WaterMask = 16;
	public const int UIMask = 32;
	public const int AttackMask = 256;
	public const int InvincibleMask = 512;
}
