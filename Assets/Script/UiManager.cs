using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script
{
    public class UiManager : MonoBehaviour
    {
        private bool _isRedTeam;

        [SerializeField] private Button btnStartGame;
        [SerializeField] private GameObject win;
        [SerializeField] private GameObject lose;
        [SerializeField] private GameObject draw;
        [SerializeField] private Button btnBack;
        [SerializeField] private Button btnCreateRoom;
        [SerializeField] private Button btnJoinRoom;
        [SerializeField] private GameObject popupCreateRoom;
        [SerializeField] private GameObject popupJoinRoom;
        [SerializeField] private Button btnShowCreateRoom;
        [SerializeField] private Button btnShowJoinRoom;
        [SerializeField] private TMP_InputField ipCreateRoom;
        [SerializeField] private TMP_InputField portCreateRoom;
        [SerializeField] private TMP_InputField ipJoinRoom;
        [SerializeField] private TMP_InputField portJoinRoom;
        [SerializeField] private Toggle tgGreenCreate;
        [SerializeField] private Toggle tgRedCreate;
        [SerializeField] private Toggle tgGreenJoin;
        [SerializeField] private Toggle tgRedJoin;
        [SerializeField] private TMP_Text roomInfo;
        [SerializeField] private Button btnNormalAttack;
        [SerializeField] private Button btnUseSkill;
        [SerializeField] private GameObject attackButtons;

        public MyNetWorkTimer timer;
        public QuestionPanel questionPanel;

        void Start()
        {
            btnNormalAttack.onClick.AddListener(NormalAttack);
            btnUseSkill.onClick.AddListener(Skill);

            btnStartGame.onClick.AddListener(StartGame);
            btnBack.onClick.AddListener(Back);

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


        public void NormalAttack()
        {
            if (InGameManager.Instance.myPlayer.InGameData.Heath <= 0)
            {
                Notice.Instance.Show("Thông báo", "Bạn không có quyền tấn công vì đã bị gạ gục", "", "Đóng", null, null,
                    true);
                return;
            }

            if (InGameManager.Instance.myPlayer.InGameData.State != PlayerState.ChoosingTarget)
            {
                Notice.Instance.Show("Thông báo", "Bạn hãy trả lời câu hỏi để có quyền tấn công", "", "Đóng", null,
                    null, true);
                return;
            }

            if (InGamePlayerObject.LastSelected == null)
            {
                Notice.Instance.Show("Thông báo", "Bạn chưa chọn mục tiêu", "", "Đóng", null, null, true);
                return;
            }

            if (InGamePlayerObject.LastSelected.PlayerInGameData.IsRedTem ==
                InGameManager.Instance.myPlayer.InGameData.IsRedTem)
            {
                Notice.Instance.Show("Thông báo", "Không thể sử dụng đánh thường lên bản thân hoặc đồng đội", "",
                    "Đóng", null, null,
                    true);
                return;
            }

            if (InGamePlayerObject.LastSelected.IsDead())
            {
                Notice.Instance.Show("Thông báo", "Đối thủ đã bị gạ gục, hãy chọn mục tiêu khác", "", "Đóng", null,
                    null,
                    true);
                return;
            }

            InGameManager.Instance.myPlayer.DoActionServerRPC(InGamePlayerObject.LastSelected.PlayerInGameData.UserName,
                ActionType.AttackNormal);
        }
        
        
        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.A))
        //     {
        //         InGameManager.Instance.myPlayer.DoActionServerRPC(InGamePlayerObject.LastSelected.PlayerInGameData.UserName,
        //             ActionType.UseSkill);
        //     }
        //     
        //     if (Input.GetKeyDown(KeyCode.B))
        //     {
        //         InGameManager.Instance.myPlayer.DoActionServerRPC(InGamePlayerObject.LastSelected.PlayerInGameData.UserName,
        //             ActionType.AttackNormal);
        //     }
        // }

        public void Skill()
        {
            if (InGameManager.Instance.myPlayer.InGameData.Heath <= 0)
            {
                Notice.Instance.Show("Thông báo", "Bạn không có quyền sử dụng kỹ năng vì đã bị gạ gục", "", "Đóng",
                    null,
                    null, true);
                return;
            }

            if (InGameManager.Instance.myPlayer.InGameData.State != PlayerState.ChoosingTarget)
            {
                Notice.Instance.Show("Thông báo", "Bạn hãy trả lời hỏi để có quyền sử dụng kỹ năng", "", "Đóng", null,
                    null,
                    true);
                return;
            }

            if (!InGameManager.Instance.myPlayer.CanUseSkill())
            {
                Notice.Instance.Show("Thông báo", "Bạn không đủ năng lượng để sử dụng kỹ năng", "", "Đóng", null, null,
                    true);
                return;
            }

            if (InGamePlayerObject.LastSelected == null)
            {
                Notice.Instance.Show("Thông báo", "Bạn chưa chọn mục tiêu", "", "Đóng", null, null, true);
                return;
            }

            if (InGameManager.Instance.myPlayer.GetInGamePlayerObject().MyCharacterData.SkillType ==
                SkillType.Teammate)
            {
                if (InGamePlayerObject.LastSelected.PlayerInGameData.IsRedTem !=
                    InGameManager.Instance.myPlayer.InGameData.IsRedTem)
                {
                    Notice.Instance.Show("Thông báo", "Kỹ năng bạn sở hữu chỉ có tác dụng lên bản thân hoặc đồng đội",
                        "", "Đóng", null,
                        null, true);
                    return;
                }
            }
            else
            {
                if (InGamePlayerObject.LastSelected.PlayerInGameData.IsRedTem ==
                    InGameManager.Instance.myPlayer.InGameData.IsRedTem)
                {
                    Notice.Instance.Show("Thông báo", "Kỹ năng bạn sở hữu chỉ có tác dụng lên đối thủ", "", "Đóng",
                        null,
                        null, true);
                    return;
                }
            }

            if (InGamePlayerObject.LastSelected.PlayerInGameData.Heath <= 0)
            {
                if (InGameManager.Instance.myPlayer.GetInGamePlayerObject().MyCharacterData.Id != 7)
                {
                    Notice.Instance.Show("Thông báo", "Kỹ năng bạn sở hữu không có tác dụng lên mục tiêu đã bị gạ gục",
                        "",
                        "Đóng", null, null, true);
                    return;
                }
            }
            else
            {
                if (InGameManager.Instance.myPlayer.GetInGamePlayerObject().MyCharacterData.Id == 7)
                {
                    Notice.Instance.Show("Thông báo", "Kỹ năng bạn sở hữu chỉ có tác dụng lên mục tiêu đã bị gạ gục",
                        "",
                        "Đóng", null, null, true);
                    return;
                }
            }

            InGameManager.Instance.myPlayer.DoActionServerRPC(InGamePlayerObject.LastSelected.PlayerInGameData.UserName,
                ActionType.UseSkill);
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

        private void Back()
        {
            SceneManager.LoadSceneAsync("Profile", LoadSceneMode.Single);
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

        public AuthenRPCData GetAuthenData()
        {
            return new AuthenRPCData
            {
                UserName = SystemInfo.deviceUniqueIdentifier + Config.Instance.MyPlayerData.DisplayName,
                IsRedTem = _isRedTeam,
                IndexCharacter = Config.Instance.MyPlayerData.IndexCharacter,
                DisplayName = Config.Instance.MyPlayerData.DisplayName,
            };
        }

        private void StartGame()
        {
            btnStartGame.gameObject.SetActive(false);
            InGameManager.Instance.StartGameClientRpc();
            InGameManager.Instance.timeStart.Value = NetworkManager.Singleton.ServerTime.TimeAsFloat;
            InGameManager.Instance.gameState = GameState.PLaying;
            foreach (var playerUserName in InGameManager.Instance.DictPlayerInGame.Keys.ToList())
            {
                InGameManager.Instance.RandomQuestionForNewPlayer(playerUserName);
            }

            foreach (var player in InGameManager.Instance.DictPlayerInGame.Values.ToList())
            {
                player.SendCurrentQuestionToClient();
            }
        }

        public void ShowButtonInteractableButtons()
        {
            attackButtons.gameObject.SetActive(true);
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
                btnStartGame.gameObject.SetActive(true);
            }
        }

        public void CheckResult(bool forceEndGame = false)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
            {
                return;
            }

            int aliveGreen = 0;
            int aliveRed = 0;

            foreach (var player in InGameManager.Instance.DictPlayerInGameData)
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
                    InGameManager.Instance.result = 0;
                }
                else if (aliveRed > aliveGreen)
                {
                    InGameManager.Instance.result = 1;
                }
                else
                {
                    InGameManager.Instance.result = -1;
                }
                
                InGameManager.Instance.gameState = GameState.End;
                InGameManager.Instance.EndGameClientRpc(InGameManager.Instance.result);
            }
        }

        public void EndGame(int result)
        {
            DOVirtual.DelayedCall(1.5f, () =>
            {
                InGameManager.Instance.gameState = GameState.End;
                var isRedTeam = InGameManager.Instance.myPlayer.InGameData.IsRedTem;

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
    }
}