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

    void Awake() //초기화 함수
    {
        rigid= GetComponent<Rigidbody>();
        boxCollider= GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            //Weapon.cs를 가져오는 코드
            curHealth -= weapon.damage;

            Debug.Log("Melee : "+curHealth);
        }

        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            //Bullet.cs를 가져오는 코드
            curHealth -= bullet.damage;

            Debug.Log("Range : " + curHealth);
        }
    }
}
