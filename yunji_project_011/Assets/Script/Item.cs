using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon }; //enum ������Ÿ��(Ÿ�� �̸� ���� �ʿ�)
    public Type type;
    public int value;

    void Update()
    {
        transform.Rotate(Vector3.up * 30 * Time.deltaTime); //��� ȸ���ϵ��� ȿ�� �ֱ�
    }
}
