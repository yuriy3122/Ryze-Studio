#ifndef SubMeshTransform_h
#define SubMeshTransform_h

#include "Vector.h"

typedef struct SubMeshTransform
{
	quaternion_t rotation;

	vector3_t position;

	int subMeshId;

} submesh_transform_t;

typedef struct SubMeshTransformList
{
	submesh_transform_t* data;

	int count;

} SubMeshTransformList;

#endif /* SubMeshTransform_h */

