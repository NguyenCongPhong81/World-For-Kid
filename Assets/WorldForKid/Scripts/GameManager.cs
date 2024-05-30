using Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    private List<int> _freeIndexRedTeam;
    private List<int> _freeIndexGreenTeam;


    public List<PlayerObject> redTeamMember;
    public List<PlayerObject> greenTeamMember;


    public Dictionary<string, PlayerInGameData> DictPlayerInGameData { get; private set; } =
            new Dictionary<string, PlayerInGameData>();


    public static GameManager Instance;

    private void Awake()
    {
        LoadingManager.Instance.Hide();
        Instance = this;

        _freeIndexRedTeam = new List<int>();
        _freeIndexGreenTeam = new List<int>();
        //_questionForEachPlayer = new Dictionary<string, List<int>>();

        //for (int i = redTeamMember.Count - 1; i >= 0; --i)
        //{
        //    _freeIndexRedTeam.Add(i);
        //}

        //for (int i = greenTeamMember.Count - 1; i >= 0; --i)
        //{
        //    _freeIndexGreenTeam.Add(i);
        //}

        var index = PlayerPrefs.GetInt("IndexCharacter");
        Debug.Log("index" + index);
        InitPlayer(index);

    }

    public PlayerObject GetPlayerObject(bool isRedTem, int index)
    {
        return isRedTem ? redTeamMember[index] : greenTeamMember[index];
    }

    //temp
    private void InitPlayer(int index)
    {
        greenTeamMember[0].InitFake(index);

    }
}
