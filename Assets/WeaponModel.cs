using UnityEngine;
using UnityEngine.Networking;

public class WeaponModel : NetworkBehaviour
{
    public new string name;
    public float damageAmount;
    public bool isOwnerLocalPlayer;
    [HideInInspector, SyncVar] public int playerId, teamId;

    public Damage GetDamage()
    {
        return new Damage(damageAmount, playerId, teamId);
    }
}
