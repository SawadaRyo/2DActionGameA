using UnityEngine;
using UnityEngine.UI;

public class PlayerGauge : MonoBehaviour
{
    [SerializeField, Tooltip("�I�u�W�F�N�g�̕K�E�Z�Q�[�W�̏��")] float m_maxSAP = 20;
    [SerializeField, Tooltip("�I�u�W�F�N�g��HP�̏��")] float m_maxHP = 20;
    [SerializeField, Tooltip("HP�Q�[�W��slider")] Slider m_HPGague = default;
    [SerializeField, Tooltip("SAP�Q�[�W��slider")] Slider m_SAPGague = default;
    [SerializeField, Tooltip("�v���C���[�̃_���[�W�T�E���h")] AudioClip m_damageSound;

    [Tooltip("�����h���")] float m_rigitDefensePercentage = 0f;
    [Tooltip("���h���")] float m_fireDifansePercentage = 0f;
    [Tooltip("�d�C�h���")] float m_elekeDifansePercentage = 0f;
    [Tooltip("�X���h���")] float m_frozenDifansePercentage = 0f;
    [Tooltip("�I�u�W�F�N�g�̌��݂̕K�E�Z�Q�[�W")] float m_sap = 0f;
    [Tooltip("�I�u�W�F�N�g�̌��݂�HP")] float m_hp = 0f;
    [Tooltip("�I�u�W�F�N�g�̐�������")] bool m_living = true;
    [Tooltip("Animator�̊i�[�ϐ�")] Animator m_animator;

    void Awake()
    {
        m_animator = gameObject.GetComponent<Animator>();
    }
    void Start()
    {
        if (m_HPGague != null && m_SAPGague != null)
        {
            m_hp = m_maxHP;
            m_sap = m_maxSAP;
            m_HPGague.value = m_hp;
            m_SAPGague.value = m_sap;
            m_living = true;
        }
    }

    void Update()
    {
        if (m_living)
        {
            return;
        }
        else
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            m_animator.SetBool("Death", true);
        }
    }
    //�_���[�W�v�Z
    public void DamageMethod(float weaponPower, float firePower, float elekePower, float frozenPower)
    {
        var damage = weaponPower * m_rigitDefensePercentage
            + firePower * m_fireDifansePercentage
            + elekePower * m_elekeDifansePercentage
            + frozenPower * m_frozenDifansePercentage;
        m_hp -= (int)damage;
        m_HPGague.value = m_hp;
        //��������
        if (m_hp <= 0)
        {
            m_living = false;
        }
        else return;
    }

    
    public void HPPuls(int hpPuls)
    {
        m_hp += hpPuls;
        if (m_hp > m_maxHP)
        {
            m_hp -= (m_hp - m_maxHP);
        }
        m_HPGague.value = m_hp/m_maxHP;
    }
    public void SAPPuls(int sapPuls)
    {
        m_sap += sapPuls;
        if (m_sap > m_maxSAP)
        {
            m_sap -= (m_sap - m_maxSAP);
        }
        m_SAPGague.value = m_sap/m_maxSAP;
    }

    public float SAP { get => m_sap; set => m_sap = value; }
}
