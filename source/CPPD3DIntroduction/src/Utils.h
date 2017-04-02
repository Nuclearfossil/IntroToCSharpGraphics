#pragma once

template <class T>
    void SafeRelease(T& IUnk)
{
    if (IUnk)
    {
        IUnk->Release();
        IUnk = NULL;
    }
}
