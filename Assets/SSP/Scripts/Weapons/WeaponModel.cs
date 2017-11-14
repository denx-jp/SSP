using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum WeaponType { HandGun, LongRangeWeapon, ShortRangeWeapon, Gimmick }
public class WeaponModel : NetworkBehaviour
{
    public new string name;
    public WeaponType type;
    public float damageAmount;
    [HideInInspector] public bool isOwnerLocalPlayer;
    public Sprite image;
    [HideInInspector, SyncVar] public int playerId, teamId;

    public Damage GetDamage()
    {
        return new Damage(damageAmount, playerId, teamId);
    }
}
