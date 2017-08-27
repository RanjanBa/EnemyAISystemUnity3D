using System.Collections.Generic;
using UnityEngine;

public class SceneCharactersManager : MonoBehaviour
{
    public List<CharacterTypeOfGameObject> m_PlayerGameObjectWithType = new List<CharacterTypeOfGameObject>();
    public List<CharacterTypeOfGameObject> m_EnemyGameObjectWithType = new List<CharacterTypeOfGameObject>();

    public void AddPlayerToCharaterList(GameObject gm, CharacterType typeChar)
    {
        CharacterTypeOfGameObject chGmType = new CharacterTypeOfGameObject(gm, typeChar);
        m_PlayerGameObjectWithType.Add(chGmType);
    }

    public void AddEnemyToCharaterList(GameObject gm, CharacterType typeChar)
    {
        CharacterTypeOfGameObject chGmType = new CharacterTypeOfGameObject(gm, typeChar);
        m_EnemyGameObjectWithType.Add(chGmType);
    }
}
