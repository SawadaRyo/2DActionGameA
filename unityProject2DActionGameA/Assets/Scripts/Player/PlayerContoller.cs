using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerContoller : SingletonBehaviour<PlayerContoller>
{
    [Header("Ground")]

    [SerializeField, Tooltip("プレイヤーの移動速度")]
    float m_speed = 5f;
    [Tooltip("変化直前のプレイヤーの移動速度")]
    float m_beroreSpeed = 0f;
    [SerializeField, Tooltip("ダッシュの倍率")] 
    float m_dashSpeed = 10f;
    [SerializeField,Tooltip("プレイヤーの振り向き速度")]
    float m_turnSpeed = 25f;
    [Tooltip("横移動のベクトル")] 
    float m_h;
    [Tooltip("スライディングの判定")] 
    bool m_isDash = false;

    [Header("Jump")]

    [SerializeField, Tooltip("プレイヤーのジャンプ力")] 
    float m_jump = 5f;
    [SerializeField, Tooltip("接地判定のRayの射程")] 
    float m_graundDistance = 1f;
    [SerializeField, Tooltip("接地判定のRayの射出点")] 
    Transform m_footPos;
    [SerializeField, Tooltip("接地判定のSphierCastの半径")] 
    float m_isGroundRengeRadios = 1f;
    [SerializeField, Tooltip("接地判定のLayerMask")]
    LayerMask m_groundMask = ~0;

    [Header("WallJump")]

    [SerializeField, Tooltip("プレイヤーの壁キックの力")]
    float m_wallJump = 7f;
    [SerializeField, Tooltip("壁をずり落ちる速度")]
    float m_wallSlideSpeed = 0.8f;
    [SerializeField, Tooltip("壁の接触判定のRayの射程")] 
    float m_walldistance = 0.1f;
    [SerializeField, Tooltip("壁の接触判定")]
    LayerMask m_wallMask = ~0;
    [SerializeField, Tooltip("接地判定のRayの射出点")] 
    Transform m_gripPos;
    [Tooltip("壁のずり落ち判定")] 
    bool m_slideWall = false;

    [Header("Animation")]

    [Tooltip("Animationを取得する為の変数")] 
    Animator m_animator;

    [Header("Audio")]

    [SerializeField, Tooltip("ジャンプのサウンド")] 
    AudioClip m_jumpSound;
    [Tooltip("オーディオを取得する為の変数")]
    AudioSource m_audio;

    [Tooltip("PlayerAnimationStateを格納する変数")]
    PlayerAnimationState m_animState;
    [Tooltip("Rigidbodyコンポーネントの取得")]
    Rigidbody m_rb;
    [Tooltip("プレイヤーの移動ベクトルを取得")]
    Vector3 m_velo = default;
    [Tooltip("接触しているオブジェクトの情報")] 
    RaycastHit m_hitInfo;

    public Vector3 NormalOfStickingWall { get; private set; } = Vector3.zero;
    delegate void WeaponAttacks();

    void Start()
    {
        //orgLocalQuaternion = this.transform.localRotation;
        m_rb = GetComponent<Rigidbody>();
        m_audio = gameObject.AddComponent<AudioSource>();
        m_velo = m_rb.velocity;
        m_animator = GetComponent<Animator>();
        m_animState = PlayerAnimationState.Instance;
    }

    void Update()
    {
        if (GameManager.Instance.IsGame)
        {
            m_h = Input.GetAxisRaw("Horizontal");
            WallJumpMethod();
            MoveMethod();
            JumpMethod();
        }
    }

    //Playerの動きを処理が書かれた関数----------------------------------------------//
    void MoveMethod()
    {
        m_isDash = Input.GetButton("Dash");
        var moveSpeed = 0f;

        //プレイヤーの方向転換
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

        //プレイヤーの移動
        if (m_h != 0 && m_animState.AbleMove)
        {
            if(IsGrounded())
            {
                if (m_isDash)
                {
                    moveSpeed = m_dashSpeed;
                }
                else
                {
                    moveSpeed = m_speed;
                }
                m_beroreSpeed = moveSpeed;
            }
            else
            {
                moveSpeed = m_beroreSpeed;
                if(!m_isDash || IsWalled())
                {
                    moveSpeed = m_speed;
                }
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
        //ジャンプの処理
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            //m_audio.PlayOneShot(m_jumpSound);
            m_rb.AddForce(0f, m_jump, 0f, ForceMode.Impulse);
        }
        m_animator.SetBool("Jump", !IsGrounded());

    }
    public bool IsWalled()
    {
        if (IsGrounded()) return false;

        Vector3 isWallCenter = m_gripPos.transform.position;
        Ray rayRight = new Ray(isWallCenter, Vector3.right);
        Ray rayLeft = new Ray(isWallCenter, Vector3.left);

        bool hitflg = Physics.Raycast(rayRight, out m_hitInfo, m_walldistance, m_wallMask)
        || Physics.Raycast(rayLeft, out m_hitInfo, m_walldistance, m_wallMask);

        return hitflg;
    }

    void WallJumpMethod()
    {
        m_animator.SetBool("WallGrip", IsWalled());
        if (IsGrounded())
        {
            m_slideWall = false;
            return;
        }
        //ToDo移動コマンドで壁キックの力が変わる様にする
        else if (IsWalled())
        {
            m_slideWall = true;
            m_h = 0;
            if (Input.GetButtonDown("Jump"))
            {
                Vector3 vec = transform.up + m_hitInfo.normal * 3;
                m_rb.AddForce(vec * m_wallJump, ForceMode.Impulse);
                //m_rb.velocity = new Vector3(m_rb.velocity.x, m_jump, 0);
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
    //AnimatorEventで呼ぶ関数----------------------------------------------//
    
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 isGroundCenter = m_footPos.transform.position;
        Ray ray = new Ray(isGroundCenter, Vector3.right);
        Gizmos.DrawRay(ray);
    }
}
