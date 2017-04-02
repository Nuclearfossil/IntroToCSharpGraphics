// CPPD3DIntroduction.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "Resource.h"

//==============================================
// TODO:
//   Eventually move this off into a separate utility library
//   We output any caught memory leaks here.
#if defined(DEBUG) || defined(_DEBUG)
#include "crtdbg.h"
#endif
//==============================================

#include "D3D11.h"
#include "RenderDevice.h"
#include "VisualGrid.h"

//--------------------------------------------------------------------------------------
// Forward declarations
//--------------------------------------------------------------------------------------
HRESULT InitWindow( HINSTANCE _instance, int _cmdShow );
HRESULT InitResources( void );
LRESULT CALLBACK    WndProc( HWND, UINT, WPARAM, LPARAM );

//--------------------------------------------------------------------------------------
// Globals
//--------------------------------------------------------------------------------------
RenderDevice    g_RenderDevice;
VisualGrid*     g_visualGrid = NULL;
HINSTANCE	    g_hInst = NULL;
HWND		    g_hWnd	= NULL;


int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    // Enable run-time memory check for debug builds.
    // Again, this should live in a separate library
#if defined(DEBUG) | defined(_DEBUG)
    _CrtSetDbgFlag( _CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF );
#endif

    if( FAILED( InitWindow( hInstance, nCmdShow ) ) )
    {
        return 0;
    }

    if ( FAILED(InitResources()) )
    {
        return 0;
    }

    // Main message loop
    MSG msg = {0};
    while( WM_QUIT != msg.message )
    {
        if( PeekMessage( &msg, NULL, 0, 0, PM_REMOVE ) )
        {
            TranslateMessage( &msg );
            DispatchMessage( &msg );
        }
        else
        {
            g_RenderDevice.Present();
        }
    }

    delete g_visualGrid;
    return ( int )msg.wParam;
}

//--------------------------------------------------------------------------------------
// Register class and create window
//--------------------------------------------------------------------------------------
HRESULT InitWindow( HINSTANCE _instance, int _cmdShow )
{
    // Register class
    WNDCLASSEX wcex;
    wcex.cbSize = sizeof( WNDCLASSEX );
    wcex.style = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc = WndProc;
    wcex.cbClsExtra = 0;
    wcex.cbWndExtra = 0;
    wcex.hInstance = _instance;
    wcex.hIcon = LoadIcon( _instance, ( LPCTSTR )IDI_CPPD3DINTRODUCTION );
    wcex.hCursor = LoadCursor( NULL, IDC_ARROW );
    wcex.hbrBackground = ( HBRUSH )( COLOR_WINDOW + 1 );
    wcex.lpszMenuName = NULL;
    wcex.lpszClassName = L"WTGTP_01";
    wcex.hIconSm = LoadIcon( wcex.hInstance, ( LPCTSTR )IDI_CPPD3DINTRODUCTION );
    if( !RegisterClassEx( &wcex ) )
    {
        return E_FAIL;
    }

    // Create window
    g_hInst = _instance;
    RECT rc = { 0, 0, 800, 600 };
    AdjustWindowRect( &rc, WS_OVERLAPPEDWINDOW, FALSE );
    g_hWnd = CreateWindow( L"WTGTP_01", L"Walking The Graphics Pipeline - 01", WS_OVERLAPPEDWINDOW,
        CW_USEDEFAULT, CW_USEDEFAULT, rc.right - rc.left, rc.bottom - rc.top, NULL, NULL, _instance,
        NULL );
    if( !g_hWnd )
    {
        return E_FAIL;
    }

    ShowWindow( g_hWnd, _cmdShow );

    if( g_RenderDevice.Init( g_hWnd, rc.right, rc.bottom, TRUE ) )
    {
        return S_OK;
    }

    return S_FALSE;
}

HRESULT InitResources( void )
{
    g_visualGrid = g_RenderDevice.CreateVisualGrid();
    return S_FALSE;
}

//--------------------------------------------------------------------------------------
// Called every time the application receives a message
//--------------------------------------------------------------------------------------
LRESULT CALLBACK WndProc( HWND _hWnd, UINT _msg, WPARAM _wParam, LPARAM _lParam )
{
    switch( _msg )
    {
    case WM_PAINT:
        g_RenderDevice.Present();
        break;

    case WM_DESTROY:
        PostQuitMessage( 0 );
        break;

    case WM_SIZE:
        // TODO: Only update the size when we're finished resizing
        g_RenderDevice.ResizeSwapchain(_hWnd);
        break;

    default:
        return DefWindowProc( _hWnd, _msg, _wParam, _lParam );
    }

    return 0;
}
