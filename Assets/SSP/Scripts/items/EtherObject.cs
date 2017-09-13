using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class EtherObject : MonoBehaviour
{
	public GameObject Ether;
	float delta; //時間経過
    public float etherValue;

	[SerializeField] private List<GameObject> Area = new List<GameObject>();	//ポップ地点の管理
    [SerializeField] private float originEtherSize;
    [SerializeField] private float triggerSize = 0.5f;
    [SerializeField] private float floatHeight;
    [SerializeField] private float trackingSpeed;
	[SerializeField] private float TimeSpan; // pop間の時間設定
    private SphereCollider trigger;

    private RaycastHit fallHit;
    private int fllLayerMask = 1 << LayerMap.Stage;

    private RaycastHit absorbHit;
    private int absorbLayerMask = ~(1 << LayerMap.EtherObject);
    //[SyncVar]
    private GameObject target;

    private void Start()
    {
        var rigid = GetComponent<Rigidbody>();

        //地面よりある程度高い位置で重力をきる処理
        this.UpdateAsObservable()
            .Where(_ => Physics.Raycast(transform.position, Vector3.down, out fallHit, floatHeight * 10, fllLayerMask))
            .Where(_ => Vector3.Distance(transform.position, fallHit.point) < floatHeight)
            .Take(1)
            .Subscribe(_ => rigid.useGravity = false);

        #region エーテル吸収処理
        //trigger圏内のPlayerのRayをとばして、障害物がなければtargetに指定
        this.OnTriggerEnterAsObservable()
            .Where(_ => target == null)
            .Where(col => col.gameObject.tag == TagMap.Player)
            .Where(col => col.GetComponent<PlayerHealthManager>().IsAlive())
            .Where(col => Physics.Raycast(transform.position, col.transform.position - transform.position, out absorbHit, 100, absorbLayerMask))
            .Where(_ => absorbHit.collider.gameObject.tag == TagMap.Player)
            .Subscribe(col => target = col.gameObject);

        //targetを追従
        this.UpdateAsObservable()
            .Where(_ => target != null)
            .Subscribe(_ => rigid.AddForce((target.transform.position - transform.position) * trackingSpeed, ForceMode.Force));

        //targetに衝突時に消滅・吸収
        this.OnCollisionEnterAsObservable()
            .Where(col => col.gameObject == target)
            .Subscribe(_ =>
            {
                target.GetComponent<IEtherAcquirer>().AcquireEther(etherValue);
                Destroy(this.gameObject);
            });
        #endregion
    }

    public void Init(float value)
    {
        etherValue = value;
        transform.localScale = Vector3.one * originEtherSize * value;
        if (transform.localScale.x < 1)
        {
            var trigger = GetComponents<SphereCollider>().Where(v => v.isTrigger).First();
            trigger.radius = triggerSize / transform.localScale.x;
        }
    }
	public void Ether_pop(){
		this.delta += Time.deltaTime;
		if(this.delta > this.TimeSpan){
			this.delta = 0;
			GameObject ether = Instantiate(Ether) as GameObject;
			float px = Random.Range (-1, 1);
			float py = Random.Range (-1, 1);
			float pz = Random.Range (-1, 1);
			ether.transform.position = new Vector3 (px, py, pz);
		}
}
}
