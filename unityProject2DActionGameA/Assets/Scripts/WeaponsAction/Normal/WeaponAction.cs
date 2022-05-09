using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAction : MonoBehaviour
{
    [SerializeField, Tooltip("���ߍU���̗��߃J�E���^�[")] int m_chrageAttackCounter = 1800;
    [SerializeField, Tooltip("WeaponChanger���i�[����ϐ�")] WeaponChanger m_weaponChanger = default;
    [SerializeField, Tooltip("Animator���i�[����ϐ�")] Animator m_animator = default;

    [Tooltip("���ߍU���̗��ߎ���")] protected int m_chrageAttackCount = 0;
    [Tooltip("")] bool m_attacked = false;

    public bool Attacked { get => m_attacked; set => m_attacked = value; }

    public virtual void IsStart()
    {
        //Start�֐��ōs�����������͂����ɏ���
    }

    public virtual void WeaponChargeAttackMethod()
    {
       //���킲�Ƃ̗��ߍU���̏����������ɏ���
    }
    void WeaponAttackMethod()
    {
        //�ʏ�U���̏���
        if (Input.GetButtonDown("Attack"))
        {
            m_animator.SetTrigger(m_weaponChanger.EquipmentWeapon.name + "Attack");
        }

        //���ߍU���̏���(�|��̃A�j���[�V���������̏����j
        if (Input.GetButton("Attack") && m_chrageAttackCount < m_chrageAttackCounter)
        {
            m_chrageAttackCount++;
        }
        if (Input.GetButtonUp("Attack"))
        {
            //m_bow.BulletInstance(m_chrageAttackCount);
            if (m_chrageAttackCount > 0)
            {
                m_chrageAttackCount = 0;
            }
        }
        m_animator.SetBool("Charge", Input.GetButton("Attack"));
    }
    void Start()
    {
        IsStart();
    }

    void Update()
    {
        WeaponAttackMethod();
        WeaponChargeAttackMethod();
    }

    void AttackJud()
    {
        m_attacked = !m_attacked;
    }
}
