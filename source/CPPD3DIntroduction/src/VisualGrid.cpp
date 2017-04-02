#include "StdAfx.h"
#include "D3D11.h"
#include "VisualGrid.h"

#include "Utils.h"

VisualGrid::VisualGrid(void)
{
}


VisualGrid::~VisualGrid(void)
{
    if (mVertexBuffer != nullptr)
    {
        SafeRelease(mVertexBuffer);
    }
}
