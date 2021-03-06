using System;
using UnityEngine;

public abstract class WeaponAction : MonoBehaviour, IWeaponAction
{
    [SerializeField, Tooltip("溜め攻撃第1段階")] protected float m_chargeLevel1 = 1f;
    [SerializeField, Tooltip("溜め攻撃第2段階")] protected float m_chargeLevel2 = 3f;

    [Tooltip("")] 
    bool m_attacked = false;
    [Tooltip("溜め攻撃の溜め時間")] 
    protected float m_chrageCount = 0;
    [Tooltip("武器名")] 
    protected string m_weaponName;
    [Tooltip("PlayerのAnimatorを格納する変数")]
    protected Animator m_animator = default;
    [Tooltip("WeaponBaseを格納する変数")] 
    protected WeaponBase m_weaponBase = default;
    [Tooltip("PlayerAnimationStateを格納する変数")]
    PlayerAnimationState m_state;

    public bool Attacked => m_attacked;
    public abstract void WeaponChargeAttackMethod(float chrageCount);

    public virtual void Start()
    {
        m_state = PlayerAnimationState.Instance;
        m_weaponName = WeaponEquipment.Instance.EquipmentWeapon.name;
        m_animator = PlayerContoller.Instance.GetComponent<Animator>();
        m_weaponBase = GetComponent<WeaponBase>();
    }
   
    public void WeaponAttackMethod(string weaponName)
    {
        if (!m_state.AbleInput) return;

        ////通常攻撃の処理
        if (Input.GetButtonDown("Attack"))
        {
            m_animator.SetTrigger(weaponName);
        }

        //溜め攻撃の処理(弓矢のアニメーションもこの処理）
        if (Input.GetButton("Attack") && m_chrageCount < m_chargeLevel2)
        {
            m_chrageCount += Time.deltaTime;
        }
        else if (Input.GetButtonUp("Attack"))
        {
            if(m_chrageCount > 0f)
            {
                WeaponChargeAttackMethod(m_chrageCount);
            }
            m_chrageCount = 0f;
        }

        m_animator.SetBool("Charge", Input.GetButton("Attack"));
    }

    
}
