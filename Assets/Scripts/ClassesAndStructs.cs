using UnityEngine;

[System.Serializable]
public struct CharacterTypeOfGameObject
{
    public GameObject characterGameObject;
    public CharacterType TypeOfCharacter;

    public CharacterTypeOfGameObject(GameObject gm, CharacterType typeChar)
    {
        characterGameObject = gm;
        TypeOfCharacter = typeChar;
    }
}

[System.Serializable]
public class CharacterTypeAndTimerOfGameObject
{
    public CharacterTypeOfGameObject CharacterGameObjectWithType;
    public float VisibilityTimer;

    public CharacterTypeAndTimerOfGameObject(CharacterTypeOfGameObject charType, float timer)
    {
        CharacterGameObjectWithType = charType;
        VisibilityTimer = timer;
    }

    public float IncrementOrDecrementTimer(float deltaTime)
    {
        VisibilityTimer += deltaTime;
        return VisibilityTimer;
    }
}

[System.Serializable]
public class MinMaxValue
{
    [Range(0f, 60f)]
    public float MinValue;
    [Range(0f, 60f)]
    public float MaxValue;

    public MinMaxValue()
    {
        MinValue = 0f;
        MaxValue = 1f;
    }

    public MinMaxValue(float maxValue)
    {
        this.MinValue = 0f;
        this.MaxValue = maxValue;
    }

    public MinMaxValue(float minValue, float maxValue)
    {
        this.MinValue = minValue;
        this.MaxValue = maxValue;
    }
}

