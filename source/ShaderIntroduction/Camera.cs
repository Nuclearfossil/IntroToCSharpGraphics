using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;

namespace ShaderIntroduction
{
    public class Camera
    {
        protected Vector3 mPosition = new Vector3(0, 0, 30);
        protected Vector3 mUp = Vector3.UnitY;
        protected Vector3 mDirection;

        protected const float mPitchLimit = 1.4f;

        protected const float mSpeed = 0.25f;
        protected const float mMouseSpeedX = 0.0045f;
        protected const float mMouseSpeedY = 0.0025f;

        protected MouseState mPrevMouse;


        /// <summary>
        /// Creates the instance of the camera.
        /// </summary>
        public Camera(GameWindow window)
        {
            // Create the direction vector and normalize it since it will be used for movement
            mDirection = Vector3.Zero - mPosition;
            mDirection.Normalize();

            // Create default camera matrices
            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, window.Width / (float)window.Height, 0.01f, 1000);
            View = CreateLookAt();
        }


        /// <summary>
        /// Creates the instance of the camera at the given location.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="target">The target towards which the camera is pointing.</param>
        public Camera(GameWindow window, Vector3 position, Vector3 target) : this(window)
        {
            mPosition = position;
            mDirection = target - mPosition;
            mDirection.Normalize();

            View = CreateLookAt();
        }

        /// <summary>
        /// Handle the camera movement using user input.
        /// </summary>
        protected virtual void ProcessInput()
        {
            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            // Move camera with WASD keys
            if (keyboard.IsKeyDown(Key.W))
                // Move forward and backwards by adding m_position and m_direction vectors
                mPosition += mDirection * mSpeed;

            if (keyboard.IsKeyDown(Key.S))
                mPosition -= mDirection * mSpeed;

            if (keyboard.IsKeyDown(Key.A))
                // Strafe by adding a cross product of m_up and m_direction vectors
                mPosition += Vector3.Cross(mUp, mDirection) * mSpeed;

            if (keyboard.IsKeyDown(Key.D))
                mPosition -= Vector3.Cross(mUp, mDirection) * mSpeed;

            if (keyboard.IsKeyDown(Key.Space))
                mPosition += mUp * mSpeed;

            if (keyboard.IsKeyDown(Key.ControlLeft) || keyboard.IsKeyDown(Key.X))
                mPosition -= mUp * mSpeed;

            if (mouse.IsButtonDown(MouseButton.Right))
            {
                // Calculate yaw to look around with a mouse
                mDirection = Vector3.Transform(mDirection, Matrix3.CreateFromAxisAngle(mUp, -mMouseSpeedX * (mouse.X - mPrevMouse.X)));

                // Pitch is limited to m_pitchLimit
                float angle = mMouseSpeedY * (mouse.Y - mPrevMouse.Y);
                if ((Pitch < mPitchLimit || angle > 0) && (Pitch > -mPitchLimit || angle < 0))
                {
                    mDirection = Vector3.Transform(mDirection, Matrix3.CreateFromAxisAngle(Vector3.Cross(mUp, mDirection), angle));
                }
            }

            mPrevMouse = mouse;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        public void Update()
        {
            // Handle camera movement
            ProcessInput();

            View = CreateLookAt();
        }

        /// <summary>
        /// Create a view (modelview) matrix using camera vectors.
        /// </summary>
        protected Matrix4 CreateLookAt()
        {
            return Matrix4.LookAt(mPosition, mPosition + mDirection, mUp);
        }

        /// <summary>
        /// Position vector.
        /// </summary>
        public Vector3 Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

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

        /// <summary>
        /// View (modelview) matrix accessor.
        /// </summary>
        public Matrix4 View { get; protected set; }

        /// <summary>
        /// Projection matrix accessor.
        /// </summary>
        public Matrix4 Projection { get; protected set; }

    }
}
