using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject m_menuPanel = default;
    [SerializeField] Button m_fastButton = default;
    bool m_menuSwitch = false;

    

    // Update is called once per frame
    void Update()
    {
        ManuHandler();
    }
    void ManuHandler()
    {
        if(Input.GetButtonDown("MenuSwitch"))
        {
            //ToDo ���j���[�̊J�ɃA�j���[�V������������
            m_menuSwitch = !m_menuSwitch;
            m_menuPanel.SetActive(m_menuSwitch);
            if(m_menuSwitch)
            {
                m_fastButton.Select();
            }
        }

    }
}
