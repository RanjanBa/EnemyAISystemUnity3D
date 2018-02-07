using UnityEngine;
using GunClass_namespace;

[RequireComponent(typeof(AudioSource))]
public class AssaulfRifle : GunClass
{
    private void Start()
    {
        m_MaggazineTotalBullet = 30;
        m_TypeOfGun = TypeOfGun.TwoHandedGun;
        Initialize();
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
