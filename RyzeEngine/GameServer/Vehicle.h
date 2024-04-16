#include "Wheel.h"
#include "GameObject.h"

#ifndef Vehicle_h
#define Vehicle_h

typedef struct Vehicle
{
	game_object_t* gameObject;

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

typedef struct VehicleList
{
	vehicle_t** data;

	int count;

} VehicleList;

#endif /* Vehicle_h */
