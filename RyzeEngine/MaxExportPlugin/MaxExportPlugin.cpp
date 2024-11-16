#include "MaxExportPlugin.h"
#include "decomp.h"
#include "modstack.h"

static MeshExporter		g_meshExporter;
static ofstream			g_outputFile;
Interface*				g_interfacePtr;

ClassDesc2* GetMaxExportPluginDesc()
{ 
	static MaxExportPluginClassDesc MaxExportPluginDesc;
	return &MaxExportPluginDesc;
}

INT_PTR CALLBACK MaxExportPluginOptionsDlgProc(HWND hWnd, UINT message, WPARAM, LPARAM lParam) 
{
	static MaxExportPlugin* imp = nullptr;

	switch(message)
	{
		case WM_INITDIALOG:
			imp = (MaxExportPlugin*)lParam;
			CenterWindow(hWnd,GetParent(hWnd));
			return TRUE;

		case WM_CLOSE:
			EndDialog(hWnd, 0);
			return 1;
	}
	return 0;
}

/*--- MaxExportPlugin ------------------------------------------------------*/
MaxExportPlugin::MaxExportPlugin()
{
}

MaxExportPlugin::~MaxExportPlugin() 
{
}

int MaxExportPlugin::ExtCount()
{
	return 1;
}

const TCHAR *MaxExportPlugin::Ext(int i)
{		
	if (i == 0)
	{
		return _T("rz");
	}
		
	return _T("");
}

const TCHAR *MaxExportPlugin::LongDesc()
{
	return _T("Ryze export format");
}
	
const TCHAR *MaxExportPlugin::ShortDesc()
{			
	return _T("Ryze");
}

const TCHAR *MaxExportPlugin::AuthorName()
{			
	return _T("");
}

const TCHAR *MaxExportPlugin::CopyrightMessage() 
{	
	return _T("");
}

const TCHAR *MaxExportPlugin::OtherMessage1() 
{		
	return _T("");
}

const TCHAR *MaxExportPlugin::OtherMessage2() 
{		
	return _T("");
}

unsigned int MaxExportPlugin::Version()
{				
	return 100;
}

void MaxExportPlugin::ShowAbout(HWND /*hWnd*/)
{			
	// Optional
}

BOOL MaxExportPlugin::SupportsOptions(int /*ext*/, DWORD /*options*/)
{
	// Decide which options to support.  Simply return true for each option supported by each Extension the exporter supports.
	return TRUE;
}

int	MaxExportPlugin::DoExport(const TCHAR* name, ExpInterface* ei, Interface* ip, BOOL /*suppressPrompts*/, DWORD /*options*/)
{
	//if (!suppressPrompts)
	//{
	//	DialogBoxParam(hInstance, MAKEINTRESOURCE(IDD_PANEL), GetActiveWindow(), MaxExportPluginOptionsDlgProc, (LPARAM)this);
	//}

	g_outputFile.open(name, ios::out | ios::binary);

	g_interfacePtr = ip;

	int currHeader = ID_VERSION;
	g_outputFile.write((char*)&currHeader, 2);

	// Go Through all scene nodes
	ei->theScene->EnumTree(&g_meshExporter);

	g_outputFile.close();

	// Return TRUE If the file is exported properly
	return TRUE;
}

/*-------------------- MeshExporter ------------------------------------------------------*/
int MeshExporter::callback(INode* node)
{
	ProcNode(node);

	return TREE_CONTINUE;
}

template<typename A, typename B>
pair<B, A> flip_pair(const pair<A, B>& p)
{
	return pair<B, A>(p.second, p.first);
}

template<typename A, typename B>
multimap<B, A> flip_map(const map<A, B>& src)
{
	multimap<B, A> dst;
	transform(src.begin(), src.end(), inserter(dst, dst.begin()), flip_pair<A, B>);

	return dst;
}

