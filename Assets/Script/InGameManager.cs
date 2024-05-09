using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

namespace Script
{
    public class InGameManager : NetworkBehaviour
    {
        private List<QuestionData> _listQuestionData;
        private Dictionary<string, List<int>> _questionForEachPlayer;
        private List<int> _freeIndexRedTeam;
        private List<int> _freeIndexGreenTeam;


        public UiManager ui;
        public List<InGamePlayerObject> redTeamMember;
        public List<InGamePlayerObject> greenTeamMember;
        public EffectManager effectManager;
        public NetworkVariable<float> timeStart = new NetworkVariable<float>();

        public Dictionary<string, InGamePlayer> DictPlayerInGame { get; private set; } =
            new Dictionary<string, InGamePlayer>();

        public Dictionary<string, PlayerInGameData> DictPlayerInGameData { get; private set; } =
            new Dictionary<string, PlayerInGameData>();

        public Dictionary<ulong, string> MapClientIdToUserNam { get; private set; } = new Dictionary<ulong, string>();

        [HideInInspector] public GameState gameState = GameState.WaitingStart;
        [HideInInspector] public int result;

        [HideInInspector] public InGamePlayer myPlayer;

        public List<QuestionData> ListQuestionData
        {
            get
            {
                if (_listQuestionData == null)
                {
                    LoadQuestion();
                }

                return _listQuestionData;
            }
        }

        public static InGameManager Instance;

        private void Awake()
        {
            LoadingManager.Instance.Hide();
            Instance = this;

            _freeIndexRedTeam = new List<int>();
            _freeIndexGreenTeam = new List<int>();
            _questionForEachPlayer = new Dictionary<string, List<int>>();

            for (int i = redTeamMember.Count - 1; i >= 0; --i)
            {
                _freeIndexRedTeam.Add(i);
            }

            for (int i = greenTeamMember.Count - 1; i >= 0; --i)
            {
                _freeIndexGreenTeam.Add(i);
            }
        }

        public override void OnNetworkSpawn()
        {
            ui.ShowUiWaitStartGame();
        }


        public InGamePlayerObject GetPlayerObject(bool isRedTem, int index)
        {
            return isRedTem ? redTeamMember[index] : greenTeamMember[index];
        }


        public void LoadQuestion()
        {
            if (_listQuestionData == null) _listQuestionData = new List<QuestionData>();
            var sheet = new ES3Spreadsheet();

            string path = Application.dataPath + "\\" + Config.QUESTION_FILE_NAME;

            if (!File.Exists(path))
            {
                //Notice.Instance.Show("Thông báo","Không tìm thấy file "+Config.QUESTION_FILE_NAME+" tại thư mục "+Application.dataPath,"","Đóng",null,null,true);
            }
            else
            {
                Debug.Log("Custom question sets");
                sheet.Load(path);

                for (int row = 1; row < sheet.RowCount; ++row)
                {
                    string title = sheet.GetCell<string>(0, row);
                    if (string.IsNullOrEmpty(title))
                    {
                        break;
                    }

                    QuestionData question = new QuestionData();
                    question.Id = row - 1;
                    question.Text = sheet.GetCell<string>(0, row);
                    question.AnswerDatas = new List<AnswerData>();
                    question.AnswerDatas.Add(new AnswerData
                    {
                        Id = 0,
                        Text = sheet.GetCell<string>(1, row),
                    });

                    question.AnswerDatas.Add(new AnswerData
                    {
                        Id = 1,
                        Text = sheet.GetCell<string>(2, row),
                    });

                    question.AnswerDatas.Add(new AnswerData
                    {
                        Id = 2,
                        Text = sheet.GetCell<string>(3, row),
                    });

                    question.AnswerDatas.Add(new AnswerData
                    {
                        Id = 3,
                        Text = sheet.GetCell<string>(4, row),
                    });

                    question.CorrectAnswerId = Int32.Parse(sheet.GetCell<string>(5, row)) - 1;
                    _listQuestionData.Add(question);
                }
            }

            if (_listQuestionData.Count > 0)
            {
                return;
            }

            Debug.Log("Default question set");

            TextAsset questionData = Resources.Load("Question") as TextAsset;

            if (questionData != null)
                _listQuestionData = JsonConvert.DeserializeObject<QuestionData[]>(questionData.ToString()).ToList();
        }

        public void RandomQuestionForNewPlayer(string userName)
        {
            var list = new List<int>();
            for (int questionId = 0; questionId < ListQuestionData.Count; ++questionId)
            {
                list.Add(questionId);
                int count = list.Count;
                int rand = (new Random()).Next(count);
                (list[rand], list[count - 1]) = (list[count - 1], list[rand]);
            }

            _questionForEachPlayer[userName] = list;
        }

        public QuestionDataRPC GetQuestion(string userName, int index)
        {
            if (index >= _questionForEachPlayer[userName].Count)
            {
                return new QuestionData
                {
                    Id = -1,
                    Text = "Bạn đã trả lời hết toàn bộ câu hỏi, vui lòng đợi đến khi trận đấu kết thúc",
                    AnswerDatas = new List<AnswerData>(),
                    CorrectAnswerId = -1,
                }.ConvertToRpc();
            }

            var questionId = _questionForEachPlayer[userName][index];
            var questionDataRPC = ListQuestionData[questionId].ConvertToRpc();
            questionDataRPC.Text = "Câu " + (index + 1) + "/" + _questionForEachPlayer[userName].Count + "\n" +
                                   questionDataRPC.Text;
            return questionDataRPC;
        }


        public int GetNextIndex(bool isRedTeam)
        {
            if (isRedTeam)
            {
                var last = _freeIndexRedTeam.Count - 1;
                if (last < 0) return -1;
                var value = _freeIndexRedTeam[last];
                _freeIndexRedTeam.RemoveAt(last);
                return value;
            }
            else
            {
                var last = _freeIndexGreenTeam.Count - 1;
                if (last < 0) return -1;
                var value = _freeIndexGreenTeam[last];
                _freeIndexGreenTeam.RemoveAt(last);
                return value;
            }
        }

        public void ResetIndex(bool isRedTeam, int index)
        {
            if (isRedTeam)
            {
                _freeIndexRedTeam.Add(index);
            }
            else
            {
                _freeIndexGreenTeam.Add(index);
            }
        }

        #region Rpc

        [ClientRpc]
        public void StartGameClientRpc(ClientRpcParams clientRpcParams = default)
        {
            gameState = GameState.PLaying;
            ui.ShowButtonInteractableButtons();
        }

        [ClientRpc]
        public void EndGameClientRpc(int gameResult)
        {
            result = gameResult;
            ui.EndGame(result);
        }

        #endregion
    }
}