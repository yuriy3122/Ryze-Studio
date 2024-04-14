#ifndef GameObject_h
#define GameObject_h

#include "Vector.h"

#pragma pack(push, 1)
typedef struct GameObject
{
	quaternion_t rotation;
    
	vector3_t scale;
    
	vector3_t position;
    
    long long ptr;
    
    int meshIdCount;
    
    int objectId;

	int meshId;
    
} game_object_t;
#pragma pack(pop)

typedef struct GameObjectList
{
    game_object_t **data;
    
    int count;
    
} GameObjectList;


#endif /* GameObject_h */
