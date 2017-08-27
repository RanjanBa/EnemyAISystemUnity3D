using UnityEngine;
using UnityEditor;

public class CreateGameObjectWizardMenu : ScriptableWizard
{
    public GameObject CharacterGameObject;
    public CharacterType TypeOfCharacter;

    [MenuItem("CreateWizardMenu/Create CoverPosition")]
    static void CreateCoverPositionWizardMenu()
    {
        GameObject gm = new GameObject();
        gm.name = "CoverPosition";
        gm.AddComponent<BoxCollider>();
        CoverPosition co = gm.AddComponent<CoverPosition>();
        co.m_LayerMask = 1 << 8;
        if (Selection.activeTransform)
        {
            gm.transform.SetParent(Selection.activeTransform);
        }
        gm.transform.localPosition = Vector3.zero;
        gm.transform.localRotation = Quaternion.identity;
    }

    [MenuItem("CreateWizardMenu/Create Character")]
    static void CreateCharacterWizardMenu()
    {
        DisplayWizard<CreateGameObjectWizardMenu>("Create Character", "Create Character");
    }

    private void OnWizardCreate()
    {
        GameObject gameObject = Instantiate(CharacterGameObject, CharacterGameObject.transform.position, Quaternion.identity);

        if (TypeOfCharacter == CharacterType.Player)
        {
            gameObject.name = "Player";
            gameObject.tag = "Player";
            gameObject.AddComponent<PlayerManager>();
        }
        else if (TypeOfCharacter == CharacterType.PlayerBoxerCampanion)
        {
            gameObject.name = "PlayerBoxerCampanion";
            gameObject.tag = "PlayerCampanion";
            gameObject.AddComponent<PlayerBoxerCampanion>();
        }
        else if (TypeOfCharacter == CharacterType.PlayerGunnerCampanion)
        {
            gameObject.name = "PlayerGunnerCampanion";
            gameObject.tag = "PlayerCampanion";
            gameObject.AddComponent<PlayerGunnerCampanion>();
        }
        else if (TypeOfCharacter == CharacterType.EnemyBoxer)
        {
            gameObject.name = "EnemyBoxer";
            gameObject.tag = "Enemy";
            gameObject.AddComponent<EnemyBoxer>();
        }
        else if (TypeOfCharacter == CharacterType.EnemyGunner)
        {
            gameObject.name = "EnemyGunner";
            gameObject.tag = "Enemy";
            gameObject.AddComponent<EnemyGunner>();
        }
    }
}
