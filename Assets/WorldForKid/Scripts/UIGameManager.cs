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
using UnityEngine.SceneManagement;

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
        btnNormalAttack.onClick.AddListener(NormalAttack);
        btnUseSkill.onClick.AddListener(Skill);

        btnShowJoinRoom.onClick.AddListener(ShowJoinRoom);
        btnBack.onClick.AddListener(Back);
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

        portCreateRoom.onValueChanged.AddListener(PortCreateRoomChange);
        portJoinRoom.onValueChanged.AddListener(PortJoinRoomChange);
        timer.OnTimeOut = () => { CheckResult(true); };

        NetworkManager.Singleton.OnServerStopped += OnServerStopped;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    private void OnClientDisconnectCallback(ulong obj)
    {
        if (obj == 0)
        {
            Notice.Instance.Show("Thông báo", "Phòng đã bị đóng", "", "Đóng", null,
                () => { btnBack.onClick.Invoke(); }, true);
        }
    }

    private void OnServerStopped(bool b)
    {
        Notice.Instance.Show("Thông báo", "Phòng đã bị đóng", "", "Đóng", null, null, true);
    }

    private void PortCreateRoomChange(string value)
    {
        if (value.StartsWith("-"))
        {
            value = value.Remove(0);
        }

        if (value.Length > 4)
        {
            value = value.Remove(value.Length - 1);
        }

        portCreateRoom.text = value;
    }

    private void PortJoinRoomChange(string value)
    {
        if (value.StartsWith("-"))
        {
            value = value.Remove(0);
        }

        if (value.Length > 4)
        {
            value = value.Remove(value.Length - 1);
        }

        portJoinRoom.text = value;
    }

    public void CheckResult(bool forceEndGame = false)
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
        {
            return;
        }

        int aliveGreen = 0;
        int aliveRed = 0;

        foreach (var player in GameManager.Instance.DictPlayerInGameData)
        {
            if (player.Value.Heath > 0)
            {
                if (player.Value.IsRedTem)
                {
                    aliveRed++;
                }
                else
                {
                    aliveGreen++;
                }
            }
        }

        if (forceEndGame || aliveGreen == 0 || aliveRed == 0)
        {

            if (aliveRed == aliveGreen)
            {
                GameManager.Instance.result = 0;
            }
            else if (aliveRed > aliveGreen)
            {
                GameManager.Instance.result = 1;
            }
            else
            {
                GameManager.Instance.result = -1;
            }

            GameManager.Instance.gameState = GameState.End;
            GameManager.Instance.EndGameClientRpc(GameManager.Instance.result);
        }
    }

    public void Skill()
    {
        if (GameManager.Instance.myPlayer.InGameData.Heath <= 0)
        {
            Notice.Instance.Show("Thông báo", "Bạn không có quyền sử dụng kỹ năng vì đã bị gạ gục", "", "Đóng",
                null,
                null, true);
            return;
        }

        if (GameManager.Instance.myPlayer.InGameData.State != PlayerState.ChoosingTarget)
        {
            Notice.Instance.Show("Thông báo", "Bạn hãy trả lời hỏi để có quyền sử dụng kỹ năng", "", "Đóng", null,
                null,
                true);
            return;
        }

        if (!GameManager.Instance.myPlayer.CanUseSkill())
        {
            Notice.Instance.Show("Thông báo", "Bạn không đủ năng lượng để sử dụng kỹ năng", "", "Đóng", null, null,
                true);
            return;
        }

        if (PlayerObject.LastSelected == null)
        {
            Notice.Instance.Show("Thông báo", "Bạn chưa chọn mục tiêu", "", "Đóng", null, null, true);
            return;
        }

        if (GameManager.Instance.myPlayer.GetInGamePlayerObject().MyCharacterData.SkillType ==
            SkillType.Teammate)
        {
            if (PlayerObject.LastSelected.PlayerInGameData.IsRedTem !=
                GameManager.Instance.myPlayer.InGameData.IsRedTem)
            {
                Notice.Instance.Show("Thông báo", "Kỹ năng bạn sở hữu chỉ có tác dụng lên bản thân hoặc đồng đội",
                    "", "Đóng", null,
                    null, true);
                return;
            }
        }
        else
        {
            if (PlayerObject.LastSelected.PlayerInGameData.IsRedTem ==
                GameManager.Instance.myPlayer.InGameData.IsRedTem)
            {
                Notice.Instance.Show("Thông báo", "Kỹ năng bạn sở hữu chỉ có tác dụng lên đối thủ", "", "Đóng",
                    null,
                    null, true);
                return;
            }
        }

        if (PlayerObject.LastSelected.PlayerInGameData.Heath <= 0)
        {
            if (GameManager.Instance.myPlayer.GetInGamePlayerObject().MyCharacterData.Id != 7)
            {
                Notice.Instance.Show("Thông báo", "Kỹ năng bạn sở hữu không có tác dụng lên mục tiêu đã bị gạ gục",
                    "",
                    "Đóng", null, null, true);
                return;
            }
        }
        else
        {
            if (GameManager.Instance.myPlayer.GetInGamePlayerObject().MyCharacterData.Id == 7)
            {
                Notice.Instance.Show("Thông báo", "Kỹ năng bạn sở hữu chỉ có tác dụng lên mục tiêu đã bị gạ gục",
                    "",
                    "Đóng", null, null, true);
                return;
            }
        }

        GameManager.Instance.myPlayer.DoActionServerRPC(PlayerObject.LastSelected.PlayerInGameData.UserName,
            ActionType.UseSkill);
    }

    public void NormalAttack()
    {
        if (GameManager.Instance.myPlayer.InGameData.Heath <= 0)
        {
            Notice.Instance.Show("Thông báo", "Bạn không có quyền tấn công vì đã bị gạ gục", "", "Đóng", null, null,
                true);
            return;
        }

        if (GameManager.Instance.myPlayer.InGameData.State != PlayerState.ChoosingTarget)
        {
            Notice.Instance.Show("Thông báo", "Bạn hãy trả lời câu hỏi để có quyền tấn công", "", "Đóng", null,
                null, true);
            return;
        }

        if (PlayerObject.LastSelected == null)
        {
            Notice.Instance.Show("Thông báo", "Bạn chưa chọn mục tiêu", "", "Đóng", null, null, true);
            return;
        }

        if (PlayerObject.LastSelected.PlayerInGameData.IsRedTem ==
            GameManager.Instance.myPlayer.InGameData.IsRedTem)
        {
            Notice.Instance.Show("Thông báo", "Không thể sử dụng đánh thường lên bản thân hoặc đồng đội", "",
                "Đóng", null, null,
                true);
            return;
        }

        if (PlayerObject.LastSelected.IsDead())
        {
            Notice.Instance.Show("Thông báo", "Đối thủ đã bị gạ gục, hãy chọn mục tiêu khác", "", "Đóng", null,
                null,
                true);
            return;
        }

        GameManager.Instance.myPlayer.DoActionServerRPC(PlayerObject.LastSelected.PlayerInGameData.UserName,
            ActionType.AttackNormal);
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
    private void Back()
    {
        SceneManager.LoadSceneAsync("ChosseCharacter", LoadSceneMode.Single);
        if (NetworkManager.Singleton != null)
        {
            try
            {
                if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }

            Destroy(NetworkManager.Singleton.gameObject);
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
