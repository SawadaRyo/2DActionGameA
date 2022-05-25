using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

 public class WeaponBase : MonoBehaviour
{
    [SerializeField] protected int m_weaponPower = 5;
    [SerializeField] protected int m_elekePower = 0;
    [SerializeField] protected int m_firePower = 0;
    [SerializeField] protected int m_frozenPower = 0;
    [SerializeField] protected LayerMask enemyLayer = ~0;
    public int WeaponPower { get => m_weaponPower; set => m_weaponPower = value; }

    public virtual void IsStart()
    {
        //Start�֐��ŌĂт��������͂��̊֐���
    }
    public virtual void IsUpdate()
    {
        //Update�֐��ŌĂт��������͂��̊֐���
    }
    void Start()
    {
        IsStart();
    }
    void Update()
    {
        IsUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "TargetObject")
        {
            var enemyHP = other.gameObject.GetComponent<GaugeManager>();
            if (enemyHP)
            {
                enemyHP.DamageMethod(m_weaponPower, m_firePower, m_elekePower, m_frozenPower);
            }
            else return;
        }
    }
}
