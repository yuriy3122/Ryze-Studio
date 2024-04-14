#include "Wheel.h"

#ifndef Vehicle_h
#define Vehicle_h

typedef struct Vehicle
{
    int vehicleId;
    
    int gameObjectId;
    
    wheel_t *frontLeftSideWheel;
    
    wheel_t *frontRightSideWheel;
    
    wheel_t *rearLeftSideWheel;
    
    wheel_t *rearRightSideWheel;

    int chassisCollisionShapeId;
    
    float mass;
    
    float maxEngineForce;

    float maxBreakingForce;

    float steeringIncrement;

    float steeringClamp;
    
} vehicle_t;//72 bytes

#endif /* Vehicle_h */
