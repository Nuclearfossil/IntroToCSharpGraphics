#pragma once
class ColorShader
{
private:
    struct MatrixBufferType
    {
        DirectX::XMMATRIX world;
        DirectX::XMMATRIX view;
        DirectX::XMMATRIX projection;
    };

public:
    ColorShader(void);
    ColorShader(const ColorShader&);
    ~ColorShader(void);

    bool Init(ID3D11Device*, HWND);
    void Shutdown();
    bool Render(ID3D11DeviceContext*, int, DirectX::XMMATRIX&, DirectX::XMMATRIX&, DirectX::XMMATRIX&);


private:
    bool InitShader(ID3D11Device* _device, HWND _hwnd, WCHAR* _vsFilename, WCHAR* _psFilename);
    void ShutdownShader();
    void OutputShaderErrorMessage(ID3D10Blob*, HWND, WCHAR*);

    bool SetShaderParameters(ID3D11DeviceContext*, DirectX::XMMATRIX&, DirectX::XMMATRIX&, DirectX::XMMATRIX& );
    void RenderShader(ID3D11DeviceContext*, int);

private:
    ID3D11VertexShader*		m_vertexShader;
    ID3D11PixelShader*		m_pixelShader;
    ID3D11InputLayout*		m_layout;
    ID3D11Buffer*			m_matrixBuffer;
};

