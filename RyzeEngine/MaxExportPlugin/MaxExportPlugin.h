#pragma once

//**************************************************************************/
// DESCRIPTION: Includes for Plugins
//***************************************************************************/
#include "resource.h"
#include "ExpDefines.h"
#include <istdplug.h>
#include <iparamb2.h>
#include <iparamm2.h>
#include <maxtypes.h>
#include <triobj.h>
#include "iskin.h"
#include <stdmat.h>
#include <impapi.h>
#include <impexp.h>
#include <fstream>
#include <mesh.h>
#include <vector>
#include <map>
#include <iterator>

using namespace std;

#define MaxExportPlugin_CLASS_ID	Class_ID(0x9b16aeb4, 0x9c3e2d6e)

extern TCHAR *GetString(int id);
extern HINSTANCE hInstance;

struct VertexPosition
{
	Point3 pos;
	Point3 normal;
	Point3 tangent;
	Point3 bitangent;
	Point3 tex;
	Point4 boneWeights;
};

class MaxExportPlugin : public SceneExport
{
public:
	MaxExportPlugin();
	~MaxExportPlugin();

	int				ExtCount();					// Number of extensions supported
	const TCHAR*	Ext(int n);					// Extension #n (i.e. "3DS")
	const TCHAR*	LongDesc();					// Long ASCII description (i.e. "Autodesk 3D Studio File")
	const TCHAR*	ShortDesc();				// Short ASCII description (i.e. "3D Studio")
	const TCHAR*	AuthorName();				// ASCII Author name
	const TCHAR*	CopyrightMessage();			// ASCII Copyright message
	const TCHAR*	OtherMessage1();			// Other message #1
	const TCHAR*	OtherMessage2();			// Other message #2
	unsigned int	Version();					// Version number * 100 (i.e. v3.01 = 301)
	void			ShowAbout(HWND hWnd);		// Show DLL's "About..." box

	BOOL SupportsOptions(int ext, DWORD options);
	int  DoExport(const TCHAR *name, ExpInterface *ei, Interface *i, BOOL suppressPrompts = FALSE, DWORD options = 0);//options = 0 (SCENE_EXPORT_SELECTED)
};

class MaxExportPluginClassDesc : public ClassDesc2
{
public:
	virtual int IsPublic() 					{ return TRUE; }
	virtual void* Create(BOOL) 				{ return new MaxExportPlugin(); }
	virtual const TCHAR* ClassName() 		{ return GetString(IDS_CLASS_NAME); }
	virtual SClass_ID SuperClassID() 		{ return SCENE_EXPORT_CLASS_ID; }
	virtual Class_ID ClassID() 				{ return MaxExportPlugin_CLASS_ID; }
	virtual const TCHAR* Category() 		{ return GetString(IDS_CATEGORY); }

	virtual const TCHAR* InternalName() 	{ return _T("MaxExportPlugin"); }	// returns fixed parsable name (scripter-visible name)
	virtual HINSTANCE HInstance() 			{ return hInstance; }				// returns owning module handle
};

class MeshExporter: public ITreeEnumProc
{
public:
	// SubExporters
	void PrepareVerts(TriObject* TObj, INode* node, ISkinContextData* skinData);
	void ExportFaces();
	void ExportVerts();
	void ExportMaterial(INode* node);

	// Main functions
	int callback(INode *node);
	void ProcNode(INode *node);

private:
	TriObject* GetTriObjFromNode(INode *node);
	Point3 GetVertexNormal(Mesh* mesh, int faceNo, RVertex* rv);
	Point3 GetVertexTangent(Mesh* mesh, int faceNo);
	Point3 GetVertexBitangent(Mesh* mesh, int faceNo);
	int GetEqualVertex(const Point3 &vp, const Point3 &vn, const Point3 &tc);
	void ExportStdMaterial(Mtl *material);
	void ExportDefaultMaterial();

	vector<VertexPosition> m_vertices;
	vector<DWORD> m_indices;
	vector<int> m_mtlsIndices;
};
