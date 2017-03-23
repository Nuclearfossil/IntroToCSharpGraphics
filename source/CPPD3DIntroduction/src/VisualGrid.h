#ifndef __VISUALGRID_H__
#define __VISUALGRID_H__

class VisualGrid
{
public:
	VisualGrid(void);
	~VisualGrid(void);

	void Render( );

	ID3D11Buffer*		m_vertexBuffer;
};

#endif // __VISUALGRID_H__