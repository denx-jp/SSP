using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum WeaponType { HandGun, LongRangeWeapon, ShortRangeWeapon, Gimmick }
public class WeaponModel : NetworkBehaviour
{
    public new string name;
    public WeaponType type;
    public float damageAmount;
    public Sprite image;

    public PlayerModel ownerPlayerModel;

    public Damage GetDamage()
    {
        return new Damage(damageAmount, ownerPlayerModel.Id, ownerPlayerModel.teamId);
    }
}
