using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeManager : MonoBehaviour //HP��SAP�Q�[�W�Ȃǂ𓝊�����X�N���v�g
{
    [SerializeField, Tooltip("HP�̏��")] int m_maxHP = 20;
    [Tooltip("�K�E�Z�Q�[�W�̏��")] int m_maxSAP = 20;
    [SerializeField, Tooltip("�I�u�W�F�N�g��HP")] int m_HP = 20;
    [Tooltip("�I�u�W�F�N�g�̕K�E�Z�Q�[�W")] int m_SAP = 0;
    [Tooltip("�I�u�W�F�N�g�̐�������")] bool m_living = false;
    [SerializeField, Tooltip("HP�Q�[�W��slider")] Slider m_HPGague;
    [SerializeField, Tooltip("SAP�Q�[�W��slider")] Slider m_SAPGague;
    [SerializeField] Image m_HPGagieHandle;
    [SerializeField] Image m_SAPGagieHandle;

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
        m_HPGague.value = HP;
        m_SAPGague.value = SAP;
        if (HP == 0)
        {
            m_living = false;
        }
        MaxGagueController();
    }
    void MaxGagueController()
    {
        if (HP > m_maxHP)
        {
            HP -= (HP - m_maxHP);
        }
        else if (HP == m_maxHP && m_HPGagieHandle != null)
        {
            m_HPGagieHandle.color = Color.green;
        }

        if (m_SAP > m_maxSAP)
        {
            m_SAP -= (m_SAP - m_maxSAP);
        }
        else if (m_SAP == m_maxSAP && m_SAPGagieHandle != null)
        {
            m_SAPGagieHandle.color = Color.yellow;
        }
    }
}
