using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class StageTransparenter : MonoBehaviour
{
    [SerializeField] private float transparentValue = 0.5f;

    void Start()
    {
        var alpha = new Color(0, 0, 0, transparentValue);

        this.OnTriggerEnterAsObservable()
            .Where(col => col.gameObject.layer == LayerMap.Stage)
            .Subscribe(col =>
            {
                var renderer = col.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.materials.ToList().ForEach(v => v.color -= alpha);
            });

        this.OnTriggerExitAsObservable()
            .Where(col => col.gameObject.layer == LayerMap.Stage)
            .Subscribe(col =>
            {
                var renderer = col.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.materials.ToList().ForEach(v => v.color += alpha);
            });
    }

}
