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
    public BoxCollider meleeArea; //근접공격범위
    public TrailRenderer trailEffect; //흔적 렌더링

    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
            
        }
    }

    IEnumerator Swing() //열거형 함수 클래스 (코루틴)
    {
        //1
        yield return new WaitForSeconds(0.1f); //결과를 전달하는 키워드 (1프레임 대기)
        meleeArea.enabled= true;
        trailEffect.enabled = true;

        //2
        yield return new WaitForSeconds(0.3f); //1프레임 대기
        meleeArea.enabled = false;

        //3
        yield return new WaitForSeconds(0.3f); //1프레임 대기
        trailEffect.enabled = false;
    }
}
