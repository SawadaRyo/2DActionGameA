using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContoller : MonoBehaviour
{
    [SerializeField, Tooltip("���ߍU���̗��߃J�E���^�[")] int m_chrageAttackCounter = 120;
    [SerializeField, Tooltip("�v���C���[�̈ړ����x")] float speed = 5f;
    [SerializeField, Tooltip("�_�b�V���̔{��")] float dashPowor = 10f;
    [SerializeField, Tooltip("�󒆂ł̈ړ����x")] float grindFroth = 5f;
    [SerializeField, Tooltip("�v���C���[�̃W�����v��")] float jump = 3f;
    [SerializeField, Tooltip("�ڒn�����Ray�̎˒�")] float distance = 0.1f;
    [SerializeField, Tooltip("�ǂ̐ڐG�����Ray�̎˒�")] float walldistance = 0.1f;
    [SerializeField, Tooltip("SphierCast�̔��a")] float isGroundRengeRadios = 1f;
    [SerializeField, Tooltip("�ǃW�����̗�")] float wallJumpPower = 10f;
    [SerializeField, Tooltip("�W�����v�̃T�E���h")] AudioClip jumpSound;
    [SerializeField, Tooltip("�v���C���[�̃_���[�W�T�E���h")] AudioClip damageSound;

    [SerializeField, Tooltip("Ray�̎ˏo�_")] Transform footPos;
    [SerializeField, Tooltip("�ڒn�����LayerMask")] LayerMask groundMask = ~0;
    [SerializeField, Tooltip("�ǂ̐ڐG����")] LayerMask wallMask = ~0;

    [SerializeField, Tooltip("�A�j���[�V�������擾����ׂ̕ϐ�")] Animator m_Animator;
    [SerializeField, Tooltip("GamManager���i�[����ϐ�")] GameManager m_gameManager;

    [SerializeField, Tooltip("����prefab���i�[����ϐ�")] GameObject[] m_weaponPrefab = new GameObject[4];
    [SerializeField, Tooltip("���C������")] GameObject m_mainWeapon = default;
    [SerializeField, Tooltip("�T�u����")] GameObject m_subWeapon = default;


    [Tooltip("���ߍU���̗��ߎ���")] int m_chrageAttackCount = 0;
    [Tooltip("Player�̐U��������x")]float m_turnSpeed = 25f;
    [Tooltip("���ړ��̃x�N�g��")]float m_h;
    [Tooltip("����؂�ւ�")] bool m_weaponSwitch = true;
    bool m_attacked = false;
    [Tooltip("Rigidbody�R���|�[�l���g�̎擾")]Rigidbody m_rb;
    //List<ItemBase> m_ItemList = new List<ItemBase>();
    [Tooltip("�I�[�f�B�I���擾����ׂ̕ϐ�")] AudioSource m_Audio;
    [Tooltip("Player�̌����Ă������")] Quaternion orgLocalQuaternion;
    [Tooltip("�������̕���")] GameObject m_equipmentWeapon = default;
    RaycastHit m_hitInfo;
    public Vector3 NormalOfStickingWall { get; private set; } = Vector3.zero;
    public bool Charge { get; private set; } = false;
    public int ChrageAttackCount { get => m_chrageAttackCount; set => m_chrageAttackCount = value; }
    public bool Attacked { get => m_attacked; set => m_attacked = value; }
    delegate void PlayerMethod();
    PlayerMethod m_playerMethod = default;
    

    void Awake()
    {
        orgLocalQuaternion = this.transform.localRotation;
        m_rb = GetComponent<Rigidbody>();
        m_Audio = gameObject.AddComponent<AudioSource>();

        m_weaponSwitch = true;
        //m_mainWeapon = m_weaponPrefab[0];
        //m_subWeapon = m_weaponPrefab[1];
        m_mainWeapon.SetActive(true);
        m_subWeapon.SetActive(false);
        m_equipmentWeapon = m_mainWeapon;
        //m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void OnEnable()
    {
        m_playerMethod += JumpMethod;
        m_playerMethod += WeaponActionMethod;
        m_playerMethod += wallJumpMethod;
        m_playerMethod += MoveMethod;
        Charge = Input.GetButton("Attack");
    }
    void OnDisable()
    {
        m_playerMethod -= JumpMethod;
        m_playerMethod -= WeaponActionMethod;
        m_playerMethod -= wallJumpMethod;
        m_playerMethod -= MoveMethod;
    }
    void Update()
    {
        if (!m_gameManager.IsGame) return;
        else
        {
            m_playerMethod();
            //Debug.Log(m_chrageAttackCount);
            Debug.Log(Attacked);
        }
    }

    //Player�̓����������������ꂽ�֐�
    //----------------------------------------------
    void MoveMethod()
    {
        m_h = Input.GetAxisRaw("Horizontal");
        var moveSpeed = 0f;
        Vector3 velocity = m_rb.velocity;
        if (m_h == -1)
        {
            Quaternion rotationLeft = Quaternion.LookRotation(Vector3.left);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotationLeft, Time.deltaTime * m_turnSpeed);
        }
        else if (m_h == 1)
        {
            Quaternion rotationRight = Quaternion.LookRotation(Vector3.right);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotationRight, Time.deltaTime * m_turnSpeed);
        }

        //�v���C���[�̈ړ�
        if (m_h != 0)
        { 
            moveSpeed = speed;
        }

        if (Input.GetButtonDown("Dash") && IsGrounded())//�_�b�V���R�}���h
        {
            m_rb.AddForce(this.transform.forward * dashPowor, ForceMode.VelocityChange);
            m_Animator.SetTrigger("Dash");
        }
        velocity.x = m_h * moveSpeed;
        m_rb.velocity = new Vector3(velocity.x, m_rb.velocity.y, 0);
        m_Animator.SetFloat("MoveSpeed", Mathf.Abs(velocity.x));
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Damage")
        {
            m_Audio.PlayOneShot(damageSound);
        }

        else if (other.gameObject.tag == "Pendulam" || other.gameObject.tag == "Ffield")
        {
            transform.parent = other.gameObject.transform;
        }

        else if (other.gameObject.tag == "Piston")
        {
            StartCoroutine("WaitKeyInput");
        }
    }

    void OnCollisionExit()
    {
        transform.parent = null;
        //this.transform.localRotation = orgLocalQuaternion;
    }

    bool IsGrounded()
    {
        Vector3 isGroundCenter = footPos.transform.position;
        Ray ray = new Ray(isGroundCenter, Vector3.down);
        bool hitFlg = Physics.SphereCast(ray, isGroundRengeRadios, out _, distance, groundMask);
        return hitFlg;
    }
    bool IsWalled()
    {
        Vector3 isWallCenter = footPos.transform.position;
        Ray rayRight = new Ray(isWallCenter, Vector3.right);
        Ray rayLeft = new Ray(isWallCenter, Vector3.left);
        bool hitFlg = Physics.Raycast(rayRight, out m_hitInfo, walldistance, wallMask)
                   || Physics.Raycast(rayLeft, out m_hitInfo, walldistance, wallMask);
        return hitFlg;
    }
    void JumpMethod()
    {
        //�W�����v�̏���
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            //Debug.Log("a");
            m_Audio.PlayOneShot(jumpSound);
            m_rb.AddForce(0f, jump, 0f, ForceMode.Impulse);
        }
        m_Animator.SetBool("Jump", !IsGrounded());
        
    }
    void wallJumpMethod()
    {
        m_Animator.SetBool("WallGrip",IsWalled());
        if (IsGrounded())
        {
            m_rb.useGravity = true;
            return;
        }
        //ToDo�ړ��R�}���h�ŕǃL�b�N�̗͂��ς��l�ɂ���
        else if (IsWalled())
        {
            m_rb.useGravity = false;
            if (Input.GetButtonDown("Jump"))
            {
                m_Animator.SetTrigger("WallJump");
                //StartCoroutine("WallJumpTime");
            }
        }
        else
        {
            m_rb.useGravity = true;
            return;
        }
    }
    IEnumerable WallJumpTime()
    {
        int i = 0;
        while(i <= 1)
        {
            m_h = 0;
            yield return new WaitForSeconds(0.5f);
        }
    }
    //AnimationEvent�ŌĂԊ֐�
    //---------------------------------------------------------
    void WalljumpPower()
    {
        m_Audio.PlayOneShot(jumpSound);
        //Vector3 vec = transform.up + m_hitInfo.normal;
        m_rb.AddForce(transform.up * wallJumpPower, ForceMode.Impulse);
        StartCoroutine("WallJumpTime");
    }
    void AttackJud()
    {
        Attacked = !Attacked;
    }

    void WeaponActionMethod()
    {
        //�ʏ�U���̏���
        if(Input.GetButtonDown("Attack"))
        {
            m_Animator.SetTrigger(m_equipmentWeapon.name + "Attack");
        }

        //���ߍU���̏���(�|��̃A�j���[�V���������̏����j
        if (Charge && ChrageAttackCount < 1800 )
        {
            ChrageAttackCount++;
        }
        if (Input.GetButtonUp("Attack"))
        {
            if (ChrageAttackCount > 0)
            {
                ChrageAttackCount = 0;
            }
        }
        m_Animator.SetBool("Charge", Input.GetButton("Attack"));
        //���C���ƃT�u�̕\����؂�ւ���
        if (Input.GetButtonDown("WeaponChange") && !Charge && !Attacked)
        {
            m_weaponSwitch = !m_weaponSwitch;
            m_mainWeapon.SetActive(m_weaponSwitch);
            m_subWeapon.SetActive(!m_weaponSwitch);

            //���C���ƃT�u�̕����؂�ւ���
            if (m_weaponSwitch)
            {
                m_equipmentWeapon = m_mainWeapon;
            }
            else
            {
                m_equipmentWeapon = m_subWeapon;
            }
        }
    }

#if UNITY_EDITOR
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Vector3 isGroundCenter = footPos.transform.position;
    //    Gizmos.DrawWireSphere(footPos.transform.position + Vector3.down * distance, isGroundRengeRadios);

    //    Vector3 isWallCenter = footPos.transform.position;
    //    Ray ray = new Ray(isWallCenter, Vector3.right);
    //    bool hitFlg = Physics.Raycast(ray, out m_hitInfo, distance, wallMask);

    //    Vector3 isWallCenter2 = footPos.transform.position;
    //    Ray ray2 = new Ray(isWallCenter, Vector3.right);
    //    bool hitFlg2 = Physics.Raycast(ray, out m_hitInfo, distance, wallMask);
    //}
#endif
}
