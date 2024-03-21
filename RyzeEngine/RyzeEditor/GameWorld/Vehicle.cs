using System;
using System.Linq;
using System.Collections.Generic;
using SharpDX;
using PropertyChanged;
using RyzeEditor.ResourceManagment;

namespace RyzeEditor.GameWorld
{
    [Serializable]
    [ImplementPropertyChanged]
    public class Wheel : EntityBase
    {
        public Wheel()
        {
            Id = Guid.NewGuid();
            SuspensionStiffness = 5.88f;
            SuspensionCompression = 0.83f;
            SuspensionDamping = 0.88f;
            MaxSuspensionTravelCm = 500.0f;
            FrictionSlip = 10.5f;
            SubMeshIds = new List<string>();
        }

        [field: NonSerialized]
        public EventHandler SubMeshIdsChanged;

        [field: NonSerialized]
        public EventHandler SubmeshSelectionModeChanged;

        public string Name { get; set; }

        public bool IsFrontWheel { get; set; }

        public bool IsLeftSideWheel { get; set; }

        public float Radius { get; set; }

        public float Width { get; set; }

        //By default MeshId = MeshId from Chassis
        public string MeshId { get; set; }

        public List<string> SubMeshIds { get; set; }

        //The direction of the wheel's axle (world space) (= chassisTransform * m_wheelAxleCS)
        //The wheel rotates around this axis, example: (-1, 0, 0)
        public Vector3 AxleCS { get; set; }

        //The direction of ray cast (in world space) (= chassisTransform * m_wheelDirectionCS)
        //The wheel moves relative to the chassis in this direction, and the suspension force acts along this direction.
        //example: (0, -1, 0)
        public Vector3 WheelDirectionCS { get; set; }

        //The starting point of the ray, where the suspension connects to the chassis
        public Vector3 ChassisConnectionPointCS { get; set; }

        public Quaternion Rotation { get; set; }

        //The maximum length of the suspension (metres)
        //Rest length is the length of a spring when no force is applied to it.
        //If the suspension’s rest lengths are too large,
        //the chassis will seem to be jacked up on stilts and the vehicle will be prone to tipping, even when not moving.
        public float SuspensionRestLength { get; set; }

        //The stiffness constant for the suspension. 10.0 - Offroad buggy, 50.0 - Sports car, 200.0 - F1 Car
        //Stiffness is the force exerted by a spring divided by its change in length.
        //If the suspension is too stiff, a small bump could cause the vehicle to bounce violently.
        //If it isn’t stiff enough, a large bump could cause the chassis to "bottom out"
        public float SuspensionStiffness { get; set; }

        public float SuspensionCompression { get; set; }

        //Each wheel has 2 suspension damping parameters, one for expansion and one for compression.
        //The range of plausible values depends on the suspension stiffness, according to the formula:
        //damping = 2f * k * FastMath.sqrt(stiffness);
        //where k is the suspension’s damping ratio: k = 0: undamped and bouncy. k = 1: critically damped.
        //Good values of k are between 0.1 and 0.3.
        //The default damping parameters of 0.83 and 0.88 are suitable for a chassis with the default stiffness of 5.88 (k= 0.171 and 0.181, respectively).
        public float SuspensionDamping { get; set; }

        public float MaxSuspensionTravelCm { get; set; }

        //The coefficient of friction between the tyre and the ground.
        //Should be about 0.8 for realistic cars, but can increased for better handling.
        //Set large (10000.0) for kart racers
        //The friction slip parameter quantifies how much traction a tire has.
        //Its effect is most noticeable when the vehicle is braking.
        //Too much traction could cause a vehicle to flip over if it braked hard.
        //Too little traction would make braking ineffective, as if the tires were bald or the supporting surface were icy.
        //The default value for friction slip is 10.5
        public float FrictionSlip { get; set; }

        public float Offset { get; set; }

