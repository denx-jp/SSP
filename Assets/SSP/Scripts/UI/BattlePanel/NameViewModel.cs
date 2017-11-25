using UnityEngine;
using UnityEngine.UI;

public class NameViewModel : MonoBehaviour
{
    [SerializeField] private Text text;

    public void Init(PlayerModel model)
    {
        text.text = model.Name;
    }
}
