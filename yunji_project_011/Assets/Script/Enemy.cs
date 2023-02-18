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
    Material mat;

    void Awake() //초기화 함수
    {
        rigid= GetComponent<Rigidbody>();
        boxCollider= GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            //Weapon.cs를 가져오는 코드
            curHealth -= weapon.damage;

            Vector3 reactVec = transform.position - other.transform.position;
            //현재 위치에 피격 위치를 빼서 반작용 방향 구하기

            StartCoroutine(OnDamage(reactVec, false));
            //코루틴 시작
        }

        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            //Bullet.cs를 가져오는 코드
            curHealth -= bullet.damage;

            Vector3 reactVec = transform.position - other.transform.position;
            //현재 위치에 피격 위치를 빼서 반작용 방향 구하기

            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec, false));
            //코루틴 시작
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;

        Vector3 reactVec = transform.position - explosionPos;
        //현재 위치에 피격 위치를 빼서 반작용 방향 구하기

        StartCoroutine(OnDamage(reactVec, true));
        //코루틴 시작
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade) //코루틴 생성
    {
        mat.color = Color.red;
  
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)//현재 체력이 0보다 크면 
        {
            mat.color = Color.white;
        }

        else //죽었을때
        {
            mat.color = Color.gray;
            gameObject.layer = 14;

            if(isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;
                //up을 3정도 더 줌
                rigid.freezeRotation= false;
                //Enemy의 freezeRotation이 고정돼있어서 체크 해제해주기
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
                //AddTorque로 회전
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                //AddForce()함수로 넉백 구현하기
            }
            Destroy(gameObject, 4); //4초뒤에 죽음
        }
    }
}
