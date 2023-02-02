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
    public int maxAmmo; //�ִ� �Ѿ� ����
    public int curAmmo; //���� ����

    public BoxCollider meleeArea; //�������ݹ���
    public TrailRenderer trailEffect; //���� ������
    public Transform bulletPos; //��ġ (�������� ������ ��ġ)
    public GameObject bullet; //�Ѿ� ������Ʈ(�������� ������ ����)
    public Transform bulletCasePos; //��ź ��ġ
    public GameObject bulletCase; //��ź ������Ʈ(������)

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");

        }
        else if (type == Type.Range && curAmmo > 0) //���� ź���� ������ 0���� Ŭ��
        {
            curAmmo--; //�ϳ��� ����
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing() //������ �Լ� Ŭ���� (�ڷ�ƾ)
    {
        //1
        yield return new WaitForSeconds(0.1f); //����� �����ϴ� Ű���� (1������ ���)
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        //2
        yield return new WaitForSeconds(0.3f); //1������ ���
        meleeArea.enabled = false;

        //3
        yield return new WaitForSeconds(0.3f); //1������ ���
        trailEffect.enabled = false;
    }
    IEnumerator Shot() //������ �Լ� Ŭ���� (�ڷ�ƾ)
    {
        //1 �Ѿ� �߻�
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation); //Instantiate�Լ��� �Ѿ� �ν���Ʈȭ �ϱ�
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>(); //Rigidbody�� �߰����༭ �ӵ� �߰�
        bulletRigid.velocity = bulletPos.forward * 50; //z�� : forward

        yield return null;

        //2 ź�� ����
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation); //Instantiate�Լ��� �Ѿ� �ν���Ʈȭ �ϱ�
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>(); //Rigidbody�� �߰����༭ �ӵ� �߰�
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3); //Back�� ���� ������ ���̳ʽ����� �������� ������
        caseRigid.AddForce(caseVec, ForceMode.Impulse); //���� �����ִ� �Լ� ��������� 
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); //ź�� ȸ��
    }
}
