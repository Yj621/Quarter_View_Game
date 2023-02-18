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

    void Awake() //�ʱ�ȭ �Լ�
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
            //Weapon.cs�� �������� �ڵ�
            curHealth -= weapon.damage;

            Vector3 reactVec = transform.position - other.transform.position;
            //���� ��ġ�� �ǰ� ��ġ�� ���� ���ۿ� ���� ���ϱ�

            StartCoroutine(OnDamage(reactVec, false));
            //�ڷ�ƾ ����
        }

        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            //Bullet.cs�� �������� �ڵ�
            curHealth -= bullet.damage;

            Vector3 reactVec = transform.position - other.transform.position;
            //���� ��ġ�� �ǰ� ��ġ�� ���� ���ۿ� ���� ���ϱ�

            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec, false));
            //�ڷ�ƾ ����
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;

        Vector3 reactVec = transform.position - explosionPos;
        //���� ��ġ�� �ǰ� ��ġ�� ���� ���ۿ� ���� ���ϱ�

        StartCoroutine(OnDamage(reactVec, true));
        //�ڷ�ƾ ����
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade) //�ڷ�ƾ ����
    {
        mat.color = Color.red;
  
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)//���� ü���� 0���� ũ�� 
        {
            mat.color = Color.white;
        }

        else //�׾�����
        {
            mat.color = Color.gray;
            gameObject.layer = 14;

            if(isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;
                //up�� 3���� �� ��
                rigid.freezeRotation= false;
                //Enemy�� freezeRotation�� �������־ üũ �������ֱ�
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
                //AddTorque�� ȸ��
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                //AddForce()�Լ��� �˹� �����ϱ�
            }
            Destroy(gameObject, 4); //4�ʵڿ� ����
        }
    }
}
