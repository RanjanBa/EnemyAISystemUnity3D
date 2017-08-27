using UnityEngine;
using gunClass_namespace;

[RequireComponent(typeof(AudioSource))]
public class AssaulfRifle : GunClass
{
    public Transform m_LeftHandPosition;
    public Transform m_RightHandPosition;

    private void Start()
    {
        Initialize();
        m_MaggazineTotalBullet = 30;
        m_ownerTag = transform.parent.tag;
        m_typeOfGun = TypeOfGun.TwoHandedGun;
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
