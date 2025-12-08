using UnityEngine;

public static class ScO_Data
{
    public static class BackAndForthPreset
    {
        public const float moveSpeedX = 10f;
        public const float moveLimitX = 15f;
        public const float scaleX = 10f;

        public const float moveSpeedX2 = 20f;
        public const float moveLimitX2 = 28f;
        public const float scaleX2 = 5f;

        public const float moveSpeedZ = 10f;
        public const float moveLimitZ = 15f;
        public const float scaleZ = 10f;

        public const float moveSpeedZ2 = 20f;
        public const float moveLimitZ2 = 18f;
        public const float scaleZ2 = 5f;
    }

    public static class RAPAndRSPreset
    {
        public static readonly float[] rotateAroundRadius = new float[] { 12f, 12f, 12f, 12f };
        public static readonly float[] rotateAroundSpeed = new float[] { 50f, 70f, 50f, 70f };
        public static readonly EnviromentController.RotationDirection[] rotateDirRAP = new EnviromentController.RotationDirection[] {
            EnviromentController.RotationDirection.Clockwise,
            EnviromentController.RotationDirection.CounterClockwise,
            EnviromentController.RotationDirection.Clockwise,
            EnviromentController.RotationDirection.CounterClockwise
        };

        public static readonly float[] selfRotateSpeed = new float[] { 90f, 120f, 90f, 120f };
        public static readonly EnviromentController.RotationDirection[] rotateDirRS = new EnviromentController.RotationDirection[] {
            EnviromentController.RotationDirection.Clockwise,
            EnviromentController.RotationDirection.CounterClockwise,
            EnviromentController.RotationDirection.Clockwise,
            EnviromentController.RotationDirection.CounterClockwise
        };
    }

    public static class RotateAroundPointPreset
    {
        public static readonly float[] rotateAroundRadius = new float[] { 10f, 17f, 20f, 23f, 26f, 29f, 31f, 33f };
        public static readonly float[] rotateAroundSpeed = new float[] { 80f, 85f, 90f, 95f, 90f, 85f, 90f, 85f };
        public static readonly EnviromentController.RotationDirection[] rotateDirRAP = new EnviromentController.RotationDirection[] {
            EnviromentController.RotationDirection.CounterClockwise,
            EnviromentController.RotationDirection.Clockwise,
            EnviromentController.RotationDirection.CounterClockwise,
            EnviromentController.RotationDirection.Clockwise,
            EnviromentController.RotationDirection.CounterClockwise,
            EnviromentController.RotationDirection.Clockwise,
            EnviromentController.RotationDirection.CounterClockwise,
            EnviromentController.RotationDirection.Clockwise
        };
    }
}

