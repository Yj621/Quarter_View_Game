using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons; //플레이어의 무기 관련 배열 함수 2개 선언
    public bool[] hasWeapons; // "

    public GameObject[] grenades; //공전하는 물체를 생성하기 위해 배열 변수 선언
    public int hasGrenades;//수류탄
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

    bool wDown; //걷기키
    bool jDown; //점프키
    bool iDown; //상호작용키
    bool fDown; //공격키
    bool rDown; //재장전

    bool sDown1;//장비 단축키(1번)
    bool sDown2;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true; //공격준비
    bool isBorder;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;
    GameObject nearObject; //근처에 있는 오브젝트 인식
    Weapon equipWeapon; //장착중인 무기 저장 변수
    int equipWeaponIndex = -1; //기본값을 -1로 해줌으로써 0번째 index랑 겹치지 않게 함
    float fireDelay; //공격딜레이

    void Start()
    {
        rigid = GetComponent<Rigidbody>(); //물리효과를 위해 Rigidbody변수 선언 후 초기화
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Reload(); //재장전
        Dodge();
        Interaction();
        Swap();
    }

    void GetInput() //Button을 Input함수로 묶어줌
    {
        hAxis = Input.GetAxisRaw("Horizontal"); //Axis값을 정수로 반환하는 함수
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        fDown = Input.GetButton("Fire1"); //공격키(마우스 왼쪽)
        rDown = Input.GetButtonDown("Reload"); //재장전 (R키)
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction"); //상호작용
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2"); //새로운 Input을 위해 Input Manager에서 size늘려서 사용자 지정
    }

    void Move() //이동 함수
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //normalized:방향 값이 1로 보정된 벡터
        if (isDodge)
        {
            moveVec = dodgeVec; //회피 중에는 움직임 벡터 => 회피방향 벡터로 바뀌도록 구현
        }

        if (isSwap || isReload || !isFireReady)
        {
            moveVec = Vector3.zero; //무기 교체를 하고 있을 때는 움직임벡터 0
        }

        if (!isBorder)
        {
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime; //transform이동은 Time.deltaTime을 곱해줘야함 

        }


        anim.SetBool("IsRun", moveVec != Vector3.zero); //SetBool함수로 파라미터 값 설정, 0,0,0만 아니면 된다
        anim.SetBool("IsWalk", wDown);

    }

    void Turn() //회전 함수
    {
        //1 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec); // 지정된 벡터를 향해 회전시켜주는 함수

        //2 마우스에 의한 회전
        if (fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit; //RaycastHit 정보를 저장할 변수 추가

            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position; //RaycastHit의 마우스 클릭 위치를 활용해 회전 구현
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }

    }

    void Jump() //점프 함수
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)  // 액션 도중 다른 액션이 실행되지 않도록
        //점프키를 누르고, 움직이지 않으며 점프 중이 아닐때
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); //AddForce()함수로 물리적인 힘을 가하기
            anim.SetBool("IsJump", true);
            anim.SetTrigger("doJump"); //애니메이션
            isJump = true;
        }
    }

    void Attack()
    {
        if(equipWeapon == null) //장착중인 무기가 없으면 실행X
        {
            return;
        }
        fireDelay += Time.deltaTime; //공격 딜레이에 시간 더함
        isFireReady= equipWeapon.rate < fireDelay; //공격 가능 여부 확인 (공격속도<딜레이)

        if(fDown && isFireReady && !isDodge &&!isSwap) //버튼을 눌렀을때
        {
            equipWeapon.Use(); //무기에 있는 함수 실행

            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            //장착된 무기의 종류가 근거리면 doSwing, 원거리면 doShot
            fireDelay = 0; //공격딜레이를 0으로 초기화해주고 다음 공격까지 기다리도록해줌
        }

    }

    void Reload() //재장전 함수
    {
        if(equipWeapon == null)
        {
            return; //장착중인 무기가 없으면 안됨
        }
        if(equipWeapon.type == Weapon.Type.Melee)
        {
            return; //장착중인 무기가 근거리면 안됨
        }
        if (ammo == 0)
        {
            return; //총알이 0개면 안됨
        }

        if(rDown && !isJump && !isDodge && !isSwap && isFireReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 3f);
        }

    }

    void ReloadOut() //장전이 완료되면
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        //플레이어가 소지한 총알 개수가 최대 개수보다 작으면 ammo로, 아니면 maxAmmo로
        equipWeapon.curAmmo = reAmmo; //현재 총알 개수를 장전한 개수로 바꿈
        ammo -= reAmmo; //가지고 있는 총알이 사라짐
        isReload= false;
    }

    void Dodge() //회피 함수
    {
        if (jDown & moveVec != Vector3.zero & !isJump && !isDodge &&!isSwap) //점프키를 누르고, 움직이며 점프 중이 아닐때
        {
            dodgeVec = moveVec; // 회피도중 방향 전환이 되지 않도록 회피 방향 추가

            speed *= 2;  //현재 속도의 2배
            anim.SetTrigger("doDodge"); //애니메이션
            isDodge = true;

            Invoke("DodgeOut", 0.4f); // 시간차 함수 호출
        }
    }

    void DodgeOut() //회피가 끝났을때
    {
        speed *= 0.5f; //속도 원래대로
        isDodge = false;
    }

    void Swap() //무기 교체
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) //1번키를 눌렀고, 1번무기가 없을때or같은 무기일때 
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) //2번키를 눌렀고, 2번무기가 없을때or같은 무기일때 
            return;

        int weaponIndex = -1; //기본값
        if (sDown1) weaponIndex = 0; 
        if (sDown2) weaponIndex = 1;
        if ((sDown1 || sDown2) && !isJump && !isDodge) //1번키나 2번키를 눌렀을때, 점프와 닷지를 하지 않고 있을 때
        {
            if(equipWeapon != null) {
                equipWeapon.gameObject.SetActive(false);
            }

            equipWeaponIndex = weaponIndex;

            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true); //무기 보이게하기

            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.4f); // 시간차 함수 호출
        }
    }
    void SwapOut() //회피가 끝났을때
    {
        isSwap = false;
    }

    void Interaction() //E키 사용
    {
        if (iDown && nearObject != null && !isJump && !isDodge) //상호작용 함수 (물체가 있을때+점프나 닷지를 하고 있지 않을때)
        {
            if (nearObject.tag == "Weapon") //근처에 있는 오브젝트의 태그가 무기일때 
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value; //value값을 지정해줘서 무기가 2개니까 하나는 1, 하나는 2로 지
                hasWeapons[weaponIndex] = true; //아이템 정보를 가져와서 해당 무기를 가지고 있는지 체크해줌

                Destroy(nearObject);
            }

        }
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        //시작위치, 쏘는 방향 / 길이, 색
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
        //시작위치, 쏘는 방향, 길이, LayerMask(특정 레이어만 카메라에 노출되도록)
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

    void OnTriggerEnter(Collider other) //item의 trigger체크
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch(item.type) //enum타입 변수+switch문으로 간단하게 조건문
            {
                case Item.Type.Ammo:
                    ammo += item.value; //value값 추가
                    if (ammo > maxAmmo) //최대값을 넘기지 않도록
                        ammo = maxAmmo; //최대값 변경
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
