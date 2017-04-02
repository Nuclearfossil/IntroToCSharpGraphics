#ifndef __VISUALGRID_H__
#define __VISUALGRID_H__

class VisualGrid
{
public:
	VisualGrid();
	~VisualGrid();

	void Render( );

	ID3D11Buffer* mVertexBuffer;
};

#endif // __VISUALGRID_H__