static void TruncateBoneWeights(map<int, float>& boneWeights)
{
	multimap<float, int> sortedWeights = flip_map(boneWeights);

	boneWeights.clear();

	int count = 0;
	float weightsSum = 0.0f;

	for (auto iter = sortedWeights.rbegin(); iter != sortedWeights.rend(); ++iter)
	{
		if (count < 4 && iter->first > 0.0001f)
		{
			boneWeights[iter->second] = iter->first;
			weightsSum += iter->first;
		}

		count++;
	}

	float diff = 1.0f - weightsSum;

	if (diff > 0.0001f)
	{
		diff /= (float)boneWeights.size();
	}

	for (auto iter = boneWeights.begin(); iter != boneWeights.end(); ++iter)
	{
		iter->second += diff;
	}
}

static Point4 PackBoneWeights(const map<int, float>& boneWeights)
{
	Point4 weights = {0.0f, 0.0f, 0.0f, 0.0f};

	if (boneWeights.size() == 0)
	{
		return weights;
	}

	int i = 0;

	for (auto iter = boneWeights.begin(); iter != boneWeights.end(); ++iter)
	{
		weights[i++] = (float)iter->first + iter->second;
	}

	int count = 4 - i;

	if (count > 0)
	{
		auto lastPair = prev(boneWeights.end());

		for (int j = i - 1; j < 4; j++)
		{
			weights[j] = (float)lastPair->first + lastPair->second / ((float)(count + 1));
		}
	}

	return weights;
}

static void GetNodeTM(INode* node, Point3& p, Quat& q, Point3& s, Interval interval = NEVER)
{
	Control* pTMController = node->GetTMController();

	if (pTMController == NULL)
	{
		return;
	}

	Matrix3 offsetTM;
	offsetTM.IdentityMatrix();
	offsetTM.PreTranslate(node->GetObjOffsetPos());
	PreRotateMatrix(offsetTM, node->GetObjOffsetRot());
	ApplyScaling(offsetTM, node->GetObjOffsetScale());

	Point3 pos = {0.0f, 0.0f, 0.0f};
	Quat rot = {0.0f, 0.0f, 0.0f, 0.0f};
	ScaleValue scale;
	scale.s = {1.0f, 1.0f, 1.0f};

	Control* pPositionController = pTMController->GetPositionController();

	if (pPositionController != NULL)
	{
		pPositionController->GetValue(0, &pos, interval, CTRL_ABSOLUTE);
	}

	Control* pRotationController = pTMController->GetRotationController();

	if (pRotationController != NULL)
	{
		pRotationController->GetValue(0, &rot, interval, CTRL_ABSOLUTE);
	}

	Control* pScaleController = pTMController->GetScaleController();

	if (pScaleController != NULL)
	{
		pScaleController->GetValue(0, &scale, interval, CTRL_ABSOLUTE);
	}

	Matrix3 localTM;
	localTM.IdentityMatrix();
	localTM.PreTranslate(pos);
	PreRotateMatrix(localTM, rot);
	ApplyScaling(localTM, scale);

	Matrix3 objectTM = offsetTM * localTM;

	DecomposeMatrix(objectTM, p, q, s);
}

