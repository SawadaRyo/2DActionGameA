using UnityEngine;
using UnityEngine.UI;

public class PlayerContoller : SingletonBehaviour<PlayerContoller>
{
    [Header("Ground")]
    [SerializeField, Tooltip("�v���C���[�̈ړ����x")] float m_speed = 5f;
    [SerializeField, Tooltip("�_�b�V���̔{��")] float m_dashSpeed = 10f;
    [Tooltip("�v���C���[�̐U��������x")] float m_turnSpeed = 25f;
    [Tooltip("���ړ��̃x�N�g��")] float m_h;
    [Tooltip("�X���C�f�B���O�̔���")] bool m_isDash = false;
    [Tooltip("")] GameObject m_otherCollider;

    [Header("Jump")]
    [SerializeField, Tooltip("�v���C���[�̃W�����v��")] float m_jump = 5f;
    [SerializeField, Tooltip("�ڒn�����Ray�̎˒�")] float m_graundDistance = 1f;
    [SerializeField, Tooltip("�ڒn�����Ray�̎ˏo�_")] Transform m_footPos;
    [SerializeField, Tooltip("�ڒn�����SphierCast�̔��a")] float m_isGroundRengeRadios = 1f;
    [SerializeField, Tooltip("�ڒn�����LayerMask")] LayerMask m_groundMask = ~0;

    [Header("WallJump")]
    [SerializeField, Tooltip("�v���C���[�̕ǃL�b�N�̗�")] float m_wallJump = 7f;
    [SerializeField, Tooltip("�ǂ����藎���鑬�x")] float m_wallSlideSpeed = 0.4f;
    [SerializeField, Tooltip("�ǂ̐ڐG�����Ray�̎˒�")] float m_walldistance = 0.1f;
    [SerializeField, Tooltip("�ǂ̐ڐG�����SphierCast�̔��a")] float m_isWallRengeRadios = 0.5f;
    [SerializeField, Tooltip("�ǂ̐ڐG����")] LayerMask m_wallMask = ~0;
    [SerializeField, Tooltip("�ڒn�����Ray�̎ˏo�_")] Transform m_gripPos;
    [Tooltip("�ǂ̂��藎������")] bool m_slideWall = false;

    [Header("Animation")]
    [Tooltip("Animation���擾����ׂ̕ϐ�")] Animator m_animator;

    [Header("Audio")]
    [SerializeField, Tooltip("�W�����v�̃T�E���h")] AudioClip m_jumpSound;
    [Tooltip("�I�[�f�B�I���擾����ׂ̕ϐ�")] AudioSource m_audio;

    [Tooltip("�ړ��\�����肷��ϐ�")] bool m_ableMove = true;
    [Tooltip("Rigidbody�R���|�[�l���g�̎擾")] Rigidbody m_rb;
    [Tooltip("�v���C���[�̈ړ��x�N�g�����擾")] Vector3 m_velo = default;
    [Tooltip("�ڐG���Ă���I�u�W�F�N�g�̏��")] RaycastHit m_hitInfo;
    [Tooltip("�������̕���")] WeaponBase m_eWeapon = default;

    public Vector3 NormalOfStickingWall { get; private set; } = Vector3.zero;
    delegate void WeaponAttacks();

