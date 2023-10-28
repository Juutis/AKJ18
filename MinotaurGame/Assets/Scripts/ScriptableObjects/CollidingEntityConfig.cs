using UnityEngine;

[CreateAssetMenu(fileName = "CollidingEntityConfig", menuName = "Configs/CollidingEntityConfig")]
public class CollidingEntityConfig : ScriptableObject
{
    [SerializeField]
    private LayerMask floorLayer;
    [SerializeField]
    private LayerMask wallLayer;
    [SerializeField]
    private bool gravity = true;
    [SerializeField]
    private float gravityStrength = 50f;
    [SerializeField]
    private float jumpSpeed = 15f;
    [SerializeField]
    private float maxFallSpeed = 15f;
    [SerializeField]
    private float runSpeed = 5f;

    public LayerMask FloorLayer { get { return floorLayer; } }
    public LayerMask WallLayer { get { return wallLayer; } }
    public bool Gravity { get { return gravity; } }
    public float GravityStrength { get { return gravityStrength; } }
    public float JumpSpeed { get { return jumpSpeed; } }
    public float MaxFallSpeed { get { return maxFallSpeed; } }
    public float RunSpeed { get { return runSpeed; } }
}
