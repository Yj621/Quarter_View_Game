using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }; //근거리, 원거리
    //무기 타입, 데미지, 공속, 범위, 효과 변수 생성
    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo; //최대 총알 개수
    public int curAmmo; //현재 개수

    public BoxCollider meleeArea; //근접공격범위
    public TrailRenderer trailEffect; //흔적 렌더링
    public Transform bulletPos; //위치 (프리팹을 형성할 위치)
    public GameObject bullet; //총알 오브젝트(프리팹을 저장할 변수)
    public Transform bulletCasePos; //총탄 위치
    public GameObject bulletCase; //총탄 오브젝트(프리팹)

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");

        }
        else if (type == Type.Range && curAmmo > 0) //현재 탄약의 개수가 0보다 클때
        {
            curAmmo--; //하나씩 줄임
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing() //열거형 함수 클래스 (코루틴)
    {
        //1
        yield return new WaitForSeconds(0.1f); //결과를 전달하는 키워드 (1프레임 대기)
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        //2
        yield return new WaitForSeconds(0.3f); //1프레임 대기
        meleeArea.enabled = false;

        //3
        yield return new WaitForSeconds(0.3f); //1프레임 대기
        trailEffect.enabled = false;
    }
    IEnumerator Shot() //열거형 함수 클래스 (코루틴)
    {
        //1 총알 발사
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation); //Instantiate함수로 총알 인스턴트화 하기
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>(); //Rigidbody를 추가해줘서 속도 추가
        bulletRigid.velocity = bulletPos.forward * 50; //z축 : forward

        yield return null;

        //2 탄피 배출
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation); //Instantiate함수로 총알 인스턴트화 하기
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>(); //Rigidbody를 추가해줘서 속도 추가
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3); //Back은 없기 때문에 마이너스값을 랜덤으로 곱해줌
        caseRigid.AddForce(caseVec, ForceMode.Impulse); //힘을 가해주는 함수 즉발적으로 
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); //탄피 회전
    }
}
