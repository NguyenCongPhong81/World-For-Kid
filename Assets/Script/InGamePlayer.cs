using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Script
{
    public class InGamePlayer : NetworkBehaviour
    {
        private string _userName;
        public string UserName
        {
            get
            {
                if (!String.IsNullOrEmpty(_userName)) return _userName;

                if (!GameManager.Instance.MapClientIdToUserNam.ContainsKey(OwnerClientId))
                {
                    Debug.Log("OwnerClientId null " + OwnerClientId);
                    return null;
                }

                _userName = GameManager.Instance.MapClientIdToUserNam[OwnerClientId];
                return _userName;
            }
        }

        public PlayerInGameData InGameData
        {
            get
            {
                if (string.IsNullOrEmpty(UserName))
                {
                    return null;
                }
                if (!GameManager.Instance.DictPlayerInGameData.ContainsKey(UserName))
                {
                    Debug.Log("Userdata not found " + UserName);
                    return null;
                }

                return GameManager.Instance.DictPlayerInGameData[UserName];
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public override void OnNetworkSpawn()
        {
            Notice.Instance.Hide();
            Debug.Log("OnNetworkSpawn" + OwnerClientId);
            if (IsOwner)
            {
                GameManager.Instance.myPlayer = this;
                AuthenServerRpc(GameManager.Instance.ui.GetAuthenData());
            }
        }

        public override void OnNetworkDespawn()
        {
            if (InGameData == null || InGameData.UserName == null || IsOwner)
            {
                return;
            }

            if (GameManager.Instance.DictPlayerInGame.ContainsKey(InGameData.UserName))
            {
                GameManager.Instance.DictPlayerInGame.Remove(InGameData.UserName);
            }

            if (GameManager.Instance.gameState == GameState.PLaying) return;
            var playerObj = GetInGamePlayerObject();
            if (playerObj != null)
            {
                playerObj.gameObject.SetActive(false);
                //playerObj.Reset();
            }

            GameManager.Instance.ResetIndex(InGameData.IsRedTem, InGameData.IndexPosition);
            if (GameManager.Instance.DictPlayerInGameData.ContainsKey(InGameData.UserName))
            {
                GameManager.Instance.DictPlayerInGameData.Remove(InGameData.UserName);
            }

            if (GameManager.Instance.MapClientIdToUserNam.ContainsKey(OwnerClientId))
            {
                GameManager.Instance.MapClientIdToUserNam.Remove(OwnerClientId);
            }
        }

        [ServerRpc]
        private void AuthenServerRpc(AuthenRPCData authenRPCData)
        {
            Debug.Log("AuthenServerRpc" + JsonConvert.SerializeObject(authenRPCData));
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { (ulong)OwnerClientId }
                }
            };

            var userName = authenRPCData.UserName.ToString();

            if (!GameManager.Instance.DictPlayerInGameData.ContainsKey(userName))
            {
                if (GameManager.Instance.gameState != GameState.WaitingStart)
                {
                    SendErrorClientRpc(ErrorCode.JoinLate, clientRpcParams);
                    return;
                }
                
                var index = GameManager.Instance.GetNextIndex(authenRPCData.IsRedTem);

                if (index < 0)
                {
                    SendErrorClientRpc(ErrorCode.FullTeam, clientRpcParams);
                    return;
                }

                GameManager.Instance.DictPlayerInGameData[userName] = new PlayerInGameData
                {
                    UserName = authenRPCData.UserName.ToString(),
                    DisplayName = authenRPCData.DisplayName.ToString(),
                    CharacterType = authenRPCData.IndexCharacter,
                    IsRedTem = authenRPCData.IsRedTem,
                    IndexPosition = index,
                    State = PlayerState.ChoosingAnswer,
                    LastAnswerIdSelected = -1,
                    Heath = GameConfig.Instance.CharacterDatas[authenRPCData.IndexCharacter].Health,
                };
            }

            GameManager.Instance.MapClientIdToUserNam[OwnerClientId] = userName;

            GameManager.Instance.DictPlayerInGame[userName] = this;

            List<PlayerInGameRPCData> listOtherPlayerInGameDataRpc = new List<PlayerInGameRPCData>();

            foreach (var player in GameManager.Instance.DictPlayerInGameData.Values.ToList())
            {
                listOtherPlayerInGameDataRpc.Add(player.ConvertToRPC());
            }

            List<MapClientIdWithUserName> mapClientIdWithUserNames = new List<MapClientIdWithUserName>();

            foreach (var mapClientIdWithUserName in GameManager.Instance.MapClientIdToUserNam)
            {
                mapClientIdWithUserNames.Add(new MapClientIdWithUserName
                {
                    ClientId = mapClientIdWithUserName.Key,
                    UserName = mapClientIdWithUserName.Value
                });
            }

            SendInGameDataClientRpc(InGameData.ConvertToRPC());

            SendCurrentStateClientRpc(listOtherPlayerInGameDataRpc.ToArray(), mapClientIdWithUserNames.ToArray(),
                clientRpcParams);

            if (GameManager.Instance.gameState >= GameState.PLaying)
            {
                GameManager.Instance.StartGameClientRpc(clientRpcParams);

                if (InGameData.LastAnswerIdSelected == -1)
                {
                    SendCurrentQuestionToClient();
                }
                else
                {
                    QuestionDataRPC questionDataRPC =
                        InGameManager.Instance.GetQuestion(InGameData.UserName, InGameData.CurrentQuestion);
                    SendCurrentQuestionWithChoseIdClientRpc(questionDataRPC, InGameData.LastAnswerIdSelected,
                        clientRpcParams);
                }
            }

            if (GameManager.Instance.gameState == GameState.End)
            {
                GameManager.Instance.EndGameClientRpc(GameManager.Instance.result);
            }
        }

        [ClientRpc]
        private void SendErrorClientRpc(ErrorCode errorCode, ClientRpcParams clientRpcParams = default)
        {
            switch (errorCode)
            {
                case ErrorCode.FullTeam:
                    Notice.Instance.Show("Đội đã đầy", "Vui lòng chọn đội khác hoặc tham gia trận đấu khác", "", "Đóng",
                        null,
                        () =>
                        {
                            try
                            {
                                var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
                                if (utp != null) utp.Shutdown();
                                if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
                            }
                            catch (Exception e)
                            {
                                Debug.Log(e.ToString());
                            }

                            GameManager.Instance.ui.ShowJoinRoom();
                        }, true);
                    break;
                case ErrorCode.JoinLate:
                    Notice.Instance.Show("Trận đấu đã bắt đầu", "Vui lòng tham gia vào trận đấu khác", "", "Đóng",
                        null,
                        () =>
                        {
                            try
                            {
                                var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
                                if (utp != null) utp.Shutdown();
                                if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
                            }
                            catch (Exception e)
                            {
                                Debug.Log(e.ToString());
                            }

                            GameManager.Instance.ui.ShowJoinRoom();
                        }, true);
                    break;
            }
        }

        [ClientRpc]
        private void SendCurrentStateClientRpc(PlayerInGameRPCData[] otherPlayerData,
            MapClientIdWithUserName[] mapClientIdWithUserNames,
            ClientRpcParams clientRpcParams = default)
        {
            
            foreach (var data in mapClientIdWithUserNames)
            {
                GameManager.Instance.MapClientIdToUserNam[data.ClientId] = data.UserName.ToString();
            }
            
            foreach (var rpcData in otherPlayerData)
            {
                var playerData = rpcData.ConvertToNormal();
                GameManager.Instance.DictPlayerInGameData[playerData.UserName] = playerData;

                var playerObject = playerData.IsRedTem
                    ? GameManager.Instance.redTeamMember[playerData.IndexPosition]
                    : GameManager.Instance.greenTeamMember[playerData.IndexPosition];

                playerObject.gameObject.SetActive(true);
                playerObject.Init(playerData);
            }
        }

        [ClientRpc]
        private void SendInGameDataClientRpc(PlayerInGameRPCData playerInGameRPCData)
        {
            if (IsOwner) return;
            var playerData = playerInGameRPCData.ConvertToNormal();
            GameManager.Instance.MapClientIdToUserNam[OwnerClientId] = playerData.UserName;

            if (!GameManager.Instance.DictPlayerInGameData.ContainsKey(playerData.UserName))
            {
                GameManager.Instance.DictPlayerInGameData[playerData.UserName] = playerData;
            }


            var playerObject = playerData.IsRedTem
                ? GameManager.Instance.redTeamMember[playerData.IndexPosition]
                : GameManager.Instance.greenTeamMember[playerData.IndexPosition];

            if (!playerObject.gameObject.activeInHierarchy)
            {
                playerObject.gameObject.SetActive(true);
                playerObject.Init(GameManager.Instance.DictPlayerInGameData[playerData.UserName]);
            }
        }

        private void UpdateEnemy()
        {
            //GetInGamePlayerObject().ChangeEnergy(10);
        }

        [ClientRpc]
        private void SendQuestionClientRpc(QuestionDataRPC questionDataRPC,int CurrentQuestion,
            ClientRpcParams clientRpcParams = default)
        {
            InGameData.State = PlayerState.ChoosingAnswer;
            InGameData.CurrentQuestion = CurrentQuestion;

            InGameManager.Instance.ui.questionPanel.ShowQuestion(questionDataRPC);
        }

        [ClientRpc]
        private void SendCurrentQuestionWithChoseIdClientRpc(QuestionDataRPC questionDataRPC, int chooseId,
            ClientRpcParams clientRpcParams = default)
        {
            InGameManager.Instance.ui.questionPanel.ShowQuestion(questionDataRPC);

            InGameManager.Instance.ui.questionPanel.ShowResult(questionDataRPC.CorrectId,chooseId);
           // GetInGamePlayerObject().ShowEffectWeapon();
        }

        [ClientRpc]
        private void SendChooseClientRpc(int chooseId, int correctAnswerId)
        {
            InGameData.State = PlayerState.ChoosingTarget;
            InGameData.LastAnswerIdSelected = chooseId;

            Debug.Log("Hoan SendChooseClientRpc" + InGameData.LastAnswerIdSelected);

            if (chooseId == correctAnswerId)
            {
                if (InGameData.CharacterType == 4) InGameData.BonusAttackNormal++;
                if (InGameData.BonusAttackNormal >= 6)
                {
                    InGameData.BonusAttackNormal = 6;
                }

                UpdateEnemy();
            }
            else
            {
                if (InGameData.CharacterType == 4)
                {
                    InGameData.BonusAttackNormal--;
                    if (InGameData.BonusAttackNormal < 0)
                        InGameData.BonusAttackNormal = 0;
                }
            }

            //GetInGamePlayerObject().ShowEffectWeapon();

            if (!InGameData.UserName.Equals(InGameManager.Instance.myPlayer.InGameData.UserName))
            {
                return;
            }

            if (IsOwner)
            {
                InGameManager.Instance.ui.questionPanel.ShowResult(correctAnswerId,chooseId);
            }
        }

        [ServerRpc]
        public void SendChooseServerRpc(int questionId, int chooseId)
        {
            InGameData.LastAnswerIdSelected = chooseId;

            Debug.Log("Hoan SendChooseServerRpc" + InGameData.LastAnswerIdSelected);

            int correctAnswerId = InGameManager.Instance.ListQuestionData[questionId].CorrectAnswerId;
            InGameData.State = PlayerState.ChoosingTarget;
            SendChooseClientRpc(chooseId, correctAnswerId);
        }


        [ServerRpc]
        public void DoActionServerRPC(string targetUserName, ActionType actionType)
        {
            if (InGameData.State != PlayerState.ChoosingTarget)
            {
                return;
            }

            if (actionType == ActionType.UseSkill && !CanUseSkill())
            {
                Debug.Log("Not enough energy ServerRPC");
                return;
            }

            if (InGameData.CharacterType == 5 && InGameManager.Instance.DictPlayerInGameData[targetUserName].HaveShield)
            {
                return;
            }

            InGameData.State = PlayerState.ChoosingAnswer;
            InGameData.CurrentQuestion = InGameData.CurrentQuestion + 1 - InGameData.TimeEffectMinusIndexQuestion;

            if (InGameData.CurrentQuestion < 0)
            {
                InGameData.TimeEffectMinusIndexQuestion = -InGameData.CurrentQuestion;
                InGameData.CurrentQuestion = 0;
            }
            else
            {
                InGameData.TimeEffectMinusIndexQuestion = 0;
            }
            
            InGameData.LastAnswerIdSelected = -1;
            Debug.Log("Hoan DoActionServerRPC" + InGameData.LastAnswerIdSelected);
            SendCurrentQuestionToClient();

            if (InGameData.CharacterType == 0 && actionType == ActionType.UseSkill)
            {
                var targetPlayerData = InGameManager.Instance.DictPlayerInGameData[targetUserName];

                if (targetPlayerData.State == PlayerState.ChoosingAnswer && targetPlayerData.CurrentQuestion > 0)
                {
                    targetPlayerData.CurrentQuestion--;
                    if (InGameManager.Instance.DictPlayerInGame.ContainsKey(targetUserName))
                    {
                        InGameManager.Instance.DictPlayerInGame[targetUserName].SendCurrentQuestionToClient();
                    }
                }
                else
                {
                    targetPlayerData.TimeEffectMinusIndexQuestion++;
                }
            }

            DoActionClientRPC(targetUserName, actionType);
        }

        public void SendCurrentQuestionToClient()
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { OwnerClientId }
                }
            };
            var questionRpc = InGameManager.Instance.GetQuestion(InGameData.UserName, InGameData.CurrentQuestion);
            questionRpc.CorrectId = -1; // not send correct id to client
            InGameData.State = PlayerState.ChoosingAnswer;
            SendQuestionClientRpc(questionRpc,InGameData.CurrentQuestion, clientRpcParams);
        }

        [ClientRpc]
        public void DoActionClientRPC(string targetUserName, ActionType actionType)
        {
            if (actionType == ActionType.UseSkill)
            {
                //GetInGamePlayerObject().UseSkill(targetUserName);
            }
            else
            {
                GetInGamePlayerObject().UseAttackNormal(targetUserName);
            }
        }

        public bool CanUseSkill()
        {
            return InGameData.Energy >= GetInGamePlayerObject().MyCharacterData.Energy;
        }

        public PlayerObject GetInGamePlayerObject()
        {
            PlayerObject playerObject =
                GameManager.Instance.GetPlayerObject(InGameData.IsRedTem, InGameData.IndexPosition);
            return playerObject;
        }
    }
}