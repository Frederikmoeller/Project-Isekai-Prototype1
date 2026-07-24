using Godot;

public partial class Enemy : Node2D
{
    [Export] public BaseInfo BaseInfo { get; set; }
    [Export] public float VisionDistance { get; set; } = 200f;

    [Export(PropertyHint.Range, "0,180,1")]
    public float VisionAngle { get; set; } = 60f;

    [Export] public Vector2 FacingDirection { get; set; } = Vector2.Right;
    [Export] public uint PlayerCollisionMask { get; set; } = 1u << 0;
    [Export] public uint ObstacleCollisionMask { get; set; } = 1u << 0;
    [Export] public bool ShowVisionDebug { get; set; } = true;

    public override void _Ready()
    {
        GD.Print($"Enemy mask: {PlayerCollisionMask}");
        SetFacingFromRotation();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (CanSeePlayer())
            GD.Print("Player spotted");
    }

    public bool CanSeePlayer()
    {
        var space = GetWorld2D().DirectSpaceState;

        var shape = new CircleShape2D { Radius = VisionDistance };
        var query = new PhysicsShapeQueryParameters2D
        {
            Shape = shape,
            Transform = new Transform2D(0f, GlobalPosition),
            CollisionMask = PlayerCollisionMask
        };

        var hits = space.IntersectShape(query, 32);

        Vector2 forward = FacingDirection.Normalized();
        float halfAngleRad = Mathf.DegToRad(VisionAngle * 0.5f);
        float angleThreshold = Mathf.Cos(halfAngleRad);

        foreach (var hit in hits)
        {
            if (!hit.ContainsKey("collider"))
                continue;

            var collider = hit["collider"].AsGodotObject();
            if (collider is not CharacterBody2D body)
                continue;

            Vector2 toTarget = body.GlobalPosition - GlobalPosition;
            float distance = toTarget.Length();

            if (distance > VisionDistance)
                continue;

            if (forward.Dot(toTarget.Normalized()) < angleThreshold)
                continue;

            var losQuery = PhysicsRayQueryParameters2D.Create(GlobalPosition, body.GlobalPosition);
            losQuery.CollisionMask = ObstacleCollisionMask;
            losQuery.Exclude = new Godot.Collections.Array<Rid>();

            var losHit = space.IntersectRay(losQuery);

            if (losHit.Count > 0)
            {
                if (!losHit.ContainsKey("collider"))
                    continue;

                var blockedBy = losHit["collider"].AsGodotObject();
                if (blockedBy is not CharacterBody2D)
                    continue;
            }

            return true;
        }

        return false;
    }

    public void SetFacingFromRotation()
    {
        FacingDirection = Vector2.Right.Rotated(Rotation).Normalized();
    }

    public override void _Draw()
    {
        if (!ShowVisionDebug)
            return;

        Vector2 forward = FacingDirection.Normalized();
        float halfAngleRad = Mathf.DegToRad(VisionAngle * 0.5f);
        int rayCount = 7;

        for (int i = 0; i < rayCount; i++)
        {
            float t = rayCount == 1 ? 0.5f : (float)i / (rayCount - 1);
            float angleOffset = Mathf.Lerp(-halfAngleRad, halfAngleRad, t);
            Vector2 dir = forward.Rotated(angleOffset).Normalized();
            DrawLine(Vector2.Zero, dir * VisionDistance, Colors.Yellow, 2f);
        }

        DrawArc(Vector2.Zero, VisionDistance, -halfAngleRad, halfAngleRad, 32, Colors.Yellow, 1f);
    }
}