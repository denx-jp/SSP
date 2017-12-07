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
	public const int LocalPlayer = 8;
	public const int Invincible = 9;
	public const int Attack = 10;
	public const int Stage = 12;
	public const int CarryObject = 14;
	public const int EtherObject = 15;
	public const int EtherDetector = 16;
	public const int GuideObject = 17;
	public const int GuideDetector = 18;
	public const int DefaultMask = 1;
	public const int TransparentFXMask = 2;
	public const int IgnoreRaycastMask = 4;
	public const int WaterMask = 16;
	public const int UIMask = 32;
	public const int LocalPlayerMask = 256;
	public const int InvincibleMask = 512;
	public const int AttackMask = 1024;
	public const int StageMask = 4096;
	public const int CarryObjectMask = 16384;
	public const int EtherObjectMask = 32768;
	public const int EtherDetectorMask = 65536;
	public const int GuideObjectMask = 131072;
	public const int GuideDetectorMask = 262144;
}
