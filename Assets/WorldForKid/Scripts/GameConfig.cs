using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameConfig : Singleton<GameConfig>
{
    private PlayerData _myPlayerData;
    private List<CharacterData> _characterDatas;

    public const int TIME_PLAY = 360;
    public const string QUESTION_FILE_NAME = "Question.csv";
    public const string TEXT_COMPLETE = "B?n ?ã tr? l?i h?t toàn b? câu h?i";

    public List<CharacterData> CharacterDatas
    {
        get
        {
            if (_characterDatas == null)
            {
                _characterDatas = (Resources.LoadAll<CharacterData>("CharacterData")).ToList();
            }

            return _characterDatas;
        }
    }

    public PlayerData PlayerData
    {
        get
        {
            if (_myPlayerData == null)
            {
                _myPlayerData = new PlayerData();
                if (PlayerPrefs.HasKey("DisplayName"))
                {
                    _myPlayerData.DisplayName = PlayerPrefs.GetString("DisplayName");
                }

                if (PlayerPrefs.HasKey("IndexCharacter"))
                {
                    _myPlayerData.IndexCharacter = PlayerPrefs.GetInt("IndexCharacter");
                }
            }
            return _myPlayerData;
        }
    }
}