void MeshExporter::ProcNode(INode* node)
{
	Control *pTMController = node->GetTMController();

	if (!pTMController)
	{
		return;
	}

	int currHeader = ID_NODE_HEADER;
	g_outputFile.write((char*)&currHeader, 2);

	ULONG id = node->GetHandle();
	g_outputFile.write((char*)&id, sizeof(ULONG));

	INode *pParent = node->GetParentNode();
	ULONG parentId = pParent ? pParent->GetHandle() : 0;
	g_outputFile.write((char*)&parentId, sizeof(ULONG));

	TriObject *triObj = GetTriObjFromNode(node);
	Modifier* skinModifier = GetSkinModifierFromNode(node);

	ISkinContextData* skinData = NULL;
	ISkin* iskin = NULL;

	if (skinModifier != NULL)
	{
		iskin = (ISkin*)skinModifier->GetInterface(I_SKIN);

		if (iskin != NULL)
		{
			skinData = iskin->GetContextInterface(node);
		}
	}

	int verts = 0;
	int faces = 0;

	if (triObj)
	{
		PrepareVerts(triObj, node, skinData);
		verts = (int)m_vertices.size();
		faces = (int)(m_indices.size() / 3);
	}

	g_outputFile.write((char*)&verts, sizeof(int));
	g_outputFile.write((char*)&faces, sizeof(int));

	Point3 pos = {0.0f, 0.0f, 0.0f};
	Quat rot = {0.0f, 0.0f, 0.0f, 0.0f};
	ScaleValue scale;
	scale.s = {1.0f, 1.0f, 1.0f};

	GetNodeTM(node, pos, rot, scale.s);

	// Write scale param for current Node
	g_outputFile.write((char*)&scale.s.x, sizeof(float));
	g_outputFile.write((char*)&scale.s.z, sizeof(float));
	g_outputFile.write((char*)&scale.s.y, sizeof(float));
	
	// Write rotation param for current Node (Transform to left-handed coordinate system)
	float x, y, z = 0;
	rot.GetEuler(&x, &y, &z);
	x = -x;
	y = -y;
	z = -z;

	g_outputFile.write((char*)&x, sizeof(float));
	g_outputFile.write((char*)&z, sizeof(float));
	g_outputFile.write((char*)&y, sizeof(float));

	// Write translation param
	g_outputFile.write((char*)&pos.x, sizeof(float));
	g_outputFile.write((char*)&pos.z, sizeof(float));
	g_outputFile.write((char*)&pos.y, sizeof(float));

	//Write tessellation factor
	int tessellationFactor = 0;
	node->GetUserPropInt(L"TessellationFactor", tessellationFactor);
	g_outputFile.write((char*)&tessellationFactor, sizeof(int));

	//Write damage level
	int damageLevel = 0;
	node->GetUserPropInt(L"DamageLevel", damageLevel);
	g_outputFile.write((char*)&damageLevel, sizeof(int));

	//Write isHidden
	int isHidden = node->IsNodeHidden();
	g_outputFile.write((char*)&isHidden, sizeof(int));

	// We don't know how long node record is. Save stream pos, and temporary fill 4 bytes by zeroes.
	const int zero = 0;
	int nextNode = g_outputFile.tellp();
	g_outputFile.write((char*)&zero, 4);

	// Write Material header
	currHeader = ID_MTL_HEADER;
	g_outputFile.write((char*)&currHeader, 2);

	int vertexPointer = g_outputFile.tellp();
	g_outputFile.write((char*)&zero, 4);

	// Export material associated with node
	if (triObj)
	{
		ExportMaterial(node);
	}

	// Write current pos as begin of Vertex section of our file
	int tmpPos = g_outputFile.tellp();
	g_outputFile.seekp(vertexPointer);
	g_outputFile.write((char*)&tmpPos, 4);
	g_outputFile.seekp(tmpPos);

	// Begin write geometry data, first of all dump 2 byte mesh header
	currHeader = ID_VERTEX_HEADER;
	g_outputFile.write((char*)&currHeader, 2);
	// Then save place for Face data pointer
	int facePointer = g_outputFile.tellp();
	g_outputFile.write((char*)&zero, 4);

	// Then pass through all faces and put into file all data: vertex coordinates, vertex mapping UV coordinates and normal vector
	if (triObj)
	{
		ExportVerts();
	}

	// Now save this pos as begin face block
	tmpPos = g_outputFile.tellp();
	g_outputFile.seekp(facePointer);
	g_outputFile.write((char*)&tmpPos, 4);
	g_outputFile.seekp(tmpPos);
	// Now export all Face data, first write Face data header
	currHeader = ID_FACE_HEADER;
	g_outputFile.write((char*)&currHeader, 2);

	// Write all facees data
	if (triObj)
	{
		ExportFaces();
	}

	//ExportAnimationKeys(node, iskin);

	// Now put this pos as begin of new node
	tmpPos = g_outputFile.tellp();
	g_outputFile.seekp(nextNode);

	g_outputFile.write((char*)&tmpPos, 4);
	g_outputFile.seekp(tmpPos);
	// Go to the next node if anyone exists
}

