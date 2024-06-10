using Script;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityExtensions;

public class GameManager : NetworkBehaviour
{


    private List<int> _freeIndexRedTeam;
    private List<int> _freeIndexGreenTeam;

    public UIGameManager ui;
    public List<PlayerObject> redTeamMember;
    public List<PlayerObject> greenTeamMember;
    public NetworkVariable<float> timeStart = new NetworkVariable<float>();


    public Dictionary<string, InGamePlayer> DictPlayerInGame { get; private set; } =
            new Dictionary<string, InGamePlayer>();
    public Dictionary<string, PlayerInGameData> DictPlayerInGameData { get; private set; } =
            new Dictionary<string, PlayerInGameData>();

    public Dictionary<ulong, string> MapClientIdToUserNam { get; private set; } = new Dictionary<ulong, string>();

    [HideInInspector] public GameState gameState = GameState.WaitingStart;
    [HideInInspector] public int result;

    [HideInInspector] public InGamePlayer myPlayer;



    public static GameManager Instance;

    private void Awake()
    {
        LoadingManager.Instance.Hide();
        Instance = this;

        _freeIndexRedTeam = new List<int>();
        _freeIndexGreenTeam = new List<int>();
        //_questionForEachPlayer = new Dictionary<string, List<int>>();

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

    public PlayerObject GetPlayerObject(bool isRedTem, int index)
    {
        return isRedTem ? redTeamMember[index] : greenTeamMember[index];
    }

    //question

    //index
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

    //Rpc
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

}
