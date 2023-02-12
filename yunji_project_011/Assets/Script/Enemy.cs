using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;

    void Awake() //�ʱ�ȭ �Լ�
    {
        rigid= GetComponent<Rigidbody>();
        boxCollider= GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            //Weapon.cs�� �������� �ڵ�
            curHealth -= weapon.damage;

            Debug.Log("Melee : "+curHealth);
        }

        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            //Bullet.cs�� �������� �ڵ�
            curHealth -= bullet.damage;

            Debug.Log("Range : " + curHealth);
        }
    }
}
