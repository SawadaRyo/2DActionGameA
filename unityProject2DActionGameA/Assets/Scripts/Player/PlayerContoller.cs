using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContoller : MonoBehaviour
{
    [SerializeField, Tooltip("�v���C���[�̈ړ����x")] float m_speed = 5f;
    [SerializeField, Tooltip("�_�b�V���̔{��")] float m_dashSpeed = 10f;
    //[SerializeField, Tooltip("�󒆂ł̈ړ����x")] float m_grindFroth = 5f;
    [SerializeField, Tooltip("�v���C���[�̃W�����v��")] float m_jump = 5f;
    [SerializeField, Tooltip("�ڒn�����Ray�̎˒�")] float m_graundDistance = 1f;
    [SerializeField, Tooltip("�ǂ̐ڐG�����Ray�̎˒�")] float m_walldistance = 0.5f;
    [SerializeField, Tooltip("SphierCast�̔��a")] float m_isGroundRengeRadios = 1f;
    [SerializeField, Tooltip("�ǃW�����̗�")] float m_wallJumpPower = 7f;
    [SerializeField, Tooltip("�W�����v�̃T�E���h")] AudioClip m_jumpSound;

    [SerializeField, Tooltip("Ray�̎ˏo�_")] Transform m_footPos;
    [SerializeField, Tooltip("�ڒn�����LayerMask")] LayerMask m_groundMask = ~0;
    [SerializeField, Tooltip("�ǂ̐ڐG����")] LayerMask m_wallMask = ~0;

    [SerializeField, Tooltip("�A�j���[�V�������擾����ׂ̕ϐ�")] Animator m_Animator;
    [SerializeField, Tooltip("GamManager���i�[����ϐ�")] GameManager m_gameManager;
    [SerializeField, Tooltip("WeaponChanger���i�[����ϐ�")] WeaponChanger m_weaponChanger;

    
    //[Tooltip("�ړ��͎w��")] float moveSpeed = 0f;
    [Tooltip("Player�̐U��������x")] float m_turnSpeed = 25f;
    [Tooltip("���ړ��̃x�N�g��")] float m_h;
    [Tooltip("�X���C�f�B���O�̔���")] bool m_isDash = false;
    bool m_ableMove = true;
    [Tooltip("Rigidbody�R���|�[�l���g�̎擾")]Rigidbody m_rb;
    //List<ItemBase> m_ItemList = new List<ItemBase>();
    [Tooltip("�I�[�f�B�I���擾����ׂ̕ϐ�")] AudioSource m_Audio;
    //[Tooltip("Player�̌����Ă������")] Quaternion orgLocalQuaternion;
    RaycastHit m_hitInfo;
    public Vector3 NormalOfStickingWall { get; private set; } = Vector3.zero;
    delegate void PlayerMethod();
    PlayerMethod m_playerMethod = default;
    

    void Start()
    {
        //orgLocalQuaternion = this.transform.localRotation;
        m_rb = GetComponent<Rigidbody>();
        m_Audio = gameObject.AddComponent<AudioSource>();

    }
    void OnEnable()
    {
        m_playerMethod += WallJumpMethod;
        m_playerMethod += MoveMethod;
        m_playerMethod += JumpMethod;
    }
    void OnDisable()
    {
        m_playerMethod -= WallJumpMethod;
        m_playerMethod -= MoveMethod;
        m_playerMethod -= JumpMethod;
    }
    void Update()
    {
        if (!m_gameManager.IsGame) return;
        else
        {
            m_playerMethod();
        }
    }

    //Player�̓����������������ꂽ�֐�
    //----------------------------------------------
    void MoveMethod()
    {
        m_h = Input.GetAxisRaw("Horizontal");
        m_isDash = Input.GetButton("Dash");
        var moveSpeed = 0f;
        Vector3 velocity = m_rb.velocity;

        //�v���C���[�̕����]��
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
        if (m_h != 0 && m_ableMove)
        { 
            moveSpeed = m_speed;
        }

        //�_�b�V���R�}���h
        if (m_isDash)
        {
            if (!IsGrounded() && m_isDash) return;
            moveSpeed = m_dashSpeed;
        }

        velocity.x = m_h * moveSpeed;
        m_rb.velocity = new Vector3(velocity.x, m_rb.velocity.y, 0);
        m_Animator.SetFloat("MoveSpeed", Mathf.Abs(velocity.x));
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Pendulam" || other.gameObject.tag == "Ffield")
        {
            transform.parent = other.gameObject.transform;
        }
    }

    void OnCollisionExit()
    {
        transform.parent = null;
    }

    bool IsGrounded()
    {
        Vector3 isGroundCenter = m_footPos.transform.position;
        Ray ray = new Ray(isGroundCenter, Vector3.down);
        bool hitFlg = Physics.SphereCast(ray, m_isGroundRengeRadios, out _, m_graundDistance, m_groundMask);
        return hitFlg;
    }
    bool IsWalled()
    {
        Vector3 isWallCenter = m_footPos.transform.position;
        Ray rayRight = new Ray(isWallCenter, Vector3.right);
        Ray rayLeft = new Ray(isWallCenter, Vector3.left);
        bool hitFlg = Physics.Raycast(rayRight, out m_hitInfo, m_walldistance, m_wallMask)
                   || Physics.Raycast(rayLeft, out m_hitInfo, m_walldistance, m_wallMask);
        return hitFlg;
    }
    void JumpMethod()
    {
        //�W�����v�̏���
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            m_Audio.PlayOneShot(m_jumpSound);
            m_rb.AddForce(0f, m_jump, 0f, ForceMode.Impulse);
        }
        m_Animator.SetBool("Jump", !IsGrounded());
        
    }
    void WallJumpMethod()
    {
        m_Animator.SetBool("WallGrip", IsWalled() && m_h != 0);
        m_weaponChanger.EquipmentWeapon.SetActive(!IsWalled());
        if (IsGrounded())
        {
            m_rb.mass = 1f;
            return;
        }
        //ToDo�ړ��R�}���h�ŕǃL�b�N�̗͂��ς��l�ɂ���
        else if (IsWalled() && m_h != 0)
        {
            //m_rb.mass = 0.5f;
            if (Input.GetButtonDown("Jump"))
            {
                m_Animator.SetTrigger("WallJump");
            }
        }
        else
        {
            m_rb.useGravity = true;
            return;
        }
    }
    //AnimatorEvent�ŌĂԊ֐�
    //----------------------------------------------
    void MoveJud()
    {
        m_ableMove = !m_ableMove;
    }

    void WallJump()
    {
        m_Audio.PlayOneShot(m_jumpSound);
        Vector3 vec = transform.up + m_hitInfo.normal;
        m_rb.AddForce(vec * m_wallJumpPower, ForceMode.Impulse);
    }
}