    void Start()
    {
        //orgLocalQuaternion = this.transform.localRotation;
        m_eWeapon = WeaponChanger.Instance.EquipmentWeapon;
        m_rb = GetComponent<Rigidbody>();
        m_audio = gameObject.AddComponent<AudioSource>();
        m_velo = m_rb.velocity;
        m_animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.Instance.IsGame)
        {
            MoveMethod();
            JumpMethod();
            WallJumpMethod();
        }
    }

    //Player�̓����������������ꂽ�֐�----------------------------------------------//
    void MoveMethod()
    {
        m_h = Input.GetAxisRaw("Horizontal");
        m_isDash = Input.GetButton("Dash");
        var moveSpeed = 0f;

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
            //�_�b�V���R�}���h
            if (m_isDash)
            {
                moveSpeed = m_dashSpeed;
            }
            else
            {
                moveSpeed = m_speed;
            }
        }

        m_velo.x = m_h * moveSpeed;
        m_rb.velocity = new Vector3(m_velo.x, m_rb.velocity.y, 0);
        m_animator.SetFloat("MoveSpeed", Mathf.Abs(m_velo.x));
    }
    bool IsGrounded()
    {
        Vector3 isGroundCenter = m_footPos.transform.position;
        Ray ray = new Ray(isGroundCenter, Vector3.down);
        bool hitFlg = Physics.SphereCast(ray, m_isGroundRengeRadios, out _, m_graundDistance, m_groundMask);
        return hitFlg;
    }
    void JumpMethod()
    {
        //�W�����v�̏���
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            m_audio.PlayOneShot(m_jumpSound);
            m_rb.AddForce(0f, m_jump, 0f, ForceMode.Impulse);
        }
        m_animator.SetBool("Jump", !IsGrounded());

    }
    bool IsWalled()
    {
        if (IsGrounded()) return false;

        Vector3 isWallCenter = m_gripPos.transform.position;
        Ray rayRight = new Ray(isWallCenter, Vector3.right);
        Ray rayLeft = new Ray(isWallCenter, Vector3.left);
        bool hitFlg = Physics.Raycast(rayRight, out m_hitInfo, m_walldistance, m_wallMask)
                   || Physics.Raycast(rayLeft, out m_hitInfo, m_walldistance, m_wallMask);
        return hitFlg;
    }
    bool IsOtherWalled()
    {
        if (IsGrounded()) return false;

        Vector3 isWallCenter = m_gripPos.transform.position;
        Ray rayRight = new Ray(isWallCenter, Vector3.right);
        Ray rayLeft = new Ray(isWallCenter, Vector3.left);
        bool otherWall = Physics.Raycast(rayRight, out m_hitInfo, m_walldistance, m_wallMask)
                   || Physics.Raycast(rayLeft, out m_hitInfo, m_walldistance, m_wallMask);
        if (m_hitInfo.collider.gameObject.layer != m_wallMask)
        {
            m_h = 0;
        }
        return otherWall;
    }
    void WallJumpMethod()
    {
        m_animator.SetBool("WallGrip", IsWalled());
        m_eWeapon.RendererActive(!IsWalled());
        if (IsGrounded())
        {
            m_slideWall = false;
            return;
        }
        //ToDo�ړ��R�}���h�ŕǃL�b�N�̗͂��ς��l�ɂ���
        else if (IsWalled())
        {
            m_slideWall = true;
            if (Input.GetButtonDown("Jump"))
            {
                m_rb.velocity = new Vector3(m_rb.velocity.x, m_jump, 0);
            }
        }
        else
        {
            m_slideWall = false;
        }


        if (m_slideWall)
        {
            m_rb.velocity = new Vector3(m_rb.velocity.x, Mathf.Clamp(m_rb.velocity.y, -m_wallSlideSpeed, float.MaxValue));
        }
    }
    //AnimatorEvent�ŌĂԊ֐�----------------------------------------------//
    void MoveJud()
    {
        m_ableMove = !m_ableMove;
    }
    void WallJump()
    {
        m_audio.PlayOneShot(m_jumpSound);
        Vector3 vec = transform.up + m_hitInfo.normal;
        m_rb.AddForce(vec * m_wallJump, ForceMode.Impulse);
    }
    void OnCollisionEnter(Collision other)
    {
        var otherCollider = other.gameObject;
        if (otherCollider)
        {
            if (otherCollider.tag == "Pendulam" || otherCollider.tag == "Ffield")
            {
                transform.parent = other.gameObject.transform;
            }
        }

    }
    void OnCollisionExit()
    {
        transform.parent = null;
    }

}
