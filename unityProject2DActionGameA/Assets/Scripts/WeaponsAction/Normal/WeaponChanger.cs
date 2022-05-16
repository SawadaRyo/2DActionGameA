using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChanger : MonoBehaviour
{
    [SerializeField, Tooltip("���C������")] GameObject m_mainWeapon = default;
    [SerializeField, Tooltip("�T�u����")] GameObject m_subWeapon = default;

    [Tooltip("����؂�ւ�")] bool m_weaponSwitch = true;
    [Tooltip("�������̕���")] GameObject m_equipmentWeapon = default;
    WeaponAction m_weaponAction = default;

    public GameObject EquipmentWeapon { get => m_equipmentWeapon; set => m_equipmentWeapon = value; }
    void Start()
    {
        m_weaponAction = GetComponent<WeaponAction>();

        m_equipmentWeapon = m_mainWeapon;
        if (m_mainWeapon != null)
        {
            m_mainWeapon.SetActive(true);
            m_subWeapon.SetActive(false);
        }
        m_weaponSwitch = true;
    }
    void Update()
    {
        WeaponChangeMethod();
    }
    void WeaponChangeMethod()
    {
        //����̃��C���ƃT�u�̕\����؂�ւ���
        if (Input.GetButtonDown("WeaponChange")
            && !Input.GetButton("Attack")
            && !m_weaponAction.Attacked)
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
}
