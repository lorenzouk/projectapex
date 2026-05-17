using UnityEngine;

public class SpeedRamp : MonoBehaviour
{
    public float boostPower = -20f;

    public enum BoostType
    {
        Forward,
        Up,
        ForwardAndUp
    }

    public BoostType boostType = BoostType.Forward;

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

        if (player == null)
        {
            return;
        }

        Vector3 boostDir = Vector3.zero;

        switch (boostType)
        {
            case BoostType.Forward:
                boostDir = transform.forward;
                boostDir.y = 0f;
                boostDir.Normalize();
                break;

            case BoostType.Up:
                boostDir = Vector3.up;
                break;

            case BoostType.ForwardAndUp:
                boostDir = transform.forward + Vector3.up * 0.5f;
                boostDir.Normalize();
                break;
        }

        player.AddExternalVelocity(boostDir * boostPower);
    }
}