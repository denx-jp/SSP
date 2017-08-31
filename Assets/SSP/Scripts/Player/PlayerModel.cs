using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerModel : MonoBehaviour
{

    public ReactiveProperty<float> Health = new ReactiveProperty<float>();
    public ReactiveProperty<float> Ether = new ReactiveProperty<float>();

}
