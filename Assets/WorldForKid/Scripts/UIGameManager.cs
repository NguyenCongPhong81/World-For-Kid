using Script;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using DG.Tweening;

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
        btnShowJoinRoom.onClick.AddListener(ShowJoinRoom);

        btnShowCreateRoom.onClick.AddListener(ShowCreateRoom);

        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                ipCreateRoom.text = ip.ToString();
            }
        }

        btnJoinRoom.onClick.AddListener(JoinRoom);
        btnCreateRoom.onClick.AddListener(CreateRoom);

        tgGreenCreate.onValueChanged.AddListener(OnTgGreenCreateChange);
        tgRedCreate.onValueChanged.AddListener(OnTgRedCreateChange);

        tgGreenJoin.onValueChanged.AddListener(OnTgGreenJoinChange);
        tgRedJoin.onValueChanged.AddListener(OnTgRedJoinChange);

    }

    public void ShowUiWaitStartGame()
    {
        var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        roomInfo.transform.parent.gameObject.SetActive(true);
        roomInfo.text = utp.ConnectionData.Address + ":" + utp.ConnectionData.Port;
        timer.gameObject.SetActive(true);
        timer.SetStartTime(Config.TIME_PLAY);
        popupCreateRoom.gameObject.SetActive(false);
        popupJoinRoom.gameObject.SetActive(false);
        LoadingManager.Instance.Hide();
        if (NetworkManager.Singleton.IsServer)
        {
            //btnStartGame.gameObject.SetActive(true);
        }
    }

    private void CreateRoom()
    {
        var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        utp.SetConnectionData(ipCreateRoom.text, ushort.Parse(portCreateRoom.text));
        _isRedTeam = tgRedCreate.isOn;
        LoadingManager.Instance.Show(10,
            () =>
            {
                Notice.Instance.Show("Tạo phòng thất bại", "Vui lòng chọn port khác", "", "Đóng", null,
                    ShowCreateRoom,
                    true);
                if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
            });
        NetworkManager.Singleton.StartHost();
    }

    private void JoinRoom()
    {
        var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        utp.SetConnectionData(ipJoinRoom.text, ushort.Parse(portJoinRoom.text));

        _isRedTeam = tgRedJoin.isOn;
        LoadingManager.Instance.Show(10, () =>
        {
            Notice.Instance.Show("Vào phòng thất bại", "Vui lòng điền đúng ip và port", "", "Đóng", null, () =>
            {
                try
                {
                    if (utp != null) utp.Shutdown();
                    if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }

                ShowJoinRoom();
            },
                true);
        });

        bool start = NetworkManager.Singleton.StartClient();


        if (!start)
        {
            LoadingManager.Instance.Hide();
            Notice.Instance.Show("Vào phòng thất bại", "Vui lòng điền đúng ip và port", "", "Đóng", null,
                ShowJoinRoom,
                true);
            try
            {
                if (utp != null) utp.Shutdown();
                if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }

    public void ShowCreateRoom()
    {
        roomInfo.transform.parent.gameObject.SetActive(false);
        btnStartGame.gameObject.SetActive(false);
        popupCreateRoom.SetActive(true);
        popupJoinRoom.SetActive(false);
        timer.gameObject.SetActive(false);
    }

    public void ShowJoinRoom()
    {
        roomInfo.transform.parent.gameObject.SetActive(false);
        btnStartGame.gameObject.SetActive(false);
        popupCreateRoom.SetActive(false);
        popupJoinRoom.SetActive(true);
        timer.gameObject.SetActive(false);
    }

    private void OnTgGreenCreateChange(bool isOn)
    {
        tgRedCreate.isOn = !isOn;
    }

    private void OnTgRedCreateChange(bool isOn)
    {
        tgGreenCreate.isOn = !isOn;
    }

    private void OnTgGreenJoinChange(bool isOn)
    {
        tgRedJoin.isOn = !isOn;
    }

    private void OnTgRedJoinChange(bool isOn)
    {
        tgGreenJoin.isOn = !isOn;
    }

    public void ShowButtonInteractableButtons()
    {
        attackButtons.gameObject.SetActive(true);
    }

    public void EndGame(int result)
    {
        DOVirtual.DelayedCall(1.5f, () =>
        {
            GameManager.Instance.gameState = GameState.End;
            var isRedTeam = GameManager.Instance.myPlayer.InGameData.IsRedTem;

            if (result == 0)
            {
                draw.SetActive(true);
                return;
            }

            if (result == 1)
            {
                win.SetActive(isRedTeam);
                lose.SetActive(!isRedTeam);
            }

            if (result == -1)
            {
                win.SetActive(!isRedTeam);
                lose.SetActive(isRedTeam);
            }
        });
    }

    public AuthenRPCData GetAuthenData()
    {
        return new AuthenRPCData
        {
            UserName = SystemInfo.deviceUniqueIdentifier + GameConfig.Instance.PlayerData.DisplayName,
            IsRedTem = _isRedTeam,
            IndexCharacter = GameConfig.Instance.PlayerData.IndexCharacter,
            DisplayName = GameConfig.Instance.PlayerData.DisplayName,
        };
    }

}
