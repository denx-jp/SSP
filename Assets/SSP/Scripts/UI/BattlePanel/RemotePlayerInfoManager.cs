using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class RemotePlayerInfoManager : MonoBehaviour
{
    [SerializeField] private NameViewModel nameVM;
    [SerializeField] private HealthViewModel healthVM;

    void Start()
    {
        var model = GetComponentInParent<PlayerModel>();

        if (model.isLocalPlayer)
        {
            gameObject.SetActive(false);
            return;
        }

        var cameraTransform = Camera.main.gameObject.transform;
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                this.transform.rotation = cameraTransform.rotation;
            });

        nameVM.Init(model);
        healthVM.Init(model);
    }
}
