#include "StdAfx.h"
#include "d3d11.h"
#include "DirectXMath.h"
#include "D3Dcompiler.h"
#include "fstream"
#include "ColorShader.h"


ColorShader::ColorShader(void)
{
    m_vertexShader = NULL;
    m_pixelShader  = NULL;
    m_layout       = NULL;
    m_matrixBuffer = NULL;
}

ColorShader::ColorShader(const ColorShader&)
{
}

ColorShader::~ColorShader(void)
{
}

bool ColorShader::Init( ID3D11Device* _device, HWND _hwnd )
{
    return InitShader( _device, _hwnd, L"color.vs", L"color.ps");
}

void ColorShader::Shutdown()
{
    ShutdownShader();
}

bool ColorShader::Render( ID3D11DeviceContext* _context, int _indexCount, DirectX::XMMATRIX& _worldMatrix, DirectX::XMMATRIX& _viewMatrix, DirectX::XMMATRIX& _projectionMatrix )
{
    bool result = false;

    result = SetShaderParameters( _context, _worldMatrix, _viewMatrix, _projectionMatrix );
    if ( result )
    {
        RenderShader( _context, _indexCount );
        result = true;
    }

    return result;
}

bool ColorShader::InitShader( ID3D11Device* _device, HWND _hwnd, WCHAR* _vsFilename, WCHAR* _psFilename )
{
    HRESULT result;
    ID3D10Blob* errorMessage;
    ID3D10Blob* vertexShaderBuffer;
    ID3D10Blob* pixelShaderBuffer;
    D3D11_INPUT_ELEMENT_DESC polygonLayout[2];
    unsigned int numElements;
    D3D11_BUFFER_DESC matrixBufferDesc;


    // Initialize the pointers this function will use to null.
    errorMessage = 0;
    vertexShaderBuffer = 0;
    pixelShaderBuffer = 0;

    // Compile the vertex shader code.
    result = D3DCompileFromFile(_vsFilename, nullptr, D3D_COMPILE_STANDARD_FILE_INCLUDE, "ColorVertexShader", "vs_5_0", D3D10_SHADER_ENABLE_STRICTNESS, 0, &vertexShaderBuffer, &errorMessage);
    if(FAILED(result))
    {
        // If the shader failed to compile it should have writen something to the error message.
        if(errorMessage)
        {
            OutputShaderErrorMessage(errorMessage, _hwnd, _vsFilename);
        }
        // If there was nothing in the error message then it simply could not find the shader file itself.
        else
        {
            MessageBox( _hwnd, _vsFilename, L"Missing Shader File", MB_OK);
        }

        return false;
    }

    // Compile the pixel shader code.
    result = D3DCompileFromFile( _psFilename, NULL, NULL, "ColorPixelShader", "ps_5_0", D3D10_SHADER_ENABLE_STRICTNESS, 0, &pixelShaderBuffer, &errorMessage);
    if(FAILED(result))
    {
        // If the shader failed to compile it should have writen something to the error message.
        if(errorMessage)
        {
            OutputShaderErrorMessage(errorMessage, _hwnd, _psFilename);
        }
        // If there was  nothing in the error message then it simply could not find the file itself.
        else
        {
            MessageBox( _hwnd, _psFilename, L"Missing Shader File", MB_OK);
        }

        return false;
    }

    // Create the vertex shader from the buffer.
    result = _device->CreateVertexShader(vertexShaderBuffer->GetBufferPointer(), vertexShaderBuffer->GetBufferSize(), NULL, &m_vertexShader);
    if(FAILED(result))
    {
        return false;
    }

    // Create the pixel shader from the buffer.
    result = _device->CreatePixelShader(pixelShaderBuffer->GetBufferPointer(), pixelShaderBuffer->GetBufferSize(), NULL, &m_pixelShader);
    if(FAILED(result))
    {
        return false;
    }

    // Now setup the layout of the data that goes into the shader.
    // This setup needs to match the VertexType stucture in the ModelClass and in the shader.
    polygonLayout[0].SemanticName = "POSITION";
    polygonLayout[0].SemanticIndex = 0;
    polygonLayout[0].Format = DXGI_FORMAT_R32G32B32_FLOAT;
    polygonLayout[0].InputSlot = 0;
    polygonLayout[0].AlignedByteOffset = 0;
    polygonLayout[0].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
    polygonLayout[0].InstanceDataStepRate = 0;

    polygonLayout[1].SemanticName = "COLOR";
    polygonLayout[1].SemanticIndex = 0;
    polygonLayout[1].Format = DXGI_FORMAT_R32G32B32A32_FLOAT;
    polygonLayout[1].InputSlot = 0;
    polygonLayout[1].AlignedByteOffset = D3D11_APPEND_ALIGNED_ELEMENT;
    polygonLayout[1].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
    polygonLayout[1].InstanceDataStepRate = 0;

    // Get a count of the elements in the layout.
    numElements = sizeof(polygonLayout) / sizeof(polygonLayout[0]);

    // Create the vertex input layout.
    result = _device->CreateInputLayout(polygonLayout, numElements, vertexShaderBuffer->GetBufferPointer(), 
        vertexShaderBuffer->GetBufferSize(), &m_layout);
    if(FAILED(result))
    {
        return false;
    }

    // Release the vertex shader buffer and pixel shader buffer since they are no longer needed.
    vertexShaderBuffer->Release();
    vertexShaderBuffer = 0;

    pixelShaderBuffer->Release();
    pixelShaderBuffer = 0;

    // Setup the description of the dynamic matrix constant buffer that is in the vertex shader.
    matrixBufferDesc.Usage = D3D11_USAGE_DYNAMIC;
    matrixBufferDesc.ByteWidth = sizeof(MatrixBufferType);
    matrixBufferDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    matrixBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    matrixBufferDesc.MiscFlags = 0;
    matrixBufferDesc.StructureByteStride = 0;

    // Create the constant buffer pointer so we can access the vertex shader constant buffer from within this class.
    result = _device->CreateBuffer(&matrixBufferDesc, NULL, &m_matrixBuffer);
    if(FAILED(result))
    {
        return false;
    }

    return true;
}


