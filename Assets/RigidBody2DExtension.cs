using UnityEngine;

public static class RigidBody2DExtension
{
    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        body.AddForce(dir.normalized * (wearoff <= 0f ? 0f : explosionForce) * wearoff);
    }
}
