using Script;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameManager : MonoBehaviour
{
    [Header("GO")]
    [SerializeField] private GameObject win;
    [SerializeField] private GameObject lose;
    [SerializeField] private GameObject draw;
    [SerializeField] private GameObject attackButtons;
    [SerializeField] private GameObject popupCreateRoom;
    [SerializeField] private GameObject popupJoinRoom;

    [Header("Button")]
    [SerializeField] private Button btnStartGame;
    [SerializeField] private Button btnBack;
    [SerializeField] private Button btnCreateRoom;
    [SerializeField] private Button btnJoinRoom;
    [SerializeField] private Button btnShowCreateRoom;
    [SerializeField] private Button btnShowJoinRoom;
    [SerializeField] private Button btnNormalAttack;
    [SerializeField] private Button btnUseSkill;

    [Header("IPF")]
    [SerializeField] private TMP_InputField ipCreateRoom;
    [SerializeField] private TMP_InputField portCreateRoom;
    [SerializeField] private TMP_InputField ipJoinRoom;
    [SerializeField] private TMP_InputField portJoinRoom;
    [SerializeField] private TMP_Text roomInfo;

    [Header("Tg")]
    [SerializeField] private Toggle tgGreenCreate;
    [SerializeField] private Toggle tgRedCreate;
    [SerializeField] private Toggle tgGreenJoin;
    [SerializeField] private Toggle tgRedJoin;

    private bool _isRedTeam;

    public MyNetWorkTimer timer;
    public QuestionPanel questionPanel;

    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
