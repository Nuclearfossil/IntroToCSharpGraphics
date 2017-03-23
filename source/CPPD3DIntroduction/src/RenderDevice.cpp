#include "StdAfx.h"
#include "D3D11.h"
#include "D3D11.h"
#include "DirectXMath.h"
#include "VisualGrid.h"
#include "RenderDevice.h"

template <class T>
    void SafeRelease(T& IUnk)
{
    if (IUnk)
    {
        IUnk->Release();
        IUnk = NULL;
    }
}

struct VS_CONSTANT_BUFFER
{
    DirectX::XMMATRIX       mWorld;
    DirectX::XMMATRIX       mView;
    DirectX::XMMATRIX       mProjection;
};

RenderDevice::RenderDevice(void)
{
    m_Device = NULL;
    m_ImmediateContext = NULL;
    m_SwapChain = NULL;
    m_BackBuffer = NULL;
    m_RenderTargetView = NULL;

}


RenderDevice::~RenderDevice(void)
{
    SafeRelease( m_RenderTargetView );
    SafeRelease( m_BackBuffer );
    SafeRelease( m_ImmediateContext );
    SafeRelease( m_Device );
}


bool RenderDevice::Init( HWND _hwnd, UINT _width, UINT _height, BOOL _windowed )
{
    D3D_DRIVER_TYPE driverTypes[] =
    {
        D3D_DRIVER_TYPE_HARDWARE,
        D3D_DRIVER_TYPE_WARP,
        D3D_DRIVER_TYPE_REFERENCE,
    };
    UINT numDriverTypes = ARRAYSIZE( driverTypes );

    D3D_FEATURE_LEVEL featureLevels[] =
    {
        D3D_FEATURE_LEVEL_11_0,
        D3D_FEATURE_LEVEL_10_1,
        D3D_FEATURE_LEVEL_10_0,
    };
    UINT numFeatureLevels = ARRAYSIZE( featureLevels );

    DXGI_SWAP_CHAIN_DESC swapChainDesc;
    ZeroMemory( &swapChainDesc, sizeof(swapChainDesc) );
    swapChainDesc.BufferCount = 1;
    swapChainDesc.BufferDesc.Width = _width;
    swapChainDesc.BufferDesc.Height = _height;
    swapChainDesc.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
    swapChainDesc.BufferDesc.RefreshRate.Numerator = 60;
    swapChainDesc.BufferDesc.RefreshRate.Denominator = 1;
    swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
    swapChainDesc.OutputWindow = _hwnd;
    swapChainDesc.SampleDesc.Count = 1;
    swapChainDesc.SampleDesc.Quality = 0;
    swapChainDesc.Windowed = TRUE;

    if( FAILED( D3D11CreateDeviceAndSwapChain( NULL, D3D_DRIVER_TYPE_HARDWARE, NULL, 0, featureLevels, numFeatureLevels,
        D3D11_SDK_VERSION, &swapChainDesc, &m_SwapChain, &m_Device, NULL, &m_ImmediateContext ) ) )
    {
        return FALSE;
    }

    // Create a render target view
    if( FAILED( m_SwapChain->GetBuffer( 0, __uuidof( ID3D11Texture2D ), (LPVOID*)&m_BackBuffer ) ) )
    {
        return FALSE;
    }

    HRESULT hr = m_Device->CreateRenderTargetView( m_BackBuffer, NULL, &m_RenderTargetView );
    SafeRelease(m_BackBuffer);
    if( FAILED( hr ) )
    {
        return FALSE;
    }

    m_ImmediateContext->OMSetRenderTargets( 1, &m_RenderTargetView, NULL );

    m_width = _width;
    m_height = _height;

    D3D11_VIEWPORT viewport;
    viewport.Width = (FLOAT)_width;
    viewport.Height = (FLOAT)_height;
    viewport.MinDepth = 0.0f;
    viewport.MaxDepth = 1.0f;
    viewport.TopLeftX = 0;
    viewport.TopLeftY = 0;
    m_ImmediateContext->RSSetViewports( 1, &viewport );

    // Setup constant buffers
    // Fill in a buffer description.
    D3D11_BUFFER_DESC cbDesc;
    cbDesc.ByteWidth = sizeof( VS_CONSTANT_BUFFER );
    cbDesc.Usage = D3D11_USAGE_DYNAMIC;
    cbDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    cbDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    cbDesc.MiscFlags = 0;
    cbDesc.StructureByteStride = 0;

    // Create the buffer.
    hr = m_Device->CreateBuffer( &cbDesc, NULL, &m_ConstantBuffer );

    if( SUCCEEDED( hr ) )
    {
        // Set the buffer.
        m_ImmediateContext->VSSetConstantBuffers( 0, 1, &m_ConstantBuffer );
    }

    return true;
}

