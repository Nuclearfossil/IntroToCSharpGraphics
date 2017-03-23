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
	RenderDevice(void);
	~RenderDevice(void);

	bool Init( HWND _hwnd, UINT _width, UINT _height, BOOL _windowed );
	bool ResizeSwapchain(  HWND _hwnd );

	void Present();
	ID3D11Device* GetDevice() { return m_Device; }

	VisualGrid* CreateVisualGrid();

private:
	ID3D11Device*			m_Device;
	ID3D11DeviceContext*	m_ImmediateContext;
	IDXGISwapChain*			m_SwapChain;
	ID3D11Texture2D*		m_BackBuffer;
	ID3D11RenderTargetView*	m_RenderTargetView;

	ID3D11Buffer*			m_ConstantBuffer;

	UINT					m_width;
	UINT					m_height;
};

#endif // __RENDERDEVICE_H__
