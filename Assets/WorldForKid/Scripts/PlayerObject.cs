using Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObject : MonoBehaviour
{
    [SerializeField] private Slider heath;
    [SerializeField] private Slider energy;
    [SerializeField] private Text playerName;
    [SerializeField] private Text textHeath;
    [SerializeField] private Text textEnergy;
    [SerializeField] private GameObject mine;
    [SerializeField] private List<Mesh> listSkinMesh;
    [SerializeField] private List<Material> listSkinMaterial;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private List<GameObject> particles;
    [SerializeField] private GameObject arrow;

    public PlayerAnimation playerAnimation;

    private PlayerInGameData _playerInGameData;
    public CharacterData MyCharacterData { get; set; }
    public static PlayerObject LastSelected;

    public PlayerInGameData PlayerInGameData => _playerInGameData;

    public void Init(PlayerInGameData ownerData)
    {
        mine.SetActive(false);
        

        _playerInGameData = ownerData;
        playerName.text = _playerInGameData.DisplayName;

        var characterIndex = _playerInGameData.CharacterType;

        skinnedMeshRenderer.sharedMesh = listSkinMesh[characterIndex];
        skinnedMeshRenderer.SetMaterials(new List<Material> { listSkinMaterial[characterIndex] });

        MyCharacterData = Config.Instance.CharacterDatas[_playerInGameData.CharacterType];

        
    }

    public void InitFake(int index)
    {
        skinnedMeshRenderer.sharedMesh = listSkinMesh[index];
        skinnedMeshRenderer.SetMaterials(new List<Material> { listSkinMaterial[index] });
        MyCharacterData = GameConfig.Instance.CharacterDatas[index];
        heath.value = (MyCharacterData.Health + .0F) / MyCharacterData.Health;
        energy.value = (MyCharacterData.Energy + .0F) / MyCharacterData.Energy;
        playerName.text = GameConfig.Instance.PlayerData.DisplayName;
        textHeath.text =
            string.Format("{0}/{1}", MyCharacterData.Health, MyCharacterData.Health);
        textEnergy.text =
            string.Format("{0}/{1}", MyCharacterData.Energy, MyCharacterData.Energy);
    }

    private void OnMouseDown()
    {
        LastSelected = this;
        arrow.SetActive(true);
        playerAnimation.Skill3(LastSelected.transform, null, 1);
    }

    public void UseAttackNormal(string targetUserName, int number = 1)
    {
        var targetPlayerData = InGameManager.Instance.DictPlayerInGameData[targetUserName];
        var targetPlayerObject =
            InGameManager.Instance.GetPlayerObject(targetPlayerData.IsRedTem, targetPlayerData.IndexPosition);
        var trans = targetPlayerObject.transform;

        playerAnimation.AttackNormal(trans,
            () =>
            {
                particles[_playerInGameData.CharacterType].SetActive(false);
                var targetCharacterData =
                    targetPlayerObject.MyCharacterData;

                //var a = MyCharacterData.Power;
                //float b = targetCharacterData.Defense;
                //var damage = Mathf.RoundToInt((a + b) * (a + b) / (b * b));
                //damage = Mathf.Max(damage, 1);
                //targetPlayerObject.ReceiveNormalAttack(damage);
            }, number);
    }
}
