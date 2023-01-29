using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target; //���� ��ǥ
    public float orbitSpeed; //���� �ӵ�
    Vector3 offset;//��ǥ���� �Ÿ�

    void Start()
    {
        offset= transform.position - target.position;
    }

    void Update()
    {
        transform.position = target.position + offset;
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
        //Ÿ�� ������ ȸ���ϴ� �Լ�
        offset = transform.position - target.position; //RotateAround���� ��ġ�� ������ ��ǥ���� �Ÿ� ����
    }
}
