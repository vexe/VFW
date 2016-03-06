using System;
using BX20Serializer;
using UnityEngine;
using Vexe.FastSave.Serializers;

namespace Vexe.FastSave
{
    public static class FSCommon
    {
        public static readonly BinaryX20 Serializer;

        static FSCommon()
        {
            BinaryX20.Log = Debug.Log;

            X20Logic.SerializeMemberAttributes = new Type[] { typeof(SaveAttribute) };
            X20Logic.DontSerializeMemberAttributes = new Type[] { typeof(DontSaveAttribute) };

            Serializer = new BinaryX20();

            var ecs = new ExplicitComponentSerializer();
            ecs.Add<Transform>("position", "eulerAngles", "localScale")
               .Add<Rigidbody>("mass", "drag", "isKinematic", "useGravity", "angularDrag", "constraints", "interpolation", "collisionDetectionMode")
               .Add<MeshFilter>("sharedMesh")
               .Add<MeshRenderer>("shadowCastingMode", "receiveShadows", "sharedMaterials", "useLightProbes", "reflectionProbeUsage", "probeAnchor")
               .Add<SkinnedMeshRenderer>("shadowCastingMode", "receiveShadows", "sharedMaterials", "useLightProbes", "reflectionProbeUsage", "probeAnchor", "rootBone", "quality", "localBounds", "sharedMesh", "updateWhenOffscreen")
               .Add<TrailRenderer>("shadowCastingMode", "receiveShadows", "sharedMaterials", "useLightProbes", "reflectionProbeUsage", "probeAnchor", "time", "startWidth", "endWidth", "autodestruct")
               .Add<ParticleRenderer>("shadowCastingMode", "receiveShadows", "sharedMaterials", "useLightProbes", "reflectionProbeUsage", "probeAnchor", "cameraVelocityScale", "particleRenderMode", "lengthScale", "velocityScale", "maxParticleSize", "uvAnimationCycles", "uvAnimationXTile", "uvAnimationYTile", "uvTiles")
               .Add<ParticleAnimator>("doesAnimateColor", "colorAnimation", "worldRotationAxis", "localRotationAxis", "rndForce", "force", "sizeGrow", "damping", "autodestruct")
               .Add<Camera>("fieldOfView", "orthographicSize", "orthographic", "nearClipPlane", "farClipPlane", "backgroundColor", "clearFlags", "rect", "depth", "renderingPath", "hdr", "targetTexture")
               .Add<CharacterController>("stepOffset", "radius", "height", "center")
               .Add<AudioSource>("clip", "volume", "pitch", "time", "mute", "playOnAwake", "loop")
               .Add<Animator>("runtimeAnimatorController", "applyRootMotion")
               .Add<BoxCollider>("isTrigger", "center", "size", "sharedMaterial")
               .Add<CapsuleCollider>("isTrigger", "center", "radius", "height", "direction", "sharedMaterial")
               .Add<MeshCollider>("isTrigger", "convex", "sharedMaterial", "sharedMesh")
               .Add<SphereCollider>("isTrigger", "radius", "center", "sharedMaterial")
               .Add<BoxCollider2D>("isTrigger", "sharedMaterial", "usedByEffector", "offset", "size")
               .Add<CircleCollider2D>("isTrigger", "sharedMaterial", "usedByEffector", "offset", "radius")
               .Add<PolygonCollider2D>("isTrigger", "sharedMaterial", "usedByEffector", "offset", "points", "pathCount")
               .Add<EdgeCollider2D>("isTrigger", "sharedMaterial", "usedByEffector", "offset", "points")
               .Add<Rigidbody2D>("mass", "drag", "angularDrag", "angularVelocity", "gravityScale", "fixedAngle", "isKinematic", "interpolation", "sleepMode", "collisionDetectionMode")
               .Add<ConstantForce2D>("force", "relativeForce", "torque")
               .Add<AreaEffector2D>("colliderMask", "forceDirection", "forceMagnitude", "forceVariation", "drag", "angularDrag", "forceTarget")
               .Add<PlatformEffector2D>("colliderMask", "oneWay", "sideFriction", "sideBounce", "sideAngleVariance")
               .Add<PointEffector2D>("colliderMask", "forceMagnitude", "forceVariation", "distanceScale", "drag", "angularDrag", "forceSource", "forceTarget", "forceMode")
               .Add<SurfaceEffector2D>("colliderMask", "speed", "speedVariation")
               .Add<DistanceJoint2D>("collideConnected", "connectedBody", "connectedAnchor", "anchor", "distance", "maxDistanceOnly")
               .Add<HingeJoint2D>("collideConnected", "connectedBody", "connectedAnchor", "anchor", "useMotor", "motor", "useLimits", "limits")
               .Add<SliderJoint2D>("collideConnected", "connectedBody", "connectedAnchor", "anchor", "angle", "useMotor", "motor", "useLimits", "limits")
               .Add<SpringJoint2D>("collideConnected", "connectedBody", "connectedAnchor", "anchor", "distance", "dampingRatio", "frequency")
               .Add<WheelJoint2D>("collideConnected", "connectedBody", "connectedAnchor", "anchor", "suspension", "useMotor", "motor");

            Serializer.AddSerializer(new AssetReferenceSerializer())
                      .AddSerializer(new ReflectiveComponentSerializer())
                   	  .AddSerializer(ecs);
        }
    }
}
