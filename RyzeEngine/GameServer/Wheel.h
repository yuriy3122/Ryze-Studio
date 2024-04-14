#ifndef Wheel_h
#define Wheel_h

#include "btBulletDynamicsCommon.h"

typedef struct Wheel
{
    int *subMeshIds;
    
    int subMeshIdCount;
    
    int wheelId;
    
    float radius;
    
    float width;
    
    float suspensionRestLength;
    
	btVector3 axleCS;

	btVector3 wheelDirectionCS;

	btVector3 chassisConnectionPointCS;
    
    //rotation in model space
    btQuaternion rotation;
    
    float suspensionStiffness;

    float suspensionCompression;

    float suspensionDamping;

    float maxSuspensionTravelCm;

    float frictionSlip;

    //offset from geometry center
    float offset;
    
} wheel_t;//[104 bytes]

#endif /* Wheel_h */
