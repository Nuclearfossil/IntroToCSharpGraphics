#include "StdAfx.h"
#include "D3D11.h"
#include "DirectXMath.h"
#include "Camera.h"


Camera::Camera(void)
{
    m_positionX = 0.0f;
    m_positionY = 0.0f;
    m_positionZ = 0.0f;

    m_rotationX = 0.0f;
    m_rotationY = 0.0f;
    m_rotationZ = 0.0f;
}

Camera::Camera( Camera &_camera )
{
}


Camera::~Camera(void)
{
}

void Camera::SetPosition(float _x, float _y, float _z)
{
    m_positionX = _x;
    m_positionY = _y;
    m_positionZ = _z;
    return;
}


void Camera::SetRotation(float _x, float _y, float _z)
{
    m_rotationX = _x;
    m_rotationY = _y;
    m_rotationZ = _z;
    return;
}

void Camera::Render()
{
    DirectX::XMFLOAT3  up, position, lookAt;
    float yaw, pitch, roll;
    DirectX::XMMATRIX rotationMatrix;


    // Setup the vector that points upwards.
    up.x = 0.0f;
    up.y = 1.0f;
    up.z = 0.0f;

    // Setup the position of the camera in the world.
    position.x = m_positionX;
    position.y = m_positionY;
    position.z = m_positionZ;

    // Setup where the camera is looking by default.
    lookAt.x = 0.0f;
    lookAt.y = 0.0f;
    lookAt.z = 1.0f;

    // Set the yaw (Y axis), pitch (X axis), and roll (Z axis) rotations in radians.
    pitch = m_rotationX * 0.0174532925f;
    yaw   = m_rotationY * 0.0174532925f;
    roll  = m_rotationZ * 0.0174532925f;

    // Create the rotation matrix from the yaw, pitch, and roll values.
    rotationMatrix = DirectX::XMMatrixRotationRollPitchYaw(pitch, yaw, roll);

    // Transform the lookAt and up vector by the rotation matrix so the view is correctly rotated at the origin.
    DirectX::XMVECTOR lookAtVector = DirectX::XMVector3TransformCoord(DirectX::XMLoadFloat3(&lookAt), rotationMatrix);
    DirectX::XMVECTOR upVector = DirectX::XMVector3TransformCoord(DirectX::XMLoadFloat3(&up), rotationMatrix);

    // Translate the rotated camera position to the location of the viewer.
    lookAtVector = DirectX::XMVectorAdd(DirectX::XMLoadFloat3(&position), lookAtVector);

    // Finally create the view matrix from the three updated vectors.
    m_viewMatrix = DirectX::XMMatrixLookAtLH(DirectX::XMLoadFloat3(&position), lookAtVector, upVector);

    return;
}