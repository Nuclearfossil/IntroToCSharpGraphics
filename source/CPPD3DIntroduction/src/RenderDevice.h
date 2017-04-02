#ifndef __RENDERDEVICE_H__
#define __RENDERDEVICE_H__

// ======================================================================================
// Forward Declarations - so the header file doesn't have to #include anything
// ======================================================================================
struct ID3D11Device;
struct ID3D11DeviceContext;
struct IDXGISwapChain;
struct ID3D11Texture2D;
struct ID3D11RenderTargetView;

class VisualGrid;

// ======================================================================================
// Class Definition
// ======================================================================================
class RenderDevice
{
public:
	RenderDevice();
	~RenderDevice();

	bool Init( HWND _hwnd, UINT _width, UINT _height, BOOL _windowed );
	bool ResizeSwapchain(  HWND _hwnd );

	void Present();
	ID3D11Device* GetDevice() { return mDevice; }

	VisualGrid* CreateVisualGrid();

private:
    void UpdateViewport();
private:
	ID3D11Device*			mDevice;
	ID3D11DeviceContext*	mImmediateContext;
	IDXGISwapChain*			mSwapChain;
	ID3D11Texture2D*		mBackBuffer;
	ID3D11RenderTargetView*	mRenderTargetView;

	ID3D11Buffer*			mConstantBuffer;

	UINT					mWidth;
	UINT					mHeight;
};

#endif // __RENDERDEVICE_H__
