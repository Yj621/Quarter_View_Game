using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public  GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;
    
    void Start()
    {
        StartCoroutine(Explosion());
    }
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity= Vector3.zero;
        rigid.angularVelocity= Vector3.zero;
        //속도와 회전속도(물리적 속도)를 모두 초기화
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        //시작위치, 쏘는 방향, 길이, LayerMask(특정 레이어만 카메라에 노출되도록))

        foreach(RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
            //foreach문으로 수류탄 범위 적들의 피격함수 호출 HitByGrenade()함수 새로 만들기
        }
    }
}
