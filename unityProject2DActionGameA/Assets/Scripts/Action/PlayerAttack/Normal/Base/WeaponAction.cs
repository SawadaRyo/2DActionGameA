using System;
using UnityEngine;

public abstract class WeaponAction : MonoBehaviour, IWeaponAction
{
    [SerializeField, Tooltip("���ߍU����1�i�K")] protected float m_chargeLevel1 = 1f;
    [SerializeField, Tooltip("���ߍU����2�i�K")] protected float m_chargeLevel2 = 1.5f;
    [SerializeField, Tooltip("���ߍU����3�i�K")] protected float m_chargeLevel3 = 3f;

    [Tooltip("")] 
    bool m_attacked = false;
    [Tooltip("���ߍU���̗��ߎ���")] 
    protected float m_chrageCount = 0;
    [Tooltip("���햼")] 
    protected string m_weaponName;
    [Tooltip("Player��Animator���i�[����ϐ�")]
    protected Animator m_animator = default;
    [Tooltip("WeaponBase���i�[����ϐ�")] 
    protected WeaponBase m_weaponBase = default;
    [Tooltip("PlayerAnimationState���i�[����ϐ�")]
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

        ////�ʏ�U���̏���
        if (Input.GetButtonDown("Attack"))
        {
            m_animator.SetTrigger(weaponName);
        }

        //���ߍU���̏���(�|��̃A�j���[�V���������̏����j
        if (Input.GetButton("Attack") && m_chrageCount < m_chargeLevel3)
        {
            m_chrageCount += Time.deltaTime;
        }
        else if (Input.GetButtonUp("Attack"))
        {
            if(m_chrageCount > m_chargeLevel1)
            {
                WeaponChargeAttackMethod(m_chrageCount);
            }
            m_chrageCount = 0f;
        }

        m_animator.SetBool("Charge", Input.GetButton("Attack"));
    }

    
}
