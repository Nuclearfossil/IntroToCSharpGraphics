using SharpDX;
using OpenTK.Input;
using System;

namespace D3D11Introduction
{
    public class CameraBase
    {
        protected Vector3 mPosition = new Vector3(0, 0, 5);
        protected Vector3 mUp = Vector3.UnitY;
        protected Vector3 mDirection = new Vector3(0,0,0);

        protected MouseState mPrevMouse;
        protected Matrix view;
        protected Matrix projection;
        protected const float kPitchLimit = 1.4f;

        protected const float kSpeed = 0.25f;
        protected const float kMouseSpeedX = 0.0045f;
        protected const float kMouseSpeedY = 0.0025f;

        protected const float kPiOver4 = 0.7853982f;

        protected int ScreenWidth { get; set; }
        protected int ScreenHeight { get; set; }
        protected int MouseX { get; set; }
        protected int MouseY { get; set; }

        public CameraBase(int screenWidth, int screenHeight)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;

            float ratio = (float)ScreenWidth / (float)ScreenHeight;
            projection = Matrix.PerspectiveFovRH(kPiOver4, ratio, 0.01f, 1000);

        }

        public void CreateLookAt()
        {
            View = Matrix.LookAtRH(mPosition, mDirection, mUp);
        }

        public virtual void Update()
        {
            ProcessInput();
            CreateLookAt();
        }

        protected virtual void ProcessInput()
        {
            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            // Move camera with WASD keys
            if (keyboard.IsKeyDown(Key.W))
                // Move forward and backwards by adding m_position and m_direction vectors
                mPosition += mDirection * kSpeed;

            if (keyboard.IsKeyDown(Key.S))
                mPosition -= mDirection * kSpeed;

            if (keyboard.IsKeyDown(Key.A))
                // Strafe by adding a cross product of m_up and m_direction vectors
                mPosition += Vector3.Cross(mUp, mDirection) * kSpeed;

            if (keyboard.IsKeyDown(Key.D))
                mPosition -= Vector3.Cross(mUp, mDirection) * kSpeed;

            if (keyboard.IsKeyDown(Key.Space))
                mPosition += mUp * kSpeed;

            if (keyboard.IsKeyDown(Key.ControlLeft) || keyboard.IsKeyDown(Key.X))
                mPosition -= mUp * kSpeed;

            if (mouse.IsButtonDown(MouseButton.Right))
            {
                // Calculate yaw to look around with a mouse
                
                mDirection = Vector3.Transform(mDirection, Matrix3x3.RotationAxis(mUp, -kMouseSpeedX * (mouse.X - mPrevMouse.X)));

                // Pitch is limited to m_pitchLimit
                float angle = kMouseSpeedY * (mouse.Y - mPrevMouse.Y);
                if ((Pitch < kPitchLimit || angle > 0) && (Pitch > -kPitchLimit || angle < 0))
                {
                    Matrix3x3.RotationAxis(Vector3.Cross(mUp, mDirection), angle);
                    mDirection = Vector3.Transform(mDirection, Matrix3x3.RotationAxis(Vector3.Cross(mUp, mDirection), angle));
                }
            }

            mPrevMouse = mouse;
        }

        public Matrix View { get; protected set; }

        /// <summary>
        /// Yaw of the camera in radians.
        /// </summary>
        public double Yaw
        {
            get { return Math.PI - Math.Atan2(mDirection.X, mDirection.Z); }
        }

        /// <summary>
        /// Pitch of the camera in radians.
        /// </summary>
        public double Pitch
        {
            get { return Math.Asin(mDirection.Y); }
        }
    }
}
