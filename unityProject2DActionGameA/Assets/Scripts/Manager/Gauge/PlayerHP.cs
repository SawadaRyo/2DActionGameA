using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [SerializeField, Tooltip("�I�u�W�F�N�g�̕K�E�Z�Q�[�W�̏��")] int m_maxSAP = 20;
    [SerializeField, Tooltip("�I�u�W�F�N�g��HP�̏��")] int m_maxHP = 20;
    [SerializeField, Tooltip("�����h���")] float m_rigitDefensePercentage = 0.8f;
    [SerializeField, Tooltip("���h���")] float m_fireDifansePercentage = 0.8f;
    [SerializeField, Tooltip("�d�C�h���")] float m_elekeDifansePercentage = 0.8f;
    [SerializeField, Tooltip("�X���h���")] float m_frozenDifansePercentage = 0.8f;
    [SerializeField, Tooltip("HP�Q�[�W��slider")] Slider m_HPGague = default;
    [SerializeField, Tooltip("SAP�Q�[�W��slider")] Slider m_SAPGague = default;
    [SerializeField, Tooltip("�v���C���[�̃_���[�W�T�E���h")] AudioClip m_damageSound;

    [Tooltip("�I�u�W�F�N�g�̌��݂̕K�E�Z�Q�[�W")] int m_sap = 0;
    [Tooltip("�I�u�W�F�N�g�̌��݂�HP")] int m_hp;
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
            m_HPGague.value = m_hp;
            MaxGagueController();
        }
        else
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            m_animator.SetBool("Death", true);
        }
    }
    //�_���[�W�v�Z
    public void DamageMethod(int weaponPower, int firePower, int elekePower, int frozenPower)
    {
        var damage = weaponPower * m_rigitDefensePercentage
            + firePower * m_fireDifansePercentage
            + elekePower * m_elekeDifansePercentage
            + frozenPower * m_frozenDifansePercentage;
        m_hp -= (int)damage;
        //��������
        if (m_hp <= 0)
        {
            m_living = false;
            Debug.Log(m_living);
        }
        else return;
    }

    void MaxGagueController()
    {
        if (m_hp > m_maxHP)
        {
            m_hp -= (m_hp - m_maxHP);
        }

        if (m_sap > m_maxSAP)
        {
            m_sap -= (m_sap - m_maxSAP);
        }
    }
    public void HPPuls(int hpPuls)
    {
        m_hp += hpPuls;
    }
    public void SAPPuls(int sapPuls)
    {
        m_sap += sapPuls;
        m_SAPGague.value = m_sap;
    }

    public int SAP { get => m_sap; set => m_sap = value; }
}
