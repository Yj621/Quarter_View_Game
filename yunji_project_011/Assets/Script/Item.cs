using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon }; //enum 열거형타입(타입 이름 지정 필요)
    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;

    void Awake()
    {
        rigid= GetComponent<Rigidbody>();
        sphereCollider= GetComponent<SphereCollider>();
        //GetComponent함수는 첫번째 컴포넌트를 가져옴

    }

    void Update()
    {
        transform.Rotate(Vector3.up * 30 * Time.deltaTime); //계속 회전하도록 효과 주기
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic= true;
            sphereCollider.enabled= false;
        }
    }
}
