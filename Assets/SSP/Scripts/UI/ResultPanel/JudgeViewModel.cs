using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JudgeViewModel : MonoBehaviour {

    [SerializeField] private Text judgeText;

    public void Init(bool isWin)
    {
        judgeText.text = isWin ? "Victory" : "Defeat";
    }
}
