using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class Config : Singleton<Config>
{
    private MyPlayerData _myPlayerData;
    private List<CharacterData> _characterDatas;

    public const int TIME_PLAY = 360;
    public const string QUESTION_FILE_NAME = "Question.csv";
    public const string TEXT_COMPLETE = "Bạn đã trả lời hết toàn bộ câu hỏi";

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
    
    public MyPlayerData MyPlayerData
    {
        get
        {
            if (_myPlayerData == null)
            {
                _myPlayerData = new MyPlayerData();
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