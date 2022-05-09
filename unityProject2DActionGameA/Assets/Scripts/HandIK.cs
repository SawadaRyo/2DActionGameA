using UnityEngine;

/// <summary>
/// ��� IK �𐧌䂷��B
/// </summary>
[RequireComponent(typeof(Animator))]
public class HandIK : MonoBehaviour
{
    /// <summary>�E��̃^�[�Q�b�g</summary>
    [SerializeField] Transform _rightTarget = default;
    /// <summary>����̃^�[�Q�b�g</summary>
    [SerializeField] Transform _leftTarget = default;
    /// <summary>�E��� Position �ɑ΂���E�F�C�g</summary>
    [SerializeField, Range(0f, 1f)] float _rightPositionWeight = 0;
    /// <summary>�E��� Rotation �ɑ΂���E�F�C�g</summary>
    [SerializeField, Range(0f, 1f)] float _rightRotationWeight = 0;
    /// <summary>����� Position �ɑ΂���E�F�C�g</summary>
    [SerializeField, Range(0f, 1f)] float _leftPositionWeight = 0;
    /// <summary>����� Rotation �ɑ΂���E�F�C�g</summary>
    [SerializeField, Range(0f, 1f)] float _leftRotationWeight = 0;
    Animator m_animator = default;

    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        // �E��ɑ΂��� IK ��ݒ肷��
        m_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _rightPositionWeight);
        m_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _rightRotationWeight);
        m_animator.SetIKPosition(AvatarIKGoal.RightHand, _rightTarget.position);
        m_animator.SetIKRotation(AvatarIKGoal.RightHand, _rightTarget.rotation);
        // ����ɑ΂��� IK ��ݒ肷��
        m_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _leftPositionWeight);
        m_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _leftRotationWeight);
        m_animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftTarget.position);
        m_animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftTarget.rotation);
    }
}