void MeshExporter::ExportStdMaterial(Mtl *material)
{
	// Get Ambient component
	Color color = material->GetAmbient(g_interfacePtr->GetTime());
	g_outputFile.write((char*)&color.r, sizeof(float));
	g_outputFile.write((char*)&color.g, sizeof(float));
	g_outputFile.write((char*)&color.b, sizeof(float));

	// Get Diffuse component
	color = material->GetDiffuse(g_interfacePtr->GetTime());
	g_outputFile.write((char*)&color.r, sizeof(float));
	g_outputFile.write((char*)&color.g, sizeof(float));
	g_outputFile.write((char*)&color.b, sizeof(float));

	// Get Specular component
	color = material->GetSpecular(g_interfacePtr->GetTime());
	g_outputFile.write((char*)&color.r, sizeof(float));
	g_outputFile.write((char*)&color.g, sizeof(float));
	g_outputFile.write((char*)&color.b, sizeof(float));

	// Get shininess
	float shininess = material->GetShininess(g_interfacePtr->GetTime());
	g_outputFile.write((char*)&shininess, sizeof(float));

	//Get Transparency
	float transparency = material->GetXParency();
	g_outputFile.write((char*)&transparency, sizeof(float));

	// Get Diffuse texture name
	Texmap* texmap = material->GetSubTexmap(ID_DI);
	int hdr = 0;

	if ((texmap == NULL) || (texmap->ClassID() != Class_ID(BMTEX_CLASS_ID, 0)))
	{
		g_outputFile.write((char*)&hdr, sizeof(int));
		return;
	}

	// If bitmap exists
	BitmapTex* bitmap = (BitmapTex*)texmap;
	TCHAR fileName[256] = {_T('\0')};
	_tcscpy(fileName, bitmap->GetMapName());

	for (int i = 0; i < 256; i++)
	{
		hdr = i;

		if (fileName[i] == '\0')
		{
			break;
		}
	}

	g_outputFile.write((char*)&hdr, sizeof(int));
	g_outputFile.write((char*)fileName, sizeof(TCHAR) * hdr);
}

void MeshExporter::ExportDefaultMaterial()
{
	int numMtls = 1;
	g_outputFile.write((char*)&numMtls, sizeof(int));

	Color color;
	color.r = color.g = color.b = 0.5f;
	g_outputFile.write((char*)&color.r, sizeof(float));
	g_outputFile.write((char*)&color.g, sizeof(float));
	g_outputFile.write((char*)&color.b, sizeof(float));

	g_outputFile.write((char*)&color.r, sizeof(float));
	g_outputFile.write((char*)&color.g, sizeof(float));
	g_outputFile.write((char*)&color.b, sizeof(float));

	g_outputFile.write((char*)&color.r, sizeof(float));
	g_outputFile.write((char*)&color.g, sizeof(float));
	g_outputFile.write((char*)&color.b, sizeof(float));

	float shininess = 0.0f;
	g_outputFile.write((char*)&shininess, sizeof(float));

	float transparency = 0.0f;
	g_outputFile.write((char*)&transparency, sizeof(float));

	int hdr = 0;
	g_outputFile.write((char*)&hdr, sizeof(int));
}

void MeshExporter::ExportAnimationKeys(INode* node, ISkin* iskin)
{
	if (iskin == NULL)
	{
		return;
	}

	for (int idx = 0; idx < iskin->GetNumBones(); idx++)
	{
		INode* boneNode = iskin->GetBone(idx);

		if (boneNode != NULL)
		{
			for (int i = 0; i < 100; i++)
			{
				Point3 pos = { 0.0f, 0.0f, 0.0f };
				Quat rot = { 0.0f, 0.0f, 0.0f, 0.0f };
				ScaleValue scale = { 0 };

				Interval interval;
				interval.SetStart(i);
				interval.SetEnd(i + 1);

				GetNodeTM(boneNode, pos, rot, scale.s, interval);
			}
		}
	}
}

