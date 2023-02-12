using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons; //�÷��̾��� ���� ���� �迭 �Լ� 2�� ����
    public bool[] hasWeapons; // "

    public GameObject[] grenades; //�����ϴ� ��ü�� �����ϱ� ���� �迭 ���� ����
    public int hasGrenades;//����ź
    public Camera followCamera;

    public int ammo;
    public int coin;
    public int health;


    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAxis;
    float vAxis;

    bool wDown; //�ȱ�Ű
    bool jDown; //����Ű
    bool iDown; //��ȣ�ۿ�Ű
    bool fDown; //����Ű
    bool rDown; //������

    bool sDown1;//��� ����Ű(1��)
    bool sDown2;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true; //�����غ�
    bool isBorder;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;
    GameObject nearObject; //��ó�� �ִ� ������Ʈ �ν�
    Weapon equipWeapon; //�������� ���� ���� ����
    int equipWeaponIndex = -1; //�⺻���� -1�� �������ν� 0��° index�� ��ġ�� �ʰ� ��
    float fireDelay; //���ݵ�����

    void Start()
    {
        rigid = GetComponent<Rigidbody>(); //����ȿ���� ���� Rigidbody���� ���� �� �ʱ�ȭ
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Reload(); //������
        Dodge();
        Interaction();
        Swap();
    }

    void GetInput() //Button�� Input�Լ��� ������
    {
        hAxis = Input.GetAxisRaw("Horizontal"); //Axis���� ������ ��ȯ�ϴ� �Լ�
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        fDown = Input.GetButton("Fire1"); //����Ű(���콺 ����)
        rDown = Input.GetButtonDown("Reload"); //������ (RŰ)
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction"); //��ȣ�ۿ�
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2"); //���ο� Input�� ���� Input Manager���� size�÷��� ����� ����
    }

    void Move() //�̵� �Լ�
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //normalized:���� ���� 1�� ������ ����
        if (isDodge)
        {
            moveVec = dodgeVec; //ȸ�� �߿��� ������ ���� => ȸ�ǹ��� ���ͷ� �ٲ�� ����
        }

        if (isSwap || isReload || !isFireReady)
        {
            moveVec = Vector3.zero; //���� ��ü�� �ϰ� ���� ���� �����Ӻ��� 0
        }

        if (!isBorder)
        {
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime; //transform�̵��� Time.deltaTime�� ��������� 

        }


        anim.SetBool("IsRun", moveVec != Vector3.zero); //SetBool�Լ��� �Ķ���� �� ����, 0,0,0�� �ƴϸ� �ȴ�
        anim.SetBool("IsWalk", wDown);

    }

    void Turn() //ȸ�� �Լ�
    {
        //1 Ű���忡 ���� ȸ��
        transform.LookAt(transform.position + moveVec); // ������ ���͸� ���� ȸ�������ִ� �Լ�

        //2 ���콺�� ���� ȸ��
        if (fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit; //RaycastHit ������ ������ ���� �߰�

            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position; //RaycastHit�� ���콺 Ŭ�� ��ġ�� Ȱ���� ȸ�� ����
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }

    }

    void Jump() //���� �Լ�
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)  // �׼� ���� �ٸ� �׼��� ������� �ʵ���
        //����Ű�� ������, �������� ������ ���� ���� �ƴҶ�
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); //AddForce()�Լ��� �������� ���� ���ϱ�
            anim.SetBool("IsJump", true);
            anim.SetTrigger("doJump"); //�ִϸ��̼�
            isJump = true;
        }
    }

    void Attack()
    {
        if(equipWeapon == null) //�������� ���Ⱑ ������ ����X
        {
            return;
        }
        fireDelay += Time.deltaTime; //���� �����̿� �ð� ����
        isFireReady= equipWeapon.rate < fireDelay; //���� ���� ���� Ȯ�� (���ݼӵ�<������)

        if(fDown && isFireReady && !isDodge &&!isSwap) //��ư�� ��������
        {
            equipWeapon.Use(); //���⿡ �ִ� �Լ� ����

            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            //������ ������ ������ �ٰŸ��� doSwing, ���Ÿ��� doShot
            fireDelay = 0; //���ݵ����̸� 0���� �ʱ�ȭ���ְ� ���� ���ݱ��� ��ٸ���������
        }

    }

    void Reload() //������ �Լ�
    {
        if(equipWeapon == null)
        {
            return; //�������� ���Ⱑ ������ �ȵ�
        }
        if(equipWeapon.type == Weapon.Type.Melee)
        {
            return; //�������� ���Ⱑ �ٰŸ��� �ȵ�
        }
        if (ammo == 0)
        {
            return; //�Ѿ��� 0���� �ȵ�
        }

        if(rDown && !isJump && !isDodge && !isSwap && isFireReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 3f);
        }

    }

    void ReloadOut() //������ �Ϸ�Ǹ�
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        //�÷��̾ ������ �Ѿ� ������ �ִ� �������� ������ ammo��, �ƴϸ� maxAmmo��
        equipWeapon.curAmmo = reAmmo; //���� �Ѿ� ������ ������ ������ �ٲ�
        ammo -= reAmmo; //������ �ִ� �Ѿ��� �����
        isReload= false;
    }

    void Dodge() //ȸ�� �Լ�
    {
        if (jDown & moveVec != Vector3.zero & !isJump && !isDodge &&!isSwap) //����Ű�� ������, �����̸� ���� ���� �ƴҶ�
        {
            dodgeVec = moveVec; // ȸ�ǵ��� ���� ��ȯ�� ���� �ʵ��� ȸ�� ���� �߰�

            speed *= 2;  //���� �ӵ��� 2��
            anim.SetTrigger("doDodge"); //�ִϸ��̼�
            isDodge = true;

            Invoke("DodgeOut", 0.4f); // �ð��� �Լ� ȣ��
        }
    }

    void DodgeOut() //ȸ�ǰ� ��������
    {
        speed *= 0.5f; //�ӵ� �������
        isDodge = false;
    }

    void Swap() //���� ��ü
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) //1��Ű�� ������, 1�����Ⱑ ������or���� �����϶� 
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) //2��Ű�� ������, 2�����Ⱑ ������or���� �����϶� 
            return;

        int weaponIndex = -1; //�⺻��
        if (sDown1) weaponIndex = 0; 
        if (sDown2) weaponIndex = 1;
        if ((sDown1 || sDown2) && !isJump && !isDodge) //1��Ű�� 2��Ű�� ��������, ������ ������ ���� �ʰ� ���� ��
        {
            if(equipWeapon != null) {
                equipWeapon.gameObject.SetActive(false);
            }

            equipWeaponIndex = weaponIndex;

            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true); //���� ���̰��ϱ�

            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.4f); // �ð��� �Լ� ȣ��
        }
    }
    void SwapOut() //ȸ�ǰ� ��������
    {
        isSwap = false;
    }

    void Interaction() //EŰ ���
    {
        if (iDown && nearObject != null && !isJump && !isDodge) //��ȣ�ۿ� �Լ� (��ü�� ������+������ ������ �ϰ� ���� ������)
        {
            if (nearObject.tag == "Weapon") //��ó�� �ִ� ������Ʈ�� �±װ� �����϶� 
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value; //value���� �������༭ ���Ⱑ 2���ϱ� �ϳ��� 1, �ϳ��� 2�� ��
                hasWeapons[weaponIndex] = true; //������ ������ �����ͼ� �ش� ���⸦ ������ �ִ��� üũ����

                Destroy(nearObject);
            }

        }
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        //������ġ, ��� ���� / ����, ��
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
        //������ġ, ��� ����, ����, LayerMask(Ư�� ���̾ ī�޶� ����ǵ���)
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }


    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("IsJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other) //item�� triggerüũ
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch(item.type) //enumŸ�� ����+switch������ �����ϰ� ���ǹ�
            {
                case Item.Type.Ammo:
                    ammo += item.value; //value�� �߰�
                    if (ammo > maxAmmo) //�ִ밪�� �ѱ��� �ʵ���
                        ammo = maxAmmo; //�ִ밪 ����
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;

                case Item.Type.Heart:
                    health+= item.value;
                    if(health > maxHealth)
                        health= maxHealth;
                    break;

                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true); 
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
    }


    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon")
        {
            nearObject = other.gameObject;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null; ;
        }
    }
}
