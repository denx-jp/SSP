using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class WeaponPopper : NetworkBehaviour
{
    [SerializeField] private int weaponCount = 36;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0, 1, 0);
    [SerializeField] private List<GameObject> popWeapons;
    private Stack<Transform> popPoints = new Stack<Transform>();

    public void Init()
    {
        if (isServer)
        {
            GameObject.FindGameObjectsWithTag(TagMap.PopPoint).Select(v => v.transform)
                .OrderBy(i => System.Guid.NewGuid()).ToList().ForEach(v => popPoints.Push(v));
            var spawnCount = weaponCount < popPoints.Count ? weaponCount : popPoints.Count;
            for (int i = 0; i < spawnCount; i++)
            {
                var popPoint = popPoints.Pop();
                var popWeapon = popWeapons[Random.Range(0, popWeapons.Count - 1)];
                var weapon = Instantiate(popWeapon, popPoint.position + spawnOffset, popPoint.rotation);
                NetworkServer.Spawn(weapon);
            }
        }
    }
}
