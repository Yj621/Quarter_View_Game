using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon }; //enum ������Ÿ��(Ÿ�� �̸� ���� �ʿ�)
    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;

    void Awake()
    {
        rigid= GetComponent<Rigidbody>();
        sphereCollider= GetComponent<SphereCollider>();
        //GetComponent�Լ��� ù��° ������Ʈ�� ������

    }

    void Update()
    {
        transform.Rotate(Vector3.up * 30 * Time.deltaTime); //��� ȸ���ϵ��� ȿ�� �ֱ�
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
