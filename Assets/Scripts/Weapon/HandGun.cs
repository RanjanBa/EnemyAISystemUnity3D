using UnityEngine;
using GunClass_namespace;

[RequireComponent(typeof(AudioSource))]
public class HandGun : GunClass
{
    private void Start()
    {
        Initialize();
        m_MaggazineTotalBullet = 12;
        m_TypeOfGun = TypeOfGun.OneHandedGun;
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
