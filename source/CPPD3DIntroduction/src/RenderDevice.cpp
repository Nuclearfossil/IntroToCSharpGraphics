#include "StdAfx.h"
#include "D3D11.h"
#include "DirectXMath.h"
#include "VisualGrid.h"
#include "RenderDevice.h"
#include "Utils.h"


struct VS_CONSTANT_BUFFER
{
    DirectX::XMMATRIX       mWorld;
    DirectX::XMMATRIX       mView;
    DirectX::XMMATRIX       mProjection;
};

RenderDevice::RenderDevice(void)
{
    mDevice = NULL;
    mImmediateContext = NULL;
    mSwapChain = NULL;
    mBackBuffer = NULL;
    mRenderTargetView = NULL;

}


RenderDevice::~RenderDevice(void)
{
    SafeRelease( mRenderTargetView );
    SafeRelease(mConstantBuffer);
    SafeRelease( mBackBuffer );
    SafeRelease( mSwapChain );
    SafeRelease( mImmediateContext );
    SafeRelease( mDevice );
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
        D3D_FEATURE_LEVEL_11_1,
        D3D_FEATURE_LEVEL_11_0
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

    if( FAILED( D3D11CreateDeviceAndSwapChain( 
                    NULL, 
                    D3D_DRIVER_TYPE_HARDWARE, 
                    NULL, 
                    0, 
                    featureLevels, 
                    numFeatureLevels,
                    D3D11_SDK_VERSION, 
                    &swapChainDesc, 
                    &mSwapChain, 
                    &mDevice, 
                    NULL, 
                    &mImmediateContext ) ) )
    {
        return FALSE;
    }

    // Create a render target view
    if( FAILED( mSwapChain->GetBuffer( 0, __uuidof( ID3D11Texture2D ), (LPVOID*)&mBackBuffer ) ) )
    {
        return FALSE;
    }

    HRESULT hr = mDevice->CreateRenderTargetView( mBackBuffer, NULL, &mRenderTargetView );
    SafeRelease(mBackBuffer);
    if( FAILED( hr ) )
    {
        return FALSE;
    }

    mImmediateContext->OMSetRenderTargets( 1, &mRenderTargetView, NULL );

    mWidth = _width;
    mHeight = _height;

    UpdateViewport();
    return true;
}

void RenderDevice::UpdateViewport()
{
    D3D11_VIEWPORT viewport;
    viewport.Width = (FLOAT)mWidth;
    viewport.Height = (FLOAT)mHeight;
    viewport.MinDepth = 0.0f;
    viewport.MaxDepth = 1.0f;
    viewport.TopLeftX = 0;
    viewport.TopLeftY = 0;
    mImmediateContext->RSSetViewports( 1, &viewport );

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
    HRESULT hr = mDevice->CreateBuffer( &cbDesc, NULL, &mConstantBuffer );

    if( SUCCEEDED( hr ) )
    {
        // Set the buffer.
        mImmediateContext->VSSetConstantBuffers( 0, 1, &mConstantBuffer );
    }
}

bool RenderDevice::ResizeSwapchain( HWND _hwnd )
{
    if (mImmediateContext == nullptr)
        return false;

    RECT rc;
    GetClientRect( _hwnd, &rc );

    mWidth = rc.right - rc.left;
    mHeight = rc.bottom - rc.top;

    // Sanity check
    if ( (mSwapChain != NULL) && !(mWidth > 0 && mHeight > 0) )
    {
        return FALSE;
    }

    mImmediateContext->OMSetRenderTargets( 0, 0, 0 );

    mRenderTargetView->Release();

    if ( FAILED( mSwapChain->ResizeBuffers( 1, mWidth, mHeight, DXGI_FORMAT_R8G8B8A8_UNORM, 0 ) ) )
    {
        return FALSE;
    }

    // Create a render target view
    if( FAILED( mSwapChain->GetBuffer( 0, __uuidof( ID3D11Texture2D ), (LPVOID*)&mBackBuffer ) ) )
    {
        return FALSE;
    }

    HRESULT hr = mDevice->CreateRenderTargetView( mBackBuffer, NULL, &mRenderTargetView );
    SafeRelease(mBackBuffer);
    if( FAILED( hr ) )
    {
        return FALSE;
    }

    mImmediateContext->OMSetRenderTargets( 1, &mRenderTargetView, NULL );

    D3D11_VIEWPORT viewport;
    viewport.Width = (FLOAT)mWidth;
    viewport.Height = (FLOAT)mHeight;
    viewport.MinDepth = 0.0f;
    viewport.MaxDepth = 1.0f;
    viewport.TopLeftX = 0;
    viewport.TopLeftY = 0;
    mImmediateContext->RSSetViewports( 1, &viewport );
    return SUCCEEDED(hr);
}

void RenderDevice::Present()
{
    float ClearColor[4] = { 0.0f, 0.125f, 0.1f, 1.0f }; // RGBA
    mImmediateContext->ClearRenderTargetView( mRenderTargetView, ClearColor );

    mSwapChain->Present( 0, 0 );
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

    mDevice->CreateBuffer( &bufferDesc, &initData, &(visualGrid->mVertexBuffer) );
    return visualGrid;
}