        public void ComputeParams()
        {
            if (string.IsNullOrEmpty(MeshId) || SubMeshIds == null || SubMeshIds.Count == 0)
            {
                return;
            }

            //Radius
            var mesh = ResourceManager.Instance.GetMesh(MeshId);
            var boundBox = mesh.GetBoundBox(SubMeshIds.Select(x => uint.Parse(x)).ToList());
            Radius = Math.Abs((boundBox.Maximum.Y - boundBox.Minimum.Y) / 2.0f);

            //Width
            var widthX = Math.Abs(boundBox.Maximum.X - boundBox.Minimum.X);
            var widthZ = Math.Abs(boundBox.Maximum.Z - boundBox.Minimum.Z);
            Width = Math.Min(widthX, widthZ);

            var subMeshId = uint.Parse(SubMeshIds.FirstOrDefault());

            //Rotation
            Rotation = mesh.GetRotationRH(subMeshId);

            //Offset
            var subMesh = mesh.SubMeshes.Where(x => x.Id == subMeshId).SingleOrDefault();
            var matrix = subMesh.GetMatrix(mesh);

            Vector4 pos = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            Vector4.Transform(ref pos, ref matrix, out Vector4 posT);
            posT.Z *= -1.0f;
            posT.Y = ChassisConnectionPointCS.Y;
            var position = new Vector3(posT.X, posT.Y, posT.Z);

            var connectionPoint = ChassisConnectionPointCS;
            connectionPoint.Z *= -1.0f;

            var offset = position - connectionPoint;
            float dir = Vector3.Dot(offset, AxleCS) > 0.0f ? -1.0f : 1.0f;

            Offset = dir * offset.Length();
        }
    }

    /// <summary>
    /// 1. Relaxing the steering angle to zero if the user does no hold down the left or right keys.
    /// 2. Reducing the maximum steering angle with increasing vehicle speed.
    /// 3. Setting engine force based on an analogue input, or alternatively based on the duration of the forward key being pressed down
    /// </summary>

    [Serializable]
    [ImplementPropertyChanged]
    public class Vehicle : GameObject
    {
        public Vehicle(List<string> geometryMeshIds, List<string> collisionMeshIds) : base(geometryMeshIds, collisionMeshIds)
        {
            Id = Guid.NewGuid();

            MaxEngineForce = 1000.0f;
            MaxBreakingForce = 100.0f;
            SteeringIncrement = 0.04f;
            SteeringClamp = 0.3f;

            var meshId = geometryMeshIds?.FirstOrDefault();

            Wheels = new List<Wheel>
            {
                new Wheel() { IsFrontWheel = true, IsLeftSideWheel = true, MeshId = meshId, ParentId = Id },
                new Wheel() { IsFrontWheel = true, IsLeftSideWheel = false, MeshId = meshId, ParentId = Id },
                new Wheel() { IsFrontWheel = false, IsLeftSideWheel = true, MeshId = meshId, ParentId = Id },
                new Wheel() { IsFrontWheel = false, IsLeftSideWheel = false, MeshId = meshId, ParentId = Id }
            };
        }

        public string GetChassisConvexHullMesh()
        {
            if (string.IsNullOrEmpty(ChassisMeshId))
            {
                return null;
            }

            var meshIds = ResourceManager.Instance.GetMeshIdList();
            var meshName = ChassisMeshId.Split('.').First() + "_chassis";
            var chassisConvexHullMeshId = meshIds.Where(x => x.Contains(meshName)).FirstOrDefault();

            return chassisConvexHullMeshId ?? ChassisMeshId;
        }

        public float Mass { get; set; }

        //Vehicle Mesh assumed to be a "Chassis"
        public string ChassisMeshId { get; set; }

        //For binary writer
        [InspectorVisible(false)]
        public int CollisionShapeId { get; set; }

        public List<Wheel> Wheels { get; set; }

        //This should be engine/velocity dependent
        public float MaxEngineForce { get; set; }

        public float MaxBreakingForce { get; set; }

        public float SteeringIncrement { get; set; }

        public float SteeringClamp { get; set; }

        public bool DrawChassisPoints { get; set; }
    }
}