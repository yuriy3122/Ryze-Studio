using System;
using System.IO;
using RyzeEditor.GameWorld;
using RyzeEditor.Extentions;

namespace RyzeEditor.Packer
{
    public class VehicleWriter
    {
        private readonly WorldMapData _worldMapData;
        private readonly PackerOptions _options;

        private const ushort ID_VEHICLE_BLOCK_CHUNK = 0x0080;

        private int _wheelId = 0;
        private int _vehicleId = 0;

        public VehicleWriter(WorldMapData worldMapData, PackerOptions options)
        {
            _worldMapData = worldMapData;
            _options = options;
        }

        public void WriteData(Stream stream)
        {
            _wheelId = 1;
            _vehicleId = 1;

            foreach (var gameObject in _worldMapData.GameObjects)
            {
                var vehicle = gameObject.Key as Vehicle;

                if (vehicle == null)
                {
                    continue;
                }

                stream.Write(BitConverter.GetBytes(ID_VEHICLE_BLOCK_CHUNK), 0, sizeof(ushort));    //Chunk Header

                //0: Front Left Wheel
                foreach (var wheel in vehicle.Wheels)
                {
                    if (wheel.IsFrontWheel && wheel.IsLeftSideWheel)
                    {
                        WriteWheelData(stream, wheel);
                    }
                }

                //1: Front Right Wheel
                foreach (var wheel in vehicle.Wheels)
                {
                    if (wheel.IsFrontWheel && !wheel.IsLeftSideWheel)
                    {
                        WriteWheelData(stream, wheel);
                    }
                }

                //2: Rear Left Wheel
                foreach (var wheel in vehicle.Wheels)
                {
                    if (!wheel.IsFrontWheel && wheel.IsLeftSideWheel)
                    {
                        WriteWheelData(stream, wheel);
                    }
                }

                //3: Rear Right Wheel
                foreach (var wheel in vehicle.Wheels)
                {
                    if (!wheel.IsFrontWheel && !wheel.IsLeftSideWheel)
                    {
                        WriteWheelData(stream, wheel);
                    }
                }

                //Write Vehicle data struct (56 bytes)
                stream.Write(BitConverter.GetBytes(0L), 0, sizeof(long));                           //Reserve for 64-bit pointer
                stream.Write(BitConverter.GetBytes(_vehicleId++), 0, sizeof(int));                  //Vehicle Id
                stream.Write(BitConverter.GetBytes((int)vehicle.UserData), 0, sizeof(int));         //GameObject Id

                //Wheels pointers
                stream.Write(BitConverter.GetBytes(0L), 0, sizeof(long));                           //Reserve for 64-bit pointer, Front Left Wheel  [0]
                stream.Write(BitConverter.GetBytes(0L), 0, sizeof(long));                           //Reserve for 64-bit pointer, Front Right Wheel [1]
                stream.Write(BitConverter.GetBytes(0L), 0, sizeof(long));                           //Reserve for 64-bit pointer, Rear Left Wheel   [2]
                stream.Write(BitConverter.GetBytes(0L), 0, sizeof(long));                           //Reserve for 64-bit pointer, Rear Right Wheel  [3]

                stream.Write(BitConverter.GetBytes(vehicle.CollisionShapeId),  0, sizeof(int));
                stream.Write(BitConverter.GetBytes(vehicle.Mass),              0, sizeof(float));
                stream.Write(BitConverter.GetBytes(vehicle.MaxEngineForce),    0, sizeof(float));
                stream.Write(BitConverter.GetBytes(vehicle.MaxBreakingForce),  0, sizeof(float));
                stream.Write(BitConverter.GetBytes(vehicle.SteeringIncrement), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(vehicle.SteeringClamp),     0, sizeof(float));
            }
        }

        private void WriteWheelData(Stream stream, Wheel wheel)
        {
            stream.Write(BitConverter.GetBytes(wheel.SubMeshIds.Count), 0, sizeof(int));

            foreach (var subMeshId in wheel.SubMeshIds)
            {
                stream.Write(BitConverter.GetBytes(int.Parse(subMeshId)), 0, sizeof(int));
            }

            //Transform to World space
            var axleCS = wheel.AxleCS;
            axleCS.Normalize();
            axleCS.Z *= -1.0f;

            //Transform to World space
            var wheelDirectionCS = wheel.WheelDirectionCS;
            wheelDirectionCS.Normalize();
            wheelDirectionCS.Z *= -1.0f;

            //Transform to World space
            var chassisConnectionPointCS = wheel.ChassisConnectionPointCS;
            chassisConnectionPointCS.Z *= -1.0f;

            wheel.ComputeParams();

            //Write Wheel data struct (104 bytes)
            stream.Write(BitConverter.GetBytes(0L), 0, sizeof(long));                           //Reserve for 64-bit pointer
            stream.Write(BitConverter.GetBytes(wheel.SubMeshIds.Count), 0, sizeof(int));        //SubMeshIds count
            stream.Write(BitConverter.GetBytes(_wheelId++), 0, sizeof(int));                    //Wheel Id
            stream.Write(BitConverter.GetBytes(wheel.Radius), 0, sizeof(float));                //Radius
            stream.Write(BitConverter.GetBytes(wheel.Width), 0, sizeof(float));                 //Width
            stream.Write(BitConverter.GetBytes(wheel.SuspensionRestLength), 0, sizeof(float));  //SuspensionRestLength
            stream.Write(axleCS.GetBytes(), 0, 3 * sizeof(float));                              //AxleCS
            stream.Write(wheelDirectionCS.GetBytes(), 0, 3 * sizeof(float));                    //WheelDirectionCS
            stream.Write(chassisConnectionPointCS.GetBytes(), 0, 3 * sizeof(float));            //ChassicConnectionPoint
            stream.Write(wheel.Rotation.GetBytes(), 0, 4 * sizeof(float));                      //Rotation (Right-handed) in Model space
            stream.Write(BitConverter.GetBytes(wheel.SuspensionStiffness), 0, sizeof(float));
            stream.Write(BitConverter.GetBytes(wheel.SuspensionCompression), 0, sizeof(float));
            stream.Write(BitConverter.GetBytes(wheel.SuspensionDamping), 0, sizeof(float));
            stream.Write(BitConverter.GetBytes(wheel.MaxSuspensionTravelCm), 0, sizeof(float));
            stream.Write(BitConverter.GetBytes(wheel.FrictionSlip), 0, sizeof(float));
            stream.Write(BitConverter.GetBytes(wheel.Offset), 0, sizeof(float));                //Offset suspention center - geometry center
        }
    }
}
