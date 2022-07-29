using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerAnimationState :SingletonBehaviour<PlayerAnimationState>
{
    [Tooltip("�ړ��\�����肷��ϐ�")]
    bool m_ableMove = true;
    [Tooltip("")]
    bool m_ableInput = true;
    [Tooltip("Animation�̑J�ڏ�")]
    ObservableStateMachineTrigger m_trigger = default;
    [Tooltip("Player��Animator���i�[����ϐ�")]
    Animator m_animator = default;
    [Tooltip("WeaponEquipment���i�[����ϐ�")]
    WeaponEquipment m_weaponEquipment;

    public bool AbleMove => m_ableMove;
    public bool AbleInput => m_ableInput;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_trigger = m_animator.GetBehaviour<ObservableStateMachineTrigger>();
        m_weaponEquipment = WeaponEquipment.Instance;
        DetectionState();
    }
    void DetectionState()
    {
        IDisposable enterState = m_trigger
            .OnStateEnterAsObservable()
            .Subscribe(onStateInfo =>
            {
                AnimatorStateInfo info = onStateInfo.StateInfo;
                if (info.IsTag("Base Layer.Ground") || info.IsName("JumpEnd"))
                {
                    Debug.Log("Enter");
                    m_ableMove = false;
                    if (info.IsName($"{m_weaponEquipment.name}+AttackFinish"))
                    {
                        m_ableInput = false;
                    }
                }
            }).AddTo(this);

        IDisposable exitState = m_trigger
            .OnStateExitAsObservable()
            .Subscribe(onStateInfo =>
            {
                AnimatorStateInfo info = onStateInfo.StateInfo;
                if (info.IsTag("Base Layer.Ground") || info.IsName("JumpEnd"))
                {
                    Debug.Log("Exit");
                    m_ableMove = true;
                    if(!m_ableInput)
                    {
                        m_ableInput = true;
                    }
                }
            }).AddTo(this);

        IDisposable isState = m_trigger
            .OnStateUpdateAsObservable()
            .Subscribe(onStateInfo =>
            {
                AnimatorStateInfo info = onStateInfo.StateInfo;
                if (info.IsTag("Base Layer.Ground") || info.IsName("JumpEnd"))
                {
                    m_ableMove = true;
                    if (!m_ableInput)
                    {
                        m_ableInput = true;
                    }
                }
            }).AddTo(this);
    }
}