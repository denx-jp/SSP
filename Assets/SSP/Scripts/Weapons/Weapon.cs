using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

[System.Serializable]
public class Weapon: MonoBehaviour
{
    public void StartNormalAttackInputStreams(GameObject go, string actionTriggerButton, string triggerName)
    {
        go.UpdateAsObservable()
           .Where(v => Input.GetButtonDown(actionTriggerButton))
           .Subscribe(_ =>
           {
               go.GetComponent<Animator>().SetTrigger(triggerName);
           }).AddTo(go);
    }
}
