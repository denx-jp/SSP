using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class EffectBullet : MonoBehaviour
{
    [HideInInspector] public int shootPlayerId, shootPlayerTeamId;
    [HideInInspector] public float deathTime;
    [SerializeField] Rigidbody rigid;

    private void Start()
    {
        Destroy(this.gameObject, deathTime);

        this.OnTriggerEnterAsObservable()
            .Select(col => col.GetComponent<PlayerModel>())
            .Where(v => v == null || v.teamId != shootPlayerTeamId)
            .Subscribe(_ =>
            {
                rigid.isKinematic = true;
                rigid.velocity = Vector3.zero;
            });
    }

    public void SetIds(int playerId, int teamId, float dtime)
    {
        shootPlayerId = playerId;
        shootPlayerTeamId = teamId;
        deathTime = dtime;
    }
}
