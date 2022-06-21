using UnityEngine;

public class GaugeManager : MonoBehaviour //�G��v���C���[�̗̑͂��Ǘ�����X�N���v�g
{
    [SerializeField, Tooltip("�I�u�W�F�N�g��HP�̏��")] protected int m_maxHP = 20;
    [SerializeField] float m_rigitDefensePercentage = 0.8f;
    [SerializeField] float m_fireDifansePercentage = 0.8f;
    [SerializeField] float m_elekeDifansePercentage = 0.8f;
    [SerializeField] float m_frozenDifansePercentage = 0.8f;
    [SerializeField, Tooltip("�v���C���[�̃_���[�W�T�E���h")] AudioClip m_damageSound;
    [SerializeField, Tooltip("Animator�̊i�[�ϐ�")] Animator m_animator;
    [Tooltip("�I�u�W�F�N�g�̌��݂�HP")] protected int m_hp;
    [Tooltip("�I�u�W�F�N�g�̐�������")] protected bool m_living = true;

    public void Start()
    {
        m_hp = m_maxHP;
        IsStart();
    }

    void Update()
    {
        if(m_living)
        {
            IsUpdate();
        }
        else
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            m_animator.SetBool("Death", true);
        }
    }
    public virtual void IsStart()
    {
        //Start�֐��ŌĂт��������͂��̊֐���
    }
    public virtual void IsUpdate()
    {
        //Update�֐��ŌĂт��������͂��̊֐���
    }
    //�_���[�W�v�Z
    public void DamageMethod(int weaponPower, int firePower, int elekePower,int frozenPower)
    {
        var damage = weaponPower * m_rigitDefensePercentage
            + firePower * m_fireDifansePercentage
            + elekePower * m_elekeDifansePercentage
            + frozenPower * m_frozenDifansePercentage;
        m_hp -= (int)damage;
        //Debug.Log(m_hp);
        //��������
        if (m_hp <= 0)
        {
            m_living = false;
            Debug.Log(m_living);
        }
        else return;
    }
    
    
    public void HPPuls(int hpPuls)
    {
        m_hp += hpPuls;
    }
}
