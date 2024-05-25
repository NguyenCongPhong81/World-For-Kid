using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class ChosseCharacterManager : MonoBehaviour
{
    [Header("UI")]
    //Text
    [SerializeField] private Text txtSkillDerscription;
    [SerializeField] private Text txtCharacterName;
    //TMP
    [SerializeField] private TMP_Text tmpPower;
    [SerializeField] private TMP_Text tmpDefene;
    [SerializeField] private TMP_Text tmpEnery;
    [SerializeField] private TMP_Text tmpHeatlh;
    //Button
    [SerializeField] private Button btnNextCharacter;
    [SerializeField] private Button btnPreCharacter;
    [SerializeField] private Button btnJoinRoom;
    //Image
    [SerializeField] private Image imgSkilll;

    [SerializeField] private InputField playerName;


    [Header("Data")]
    //Mesh
    [SerializeField] private List<Mesh> listSkinMesh;
    [SerializeField] private List<Material> listSkinMaterial;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private List<GameObject> particles;

    public void Awake()
    {
        btnNextCharacter.onClick.AddListener(NextCharacter);
        btnPreCharacter.onClick.AddListener(PreCharacter);
        playerName.text = GameConfig.Instance.PlayerData.DisplayName;
        SelectCharacter(GameConfig.Instance.PlayerData.IndexCharacter);
        playerName.onEndEdit.AddListener(ChangeDisplayName);
        btnJoinRoom.onClick.AddListener(Fight);
        Notice.Instance.Hide();
    }

    private void ChangeDisplayName(string text)
    {
        GameConfig.Instance.PlayerData.DisplayName = text;
        PlayerPrefs.SetString("DisplayName", GameConfig.Instance.PlayerData.DisplayName);
    }

    private void Fight()
    {
        LoadingManager.Instance.Show();
        SceneManager.LoadSceneAsync("MainGamePlay", LoadSceneMode.Single);
    }


    private void NextCharacter()
    {
        var index = GameConfig.Instance.PlayerData.IndexCharacter + 1;
        index = index % GameConfig.Instance.CharacterDatas.Count;
        if (index < 0)
            index += GameConfig.Instance.CharacterDatas.Count;

        SelectCharacter(index);
    }

    private void PreCharacter()
    {
        var index = GameConfig.Instance.PlayerData.IndexCharacter - 1;
        index = index % GameConfig.Instance.CharacterDatas.Count;
        if (index < 0)
            index += GameConfig.Instance.CharacterDatas.Count;

        SelectCharacter(index);
    }

    private void SelectCharacter(int index)
    {
        index = index % GameConfig.Instance.CharacterDatas.Count;
        if (index < 0)
            index += GameConfig.Instance.CharacterDatas.Count;
        particles[GameConfig.Instance.PlayerData.IndexCharacter].SetActive(false);
        particles[index].SetActive(true);
        GameConfig.Instance.PlayerData.IndexCharacter = index;
        PlayerPrefs.SetInt("IndexCharacter", GameConfig.Instance.PlayerData.IndexCharacter);
        skinnedMeshRenderer.sharedMesh = listSkinMesh[index];

        skinnedMeshRenderer.SetMaterials(new List<Material> { listSkinMaterial[index] });
        var data = GameConfig.Instance.CharacterDatas[index];
        // MyCharracterdata.Id = data.Id;
        // MyCharracterdata.SkinType = data.SkinType;
        // MyCharracterdata.SkillType = data.SkillType;
        // MyCharracterdata.Range = data.Range;
        tmpPower.text = data.Power.ToString();
        tmpHeatlh.text = data.Health.ToString();
        tmpDefene.text = data.Defense.ToString();
        tmpEnery.text = data.Energy.ToString();
        imgSkilll.sprite = data.Sprite;
        txtSkillDerscription.text = data.Description;
        txtCharacterName.text = data.NameCharacter;
    }


}
