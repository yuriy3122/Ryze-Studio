#ifndef RigidBody_h
#define RigidBody_h

typedef struct RigidBody
{
    int gameObjectId;
    
    int collisionShapeIndex;
    
    float invMass;
    
} rigid_body_t;

typedef struct RigidBodyList
{
	rigid_body_t** data;

	int count;

} RigidBodyList;

#endif /* RigidBody_h */
