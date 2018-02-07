public enum SelectCharacterForm
{
    Player,
    AIManager,
}

public enum CharacterType
{
    Player,
    PlayerBoxerCampanion,
    PlayerGunnerCampanion,
    EnemyBoxer,
    EnemyGunner,
}

public enum HitColliderType
{
    LeftElbow,
    RightElbow,
    LeftHand,
    RightHand,
    LeftKnee,
    RightKnee,
    LeftAnkle,
    RightAnkle,
}

public enum AnimationState
{
    IdleAnimation,
    WalkingAnimation,
    RunningAnimation,
}

public enum TypeOfGun
{
    OneHandedGun,
    TwoHandedGun,
}

public enum UseIK
{
    LeftHand,
    RightHand,
}

public enum DamageType
{
    DamageByHand,
    DamageByGun,
}

public enum CameraMovementType
{
    FreeLook,
    LockLook,
}