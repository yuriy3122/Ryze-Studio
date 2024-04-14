#ifndef CollisionShape_h
#define CollisionShape_h

#include "Vector.h"

typedef struct CollisionShape
{
    int userIndex;
    
    int shapeType;
    
    int numberOfVertices;
    
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