void ColorShader::RenderShader( ID3D11DeviceContext* _context, int _indexCount )
{
    // Set the vertex input layout.
    _context->IASetInputLayout(m_layout);

    // Set the vertex and pixel shaders that will be used to render this triangle.
    _context->VSSetShader(m_vertexShader, NULL, 0);
    _context->PSSetShader(m_pixelShader, NULL, 0);

    // Render the triangle.
    _context->DrawIndexed(_indexCount, 0, 0);
}


void ColorShader::ShutdownShader()
{
    // Release the matrix constant buffer.
    if(m_matrixBuffer)
    {
        m_matrixBuffer->Release();
        m_matrixBuffer = 0;
    }

    // Release the layout.
    if(m_layout)
    {
        m_layout->Release();
        m_layout = 0;
    }

    // Release the pixel shader.
    if(m_pixelShader)
    {
        m_pixelShader->Release();
        m_pixelShader = 0;
    }

    // Release the vertex shader.
    if(m_vertexShader)
    {
        m_vertexShader->Release();
        m_vertexShader = 0;
    }
}

void ColorShader::OutputShaderErrorMessage( ID3D10Blob* _errorMsg, HWND _hwnd, WCHAR* _shaderFilename)
{
    char* compileErrors;
    SIZE_T bufferSize, i;
    std::ofstream fout;


    // Get a pointer to the error message text buffer.
    compileErrors = (char*)(_errorMsg->GetBufferPointer());

    // Get the length of the message.
    bufferSize = _errorMsg->GetBufferSize();

    // Open a file to write the error message to.
    fout.open("shader-error.txt");

    // Write out the error message.
    for(i=0; i<bufferSize; i++)
    {
        fout << compileErrors[i];
    }

    // Close the file.
    fout.close();

    // Release the error message.
    _errorMsg->Release();
    _errorMsg = 0;

    // Pop a message up on the screen to notify the user to check the text file for compile errors.
    MessageBox(_hwnd, L"Error compiling shader.  Check shader-error.txt for message.", _shaderFilename, MB_OK);
}

bool ColorShader::SetShaderParameters( ID3D11DeviceContext* _context, DirectX::XMMATRIX& _worldMatrix, DirectX::XMMATRIX& _viewMatrix, DirectX::XMMATRIX& _projectionMatrix )
{
    HRESULT result;
    D3D11_MAPPED_SUBRESOURCE mappedResource;
    MatrixBufferType* dataPtr;
    unsigned int bufferNumber;

    DirectX::XMMATRIX worldMatrix, viewMatrix, projectionMatrix;

    // Transpose the matrices to prepare them for the shader.
    worldMatrix = XMMatrixTranspose(_worldMatrix);
    viewMatrix = XMMatrixTranspose(_viewMatrix);
    projectionMatrix = XMMatrixTranspose(_projectionMatrix);

    // Lock the constant buffer so it can be written to.
    result = _context->Map(m_matrixBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource);
    if(FAILED(result))
    {
        return false;
    }

    // Get a pointer to the data in the constant buffer.
    dataPtr = (MatrixBufferType*)mappedResource.pData;

    // Copy the matrices into the constant buffer.
    dataPtr->world = worldMatrix;
    dataPtr->view = viewMatrix;
    dataPtr->projection = projectionMatrix;

    // Unlock the constant buffer.
    _context->Unmap(m_matrixBuffer, 0);

    // Set the position of the constant buffer in the vertex shader.
    bufferNumber = 0;

    // Finanly set the constant buffer in the vertex shader with the updated values.
    _context->VSSetConstantBuffers(bufferNumber, 1, &m_matrixBuffer);

    return true;
}


