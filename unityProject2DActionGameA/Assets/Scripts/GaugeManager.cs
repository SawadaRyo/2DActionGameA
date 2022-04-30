using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeManager : MonoBehaviour //HP��SAP�Q�[�W�Ȃǂ𓝊�����X�N���v�g
{
    [SerializeField, Tooltip("�I�u�W�F�N�g��HP�̏��")] int m_maxHP = 20;
    [SerializeField, Tooltip("�I�u�W�F�N�g�̌��݂�HP")] protected int m_HP = 20;
    [SerializeField, Tooltip("HP�Q�[�W��slider")] Slider m_HPGague;
    [SerializeField, Tooltip("SAP�Q�[�W��slider")] Slider m_SAPGague;
    [SerializeField] float m_rigitDefensePercentage = 0.8f;
    [SerializeField] float m_fireDifansePercentage = 0.8f;
    [SerializeField] float m_elekeDifansePercentage = 0.8f;
    [SerializeField] float m_frozenDifansePercentage = 0.8f;
    [SerializeField, Tooltip("�v���C���[�̃_���[�W�T�E���h")] AudioClip m_damageSound;
    [Tooltip("�I�u�W�F�N�g�̕K�E�Z�Q�[�W")] int m_SAP = 0;
    [Tooltip("�K�E�Z�Q�[�W�̏��")] int m_maxSAP = 20;
    [Tooltip("�I�u�W�F�N�g�̐�������")] protected bool m_living = false;

    public int HP { get => m_HP; set => m_HP = value; }
    public int SAP { get => m_SAP; set => m_SAP = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (m_HPGague != null)
        {
            //m_HP = m_maxHP;
            m_HPGague.value = m_HP;
            m_living = true;
        }

        if (m_SAPGague != null && this.gameObject.tag == "Player")
        {
            m_SAPGague.value = m_SAP;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.tag == "Player")
        {
            m_HPGague.value = HP;
            m_SAPGague.value = SAP;
        }
        MaxGagueController();
    }
    void MaxGagueController()
    {
        if (HP > m_maxHP)
        {
            HP -= (HP - m_maxHP);
        }

        if (m_SAP > m_maxSAP)
        {
            m_SAP -= (m_SAP - m_maxSAP);
        }
    }
    //�_���[�W�v�Z
    public void DamageMethod(int weaponPower, int firePower, int elekePower,int frozenPower)
    {
        var damage = weaponPower * m_rigitDefensePercentage
            + firePower * m_fireDifansePercentage
            + elekePower * m_elekeDifansePercentage
            + frozenPower * m_frozenDifansePercentage;
        HP -= (int)damage;
        //��������
        if (HP == 0)
        {
            m_living = false;
        }
        else return;
    }
}
