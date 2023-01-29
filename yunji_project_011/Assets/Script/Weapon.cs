using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }; //�ٰŸ�, ���Ÿ�
    //���� Ÿ��, ������, ����, ����, ȿ�� ���� ����
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea; //�������ݹ���
    public TrailRenderer trailEffect; //���� ������

    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
            
        }
    }

    IEnumerator Swing() //������ �Լ� Ŭ���� (�ڷ�ƾ)
    {
        //1
        yield return new WaitForSeconds(0.1f); //����� �����ϴ� Ű���� (1������ ���)
        meleeArea.enabled= true;
        trailEffect.enabled = true;

        //2
        yield return new WaitForSeconds(0.3f); //1������ ���
        meleeArea.enabled = false;

        //3
        yield return new WaitForSeconds(0.3f); //1������ ���
        trailEffect.enabled = false;
    }
}