void MeshExporter::ExportMaterial(INode* node)
{
	TriObject *triObj = GetTriObjFromNode(node);

	if (!triObj)
	{
		return;
	}

	Mtl* material = node->GetMtl();

	if (material == NULL)
	{
		ExportDefaultMaterial();

		return;
	}

	Interval v;
	material->Update(0, v);

	if (material->ClassID() == Class_ID(MULTI_CLASS_ID, 0))
	{		
		int numMtls = material->NumSubMtls();
		g_outputFile.write((char*)&numMtls, sizeof(int));

		for (int i = 0; i < numMtls; i++)
		{
			ExportStdMaterial(material->GetSubMtl(i));
		}
	}
	else
	{
		int numMtls = 1;
		g_outputFile.write((char*)&numMtls, sizeof(int));

		ExportStdMaterial(material);
	}
}

Point3 MeshExporter::GetVertexNormal(Mesh* mesh, int faceNo, RVertex* rv)
{
	Face* f = &mesh->faces[faceNo];
	DWORD smGroup = f->smGroup;
	int numNormals = 0;
	Point3 vertexNormal;
	
	// Is normal specified
	// SPCIFIED is not currently used, but may be used in future versions.
	if (rv->rFlags & SPECIFIED_NORMAL) 
	{
		vertexNormal = rv->rn.getNormal();
	}
	// If normal is not specified it's only available if the face belongs
	// to a smoothing group
	else if ((numNormals = rv->rFlags & NORCT_MASK) != 0 && smGroup) 
	{
		// If there is only one vertex is found in the rn member.
		if (numNormals == 1) 
		{
			vertexNormal = rv->rn.getNormal();
		}
		else 
		{
			// If two or more vertices are there you need to step through them
			// and find the vertex with the same smoothing group as the current face.
			// You will find multiple normals in the ern member.
			for (int i = 0; i < numNormals; i++) 
			{
				if (rv->ern[i].getSmGroup() & smGroup) 
				{
					vertexNormal = rv->ern[i].getNormal();
				}
			}
		}
	}
	else
	{
		// Get the normal from the Face if no smoothing groups are there
		vertexNormal = mesh->getFaceNormal(faceNo);
	}
	
	return vertexNormal.Normalize();
}

Point3 MeshExporter::GetVertexTangent(Mesh* mesh, int faceNo)
{
	Face* face = &mesh->faces[faceNo];

	Point3 v0 = mesh->verts[face->getVert(2)];
	Point3 v1 = mesh->verts[face->getVert(1)];
	Point3 v2 = mesh->verts[face->getVert(0)];
	Point3 uv0 = { 0.0f, 0.0f, 0.0f };
	Point3 uv1 = { 1.0f, 0.0f, 0.0f };
	Point3 uv2 = { 0.0f, 1.0f, 0.0f };

	if (mesh->numTVerts != 0)
	{
		uv0 = mesh->tVerts[mesh->tvFace[faceNo].t[2]];
		uv1 = mesh->tVerts[mesh->tvFace[faceNo].t[1]];
		uv2 = mesh->tVerts[mesh->tvFace[faceNo].t[0]];
	}
	uv0.y = 1.0f - uv0.y;
	uv1.y = 1.0f - uv1.y;
	uv2.y = 1.0f - uv2.y;

	Point3 deltaPos1 = v1 - v0;
	Point3 deltaPos2 = v2 - v0;

	Point3 deltaUV1 = uv1 - uv0;
	Point3 deltaUV2 = uv2 - uv0;

	float r = 1.0f / (deltaUV1.x * deltaUV2.y - deltaUV1.y * deltaUV2.x);
	Point3 tangent = (deltaPos1 * deltaUV2.y - deltaPos2 * deltaUV1.y) * r;

	return tangent.Normalize();
}

