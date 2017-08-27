
public interface IBoxerManager
{
    void Punch(float sqrDistFromOpp);
    void Kick(float sqrDistFromOpp);
    void EnableGiveHitCollider(HitColliderType name);
    void DisableGiveHitCollider(HitColliderType name);
    void TakeAwarness(int blockID);
    void TakeDamageByBoxing(int damage, int hitID);
}
