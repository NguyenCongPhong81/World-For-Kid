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
}
