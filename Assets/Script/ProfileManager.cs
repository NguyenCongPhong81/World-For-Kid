using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script
{
    public class ProfileManager : MonoBehaviour
    {
        [SerializeField] private InputField playerName;
        [SerializeField] private Button btnNext;
        [SerializeField] private Button btnPre;
        [SerializeField] private Button btnFight;
        [SerializeField] private Text characterName;
        [SerializeField] private Text skillDescription;
        [SerializeField] private TMP_Text power;
        [SerializeField] private TMP_Text defense;
        [SerializeField] private TMP_Text heath;
        [SerializeField] private TMP_Text energy;
        [SerializeField] private Image imageSkill;
        [SerializeField] private List<Mesh> listSkinMesh;
        [SerializeField] private List<Material> listSkinMaterial;
        [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
        [SerializeField] private List<GameObject> particles;

        public void Awake()
        {
            btnNext.onClick.AddListener(NextCharacter);
            btnPre.onClick.AddListener(PreCharacter);
            playerName.text = Config.Instance.MyPlayerData.DisplayName;
            SelectCharacter(Config.Instance.MyPlayerData.IndexCharacter);
            playerName.onEndEdit.AddListener(ChangeDisplayName);
            btnFight.onClick.AddListener(Fight);
            Notice.Instance.Hide();
        }

        private void ChangeDisplayName(string text)
        {
            Config.Instance.MyPlayerData.DisplayName = text;
            PlayerPrefs.SetString("DisplayName", Config.Instance.MyPlayerData.DisplayName);
        }

        private void Fight()
        {
            LoadingManager.Instance.Show();
            SceneManager.LoadSceneAsync("MainGame", LoadSceneMode.Single);
        }


        private void NextCharacter()
        {
            var index = Config.Instance.MyPlayerData.IndexCharacter + 1;
            index = index % Config.Instance.CharacterDatas.Count;
            if (index < 0)
                index += Config.Instance.CharacterDatas.Count;

            SelectCharacter(index);
        }

        private void PreCharacter()
        {
            var index = Config.Instance.MyPlayerData.IndexCharacter - 1;
            index = index % Config.Instance.CharacterDatas.Count;
            if (index < 0)
                index += Config.Instance.CharacterDatas.Count;

            SelectCharacter(index);
        }

        private void SelectCharacter(int index)
        {
            index = index % Config.Instance.CharacterDatas.Count;
            if (index < 0)
                index += Config.Instance.CharacterDatas.Count;
            particles[Config.Instance.MyPlayerData.IndexCharacter].SetActive(false);
            particles[index].SetActive(true);
            Config.Instance.MyPlayerData.IndexCharacter = index;
            PlayerPrefs.SetInt("IndexCharacter", Config.Instance.MyPlayerData.IndexCharacter);
            skinnedMeshRenderer.sharedMesh = listSkinMesh[index];

            skinnedMeshRenderer.SetMaterials(new List<Material> { listSkinMaterial[index] });
            var data = Config.Instance.CharacterDatas[index];
            // MyCharracterdata.Id = data.Id;
            // MyCharracterdata.SkinType = data.SkinType;
            // MyCharracterdata.SkillType = data.SkillType;
            // MyCharracterdata.Range = data.Range;
            power.text = data.Power.ToString();
            heath.text = data.Health.ToString();
            defense.text = data.Defense.ToString();
            energy.text = data.Energy.ToString();
            imageSkill.sprite = data.Sprite;
            skillDescription.text = data.Description;
            characterName.text = data.NameCharacter;
        }
    }
}