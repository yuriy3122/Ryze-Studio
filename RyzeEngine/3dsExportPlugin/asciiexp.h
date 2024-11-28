#ifndef __ASCIIEXP__H
#define __ASCIIEXP__H

#include "iskin.h"
#include "Max.h"
#include "resource.h"
#include "istdplug.h"
#include "stdmat.h"
#include "decomp.h"
#include "shape.h"
#include "interpik.h"
#include "maxtextfile.h"
#include "ExpDefines.h"
#include <fstream>
#include <vector>
#include <map>
#include <iterator>

using namespace std;

extern ClassDesc* GetAsciiExpDesc();
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

#define VERSION			200			// Version number * 100
#define CFGFILENAME		_T("ASCIIEXP.CFG")	// Configuration file

class AsciiExp : public SceneExport
{
public:
	AsciiExp();
	~AsciiExp();

	// SceneExport methods
	int    ExtCount();     // Number of extensions supported 
	const TCHAR * Ext(int n);     // Extension #n (i.e. "ASC")
	const TCHAR * LongDesc();     // Long ASCII description (i.e. "Ascii Export") 
	const TCHAR * ShortDesc();    // Short ASCII description (i.e. "Ascii")
	const TCHAR * AuthorName();    // ASCII Author name
	const TCHAR * CopyrightMessage();   // ASCII Copyright message 
	const TCHAR * OtherMessage1();   // Other message #1
	const TCHAR * OtherMessage2();   // Other message #2
	unsigned int Version();     // Version number * 100 (i.e. v3.01 = 301) 
	void	ShowAbout(HWND hWnd);  // Show DLL's "About..." box
	int		DoExport(const TCHAR *name,ExpInterface *ei,Interface *i, BOOL suppressPrompts=FALSE, DWORD options=0); // Export	file
	BOOL	SupportsOptions(int ext, DWORD options);

private:
	Interface*	ip;
};

class MeshExporter : public ITreeEnumProc
{
public:
	// SubExporters
	void PrepareVerts(TriObject* TObj, INode* node, ISkinContextData* skinData);
	void ExportFaces();
	void ExportVerts();
	void ExportMaterial(INode* node);

	// Main functions
	int callback(INode* node);
	void ProcNode(INode* node);

private:
	TriObject* GetTriObjFromNode(INode* node);
	Modifier* GetSkinModifierFromNode(INode* node);

	Point3 GetVertexNormal(Mesh* mesh, int faceNo, RVertex* rv);
	Point3 GetVertexTangent(Mesh* mesh, int faceNo);
	Point3 GetVertexBitangent(Mesh* mesh, int faceNo);
	int GetEqualVertex(const Point3& vp, const Point3& vn, const Point3& tc);

	void ExportStdMaterial(Mtl* material);
	void ExportDefaultMaterial();
	void ExportAnimationKeys(INode* node, ISkin* iskin);

	vector<VertexPosition> m_vertices;
	vector<DWORD> m_indices;
	vector<int> m_mtlsIndices;
};

#endif // __ASCIIEXP__H