Point3 MeshExporter::GetVertexBitangent(Mesh* mesh, int faceNo)
{
	Face* face = &mesh->faces[faceNo];

	Point3 v0 = mesh->verts[face->getVert(2)];
	Point3 v1 = mesh->verts[face->getVert(1)];
	Point3 v2 = mesh->verts[face->getVert(0)];
	Point3 uv0 = { 0.0f, 0.0f, 0.0f };
	Point3 uv1 = { 1.0f, 0.0f, 0.0f };
	Point3 uv2 = { 0.0f, 1.0f, 0.0f };

	if (mesh->numTVerts != 0)
	{
		uv0 = mesh->tVerts[mesh->tvFace[faceNo].t[2]];
		uv1 = mesh->tVerts[mesh->tvFace[faceNo].t[1]];
		uv2 = mesh->tVerts[mesh->tvFace[faceNo].t[0]];
	}
	uv0.y = 1.0f - uv0.y;
	uv1.y = 1.0f - uv1.y;
	uv2.y = 1.0f - uv2.y;

	Point3 deltaPos1 = v1 - v0;
	Point3 deltaPos2 = v2 - v0;

	Point3 deltaUV1 = uv1 - uv0;
	Point3 deltaUV2 = uv2 - uv0;

	float r = 1.0f / (deltaUV1.x * deltaUV2.y - deltaUV1.y * deltaUV2.x);
	Point3 bitangent = (deltaPos2 * deltaUV1.x - deltaPos1 * deltaUV2.x) * r;

	return bitangent.Normalize();
}

int MeshExporter::GetEqualVertex(const Point3 &vp, const Point3 &vn, const Point3 &tc)
{
	for (auto iter = m_vertices.cbegin(); iter != m_vertices.cend(); ++iter)
	{
		if (vp.Equals((*iter).pos) && vn.Equals((*iter).normal) && tc.Equals((*iter).tex))
		{
			return distance(m_vertices.cbegin(), iter);
		}
	}

	return -1;
}

void MeshExporter::PrepareVerts(TriObject* triObj, INode* node, ISkinContextData* skinData)
{
	Point3 vp { 0.0f, 0.0f, 0.0f };			// Vertex position 
	Point3 vn { 0.0f, 0.0f, 0.0f };			// Vertex normal
	Point3 tangent { 0.0f, 0.0f, 0.0f };	// Vertex tangent
	Point3 bitangent { 0.0f, 0.0f, 0.0f };	// Vertex bitangent
	Point3 tc { 0.0f, 0.0f, 0.0f };			// Vertex texture coord

	triObj->mesh.buildNormals();

	Face* face;
	int vi, j, index = 0;
	Mesh* mesh = &triObj->mesh;

	m_vertices.clear();
	m_indices.clear();
	m_mtlsIndices.clear();

	map<int, float> boneWeights;

	// In MAX a vertex can have more than one normal (but doesn't always have it).
	// This is depending on the face you are accessing the vertex through.
	// To get all information we need to export all three vertex normals for every face.
	for (int i = 0; i < mesh->getNumFaces(); i++)
	{
		face = &mesh->faces[i];
			
		for (j = 0; j < 3; j++)
		{
			vi = face->getVert(2 - j);
			vp = mesh->getVert(vi);
			vn = GetVertexNormal(mesh, i, mesh->getRVertPtr(vi));
			tangent = GetVertexTangent(mesh, i);
			bitangent = GetVertexBitangent(mesh, i);

			if (mesh->numTVerts != 0)
			{
				DWORD ti = mesh->tvFace[i].t[2 - j];
				tc = mesh->tVerts[ti];
			}
			else
			{
				tc = { 0.0f, 0.0f, 0.0f };
			}

			int mi = 0;
			Mtl *material = node->GetMtl();
			if (material != NULL)
			{
				mi = face->getMatID();
			}

			boneWeights.clear();

			if (skinData != NULL)
			{
				for (int boneIdx = 0; boneIdx < skinData->GetNumAssignedBones(vi); boneIdx++)
				{
					int boneId = skinData->GetAssignedBone(vi, boneIdx);
					float weight = skinData->GetBoneWeight(vi, boneIdx);

					boneWeights[boneId] = weight;
				}

				TruncateBoneWeights(boneWeights);
			}

			Point4 weights = PackBoneWeights(boneWeights);

			index = GetEqualVertex(vp, vn, tc);

			if (index >= 0)
			{
				VertexPosition vertex = m_vertices[index];
				vertex.tangent += tangent;
				vertex.bitangent += bitangent;
				vertex.tangent.Normalize();
				vertex.bitangent.Normalize();
				vertex.boneWeights = weights;

				m_indices.push_back(index);
				m_mtlsIndices.push_back(mi);
			}
			else
			{
				VertexPosition vert;
				vert.pos = vp;
				vert.normal = vn;
				vert.tangent = tangent;
				vert.bitangent = bitangent;
				vert.tex = tc;
				vert.boneWeights = weights;

				m_vertices.push_back(vert);
				m_indices.push_back((DWORD)m_vertices.size() - 1);
				m_mtlsIndices.push_back(mi);
			}
		}
	}
}

