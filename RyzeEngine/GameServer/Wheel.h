#ifndef Wheel_h
#define Wheel_h

#include "Vector.h"

typedef struct Wheel
{
    int *subMeshIds;
    
    int subMeshIdCount;
    
    int wheelId;
    
    float radius;
    
    float width;
    
    float suspensionRestLength;
    
	vector3_t axleCS;

	vector3_t wheelDirectionCS;

	vector3_t chassisConnectionPointCS;
    
    //rotation in model space
	quaternion_t rotation;
    
    float suspensionStiffness;

    float suspensionCompression;

    float suspensionDamping;

    float maxSuspensionTravelCm;

    float frictionSlip;

    //offset from geometry center
    float offset;
    
} wheel_t;//[104 bytes]

#endif /* Wheel_h */
