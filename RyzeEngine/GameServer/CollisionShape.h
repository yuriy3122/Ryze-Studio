#ifndef CollisionShape_h
#define CollisionShape_h

#include "Vector.h"

#define ID_BOX_COLLISION_SHAPE              0
#define ID_CONVEX_HULL_COLLISION_SHAPE      4
#define ID_TERRAIN_COLLISION_SHAPE          24

typedef struct CollisionShape
{
    int userIndex;
    
    int shapeType;
    
    int numberOfVertices;

	int heightStickWidth;

	int heightStickLength;

	float gridSpacing;

	float minHeight;

	float maxHeight;
    
    void *vertexData;
    
	vector3_t boundBox;
    
	vector3_t center;
    
} collision_shape_t;

typedef struct CollisionShapeList
{
	collision_shape_t** data;

	int count;

} CollisionShapeList;

#endif /* CollisionShape_h */