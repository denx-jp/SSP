using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable　//IInteractableを継承
{
    bool isOpen; //falseが閉まっている状態、trueが開いている状態
    GameObject dooredge1;
    GameObject dooredge2;
    Vector3 EdgePos1;
    Vector3 EdgePos2;
    //Interactメソッドが呼ばれるとisOpen切り替え、trueかfalseでOpenかCloseのメソッド呼び出し
    public void Interact(PlayerManager playerManager)
    {
        isOpen = !isOpen;
       
    }
    void Start()
    {
        isOpen = false; //最初は閉まっている
        dooredge1 = GameObject.Find("DoorEdge1");
        dooredge2 = GameObject.Find("DoorEdge2");
        EdgePos1 = dooredge1.transform.position;//扉が閉まった後静止する地点
        EdgePos2 = dooredge2.transform.position;//扉が開いた後静止する地点
        
    }

    void Update()
    {
        //扉の移動範囲をEdgePos1のx座標からEdgePos2のx座標まで制限
        transform.position = (new Vector3(Mathf.Clamp(transform.position.x, EdgePos1.x, EdgePos2.x), transform.position.y, transform.position.z));
        if (isOpen)
        {
            Open();
        }
        else if (!isOpen)
        {
            Close();
        }
    }

    private void Close()
    {
        for (int i = 0; i<10; i++)
        {
           
            transform.position += new Vector3(0.05f, 0.0f, 0.0f);
        }

    }

    private void Open()
    {
       for(int i=0; i<10; i++)
        {
            transform.position += new Vector3(-0.05f, 0.0f, 0.0f);
        }
    }
}
