using UnityEngine;
using gunClass_namespace;

[RequireComponent(typeof(AudioSource))]
public class HandGun : GunClass
{
    public Transform m_LeftHandPosition;

    private void Start()
    {
        Initialize();
        m_MaggazineTotalBullet = 12;
        m_typeOfGun = TypeOfGun.OneHandedGun;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    public override void Fire(Vector3 direction)
    {
        base.Fire(direction);
    }
}