bool RenderDevice::ResizeSwapchain( HWND _hwnd )
{
    if (m_ImmediateContext == nullptr)
        return false;

    RECT rc;
    GetClientRect( _hwnd, &rc );

    m_width = rc.right - rc.left;
    m_height = rc.bottom - rc.top;

    // Sanity check
    if ( (m_SwapChain != NULL) && !(m_width > 0 && m_height > 0) )
    {
        return FALSE;
    }

    m_ImmediateContext->OMSetRenderTargets( 0, 0, 0 );

    m_RenderTargetView->Release();

    if ( FAILED( m_SwapChain->ResizeBuffers( 1, m_width, m_height, DXGI_FORMAT_R8G8B8A8_UNORM, 0 ) ) )
    {
        return FALSE;
    }

    // Create a render target view
    if( FAILED( m_SwapChain->GetBuffer( 0, __uuidof( ID3D11Texture2D ), (LPVOID*)&m_BackBuffer ) ) )
    {
        return FALSE;
    }

    HRESULT hr = m_Device->CreateRenderTargetView( m_BackBuffer, NULL, &m_RenderTargetView );
    SafeRelease(m_BackBuffer);
    if( FAILED( hr ) )
    {
        return FALSE;
    }

    m_ImmediateContext->OMSetRenderTargets( 1, &m_RenderTargetView, NULL );

    D3D11_VIEWPORT viewport;
    viewport.Width = (FLOAT)m_width;
    viewport.Height = (FLOAT)m_height;
    viewport.MinDepth = 0.0f;
    viewport.MaxDepth = 1.0f;
    viewport.TopLeftX = 0;
    viewport.TopLeftY = 0;
    m_ImmediateContext->RSSetViewports( 1, &viewport );
    return SUCCEEDED(hr);
}

void RenderDevice::Present()
{
    float ClearColor[4] = { 0.0f, 0.125f, 0.1f, 1.0f }; // RGBA
    m_ImmediateContext->ClearRenderTargetView( m_RenderTargetView, ClearColor );

    m_SwapChain->Present( 0, 0 );
}

struct SimpleVertexCombined
{
    DirectX::XMFLOAT3   Pos;
    DirectX::XMFLOAT3   Col;
};

SimpleVertexCombined verticesCombo[] =
{
    DirectX::XMFLOAT3( 0.0f, 0.5f, 0.5f ),
    DirectX::XMFLOAT3( 0.0f, 0.0f, 0.5f ),
    DirectX::XMFLOAT3( 0.5f, -0.5f, 0.5f ),
    DirectX::XMFLOAT3( 0.5f, 0.0f, 0.0f ),
    DirectX::XMFLOAT3( -0.5f, -0.5f, 0.5f ),
    DirectX::XMFLOAT3( 0.0f, 0.5f, 0.0f ),
};



VisualGrid* RenderDevice::CreateVisualGrid()
{
    VisualGrid* visualGrid = new VisualGrid();

    D3D11_BUFFER_DESC bufferDesc;
    bufferDesc.Usage = D3D11_USAGE_DEFAULT;
    bufferDesc.ByteWidth = sizeof(SimpleVertexCombined) * 3;
    bufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    bufferDesc.CPUAccessFlags = 0;
    bufferDesc.MiscFlags = 0;

    D3D11_SUBRESOURCE_DATA initData;
    initData.pSysMem = verticesCombo;
    initData.SysMemPitch = 0;
    initData.SysMemSlicePitch = 0;

    m_Device->CreateBuffer( &bufferDesc, &initData, &(visualGrid->m_vertexBuffer) );
    return visualGrid;
}