void MeshExporter::ExportVerts()
{
	for (auto iter = m_vertices.cbegin(); iter != m_vertices.cend(); ++iter)
	{
		Point3 pos = (*iter).pos;
		g_outputFile.write((char*)&pos.x, sizeof(float));
		g_outputFile.write((char*)&pos.z, sizeof(float));
		g_outputFile.write((char*)&pos.y, sizeof(float));

		Point3 norm = (*iter).normal;
		g_outputFile.write((char*)&norm.x, sizeof(float));
		g_outputFile.write((char*)&norm.z, sizeof(float));
		g_outputFile.write((char*)&norm.y, sizeof(float));

		Point3 tangent = (*iter).tangent;
		g_outputFile.write((char*)&tangent.x, sizeof(float));
		g_outputFile.write((char*)&tangent.z, sizeof(float));
		g_outputFile.write((char*)&tangent.y, sizeof(float));

		Point3 bitangent = (*iter).bitangent;
		g_outputFile.write((char*)&bitangent.x, sizeof(float));
		g_outputFile.write((char*)&bitangent.z, sizeof(float));
		g_outputFile.write((char*)&bitangent.y, sizeof(float));

		Point3 tex = (*iter).tex;
		tex.y = 1.0f - tex.y;
		g_outputFile.write((char*)&tex.x, sizeof(float));
		g_outputFile.write((char*)&tex.y, sizeof(float));
	}
}

void MeshExporter::ExportFaces()
{
	for (auto iter = m_indices.cbegin(); iter != m_indices.cend(); ++iter)
	{
		DWORD index = *iter;
		g_outputFile.write((char*)&index, sizeof(DWORD));

		int i = distance(m_indices.cbegin(), iter);
		int mi = m_mtlsIndices[i];
		g_outputFile.write((char*)&mi, sizeof(int));
	}
}

TriObject* MeshExporter::GetTriObjFromNode(INode* node)
{
	Object *obj = node->EvalWorldState(g_interfacePtr->GetTime()).obj;
	TriObject *triObj = NULL;

	if (obj->CanConvertToType(Class_ID(TRIOBJ_CLASS_ID,0)))
	{
		triObj = (TriObject*)obj->ConvertToType(g_interfacePtr->GetTime(), Class_ID(TRIOBJ_CLASS_ID, 0));
	}

	return triObj;
}

Modifier* MeshExporter::GetSkinModifierFromNode(INode* node)
{
	Object* ObjectPtr = node->GetObjectRef();

	if (ObjectPtr == NULL)
	{
		return NULL;
	}

	while (ObjectPtr && ObjectPtr->SuperClassID() == GEN_DERIVOB_CLASS_ID)
	{
		IDerivedObject* DerivedObjectPtr = (IDerivedObject*)(ObjectPtr);
		int ModStackIndex = 0;

		while (ModStackIndex < DerivedObjectPtr->NumModifiers())
		{
			Modifier* ModifierPtr = DerivedObjectPtr->GetModifier(ModStackIndex);

			if (ModifierPtr->ClassID() == Class_ID(SKIN_CLASSID))
			{
				return ModifierPtr;
			}

			ModStackIndex++;
		}

		ObjectPtr = DerivedObjectPtr->GetObjRef();
	}

	return NULL;
}