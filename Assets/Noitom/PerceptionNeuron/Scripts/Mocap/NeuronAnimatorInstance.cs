/************************************************************************************
 Copyright: Copyright 2014 Beijing Noitom Technology Ltd. All Rights reserved.
 Pending Patents: PCT/CN2014/085659 PCT/CN2014/071006

 Licensed under the Perception Neuron SDK License Beta Version (the “License");
 You may only use the Perception Neuron SDK when in compliance with the License,
 which is provided at the time of installation or download, or which
 otherwise accompanies this software in the form of either an electronic or a hard copy.

 A copy of the License is included with this package or can be obtained at:
 http://www.neuronmocap.com

 Unless required by applicable law or agreed to in writing, the Perception Neuron SDK
 distributed under the License is provided on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing conditions and
 limitations under the License.
************************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using Neuron;

public class NeuronAnimatorInstance : NeuronInstance
{
    public bool useNewRig = true;
    public Animator boundAnimator = null;
    public UpdateMethod motionUpdateMethod = UpdateMethod.Normal;
    public bool enableHipsMovement = true;

    public Animator physicalReferenceOverride; //use an already existing NeuronAnimatorInstance as the physical reference
    public NeuronAnimatorPhysicalReference physicalReference = new NeuronAnimatorPhysicalReference();
    Vector3[] bonePositionOffsets = new Vector3[(int)HumanBodyBones.LastBone];
    Vector3[] boneRotationOffsets = new Vector3[(int)HumanBodyBones.LastBone];

    public float velocityMagic = 3000.0f;
    public float angularVelocityMagic = 20.0f;
    Quaternion[] orignalRot = new Quaternion[(int)HumanBodyBones.LastBone];
    Quaternion[] orignalParentRot = new Quaternion[(int)HumanBodyBones.LastBone];

    [Range(0.0f, 1.0f)]
    public float pers = 1.0f;

    public Animator PauseAnimator;

    public NeuronAnimatorInstance()
    {
    }

    public NeuronAnimatorInstance(string address, int port, int commandServerPort, NeuronConnection.SocketType socketType, int actorID)
        : base(address, port, commandServerPort, socketType, actorID)
    {
    }

    public NeuronAnimatorInstance(Animator animator, string address, int port, int commandServerPort, NeuronConnection.SocketType socketType, int actorID)
        : base(address, port, commandServerPort, socketType, actorID)
    {
        boundAnimator = animator;
        UpdateOffset();
    }

    public NeuronAnimatorInstance(Animator animator, NeuronActor actor)
        : base(actor)
    {
        boundAnimator = animator;
        UpdateOffset();
    }

    public NeuronAnimatorInstance(NeuronActor actor)
        : base(actor)
    {
    }

    bool inited = false;
    new void OnEnable()
    {
        if (inited)
        {
            return;
        }
        inited = true;
        base.OnEnable();
        if (boundAnimator == null)
        {
            boundAnimator = GetComponent<Animator>();
        }
        UpdateOffset();
        CaluateOrignalRot();
    }

    new void Update()
    {
        base.ToggleConnect();
        base.Update();

        if (boundActor != null && boundAnimator != null && motionUpdateMethod == UpdateMethod.Normal) // !physicalUpdate )
        {
            if (physicalReference.Initiated())
            {
                ReleasePhysicalContext();
            }

            ApplyMotion(boundActor, boundAnimator, bonePositionOffsets, boneRotationOffsets);
        }
    }

    void FixedUpdate()
    {
        base.ToggleConnect();

        if (boundActor != null && boundAnimator != null && motionUpdateMethod != UpdateMethod.Normal) // && physicalUpdate )
        {
            if (!physicalReference.Initiated())
            {
                //physicalUpdate = InitPhysicalContext ();
                InitPhysicalContext();
            }

            ApplyMotionPhysically(physicalReference.GetReferenceAnimator(), boundAnimator);
        }
    }

    bool ValidateVector3(Vector3 vec)
    {
        return !float.IsNaN(vec.x) && !float.IsNaN(vec.y) && !float.IsNaN(vec.z)
            && !float.IsInfinity(vec.x) && !float.IsInfinity(vec.y) && !float.IsInfinity(vec.z);
    }

    void SetScale(Animator animator, HumanBodyBones bone, float size, float referenceSize)
    {
        Transform t = animator.GetBoneTransform(bone);
        Transform pauset = PauseAnimator.GetBoneTransform(bone);
        if (t != null && bone <= HumanBodyBones.Jaw)
        {
            float ratio = size / referenceSize;

            Vector3 newScale = new Vector3(ratio, ratio, ratio);
            newScale.Scale(new Vector3(1.0f / t.parent.lossyScale.x, 1.0f / t.parent.lossyScale.y, 1.0f / t.parent.lossyScale.z));

            if (ValidateVector3(newScale))
            {
                //t.localScale = newScale;
                //t.localScale = pauset.localScale;
                t.localScale = Vector3.Lerp(newScale, pauset.localScale, pers);
            }
        }
    }

    // set position for bone in animator
    void SetPosition(Animator animator, HumanBodyBones bone, Vector3 pos)
    {
        Transform t = animator.GetBoneTransform(bone);
        Transform pauset = PauseAnimator.GetBoneTransform(bone);
        if (t != null)
        {
            if (!float.IsNaN(pos.x) && !float.IsNaN(pos.y) && !float.IsNaN(pos.z))
            {
                //	t.localPosition = pos;

                Vector3 srcP = pos;
                Vector3 finalP = Quaternion.Inverse(orignalParentRot[(int)bone]) * srcP;
                //t.localPosition = finalP;
                t.localPosition = Vector3.Lerp(finalP, pauset.localPosition, pers);
            }
        }
    }

    // set rotation for bone in animator
    void SetRotation(Animator animator, HumanBodyBones bone, Vector3 rotation)
    {
        Transform t = animator.GetBoneTransform(bone);
        Transform pauset = PauseAnimator.GetBoneTransform(bone);
        if (t != null)
        {
            Quaternion rot = Quaternion.Euler(rotation);
            if (!float.IsNaN(rot.x) && !float.IsNaN(rot.y) && !float.IsNaN(rot.z) && !float.IsNaN(rot.w))
            {
                //t.localRotation = rot;

                Quaternion orignalBoneRot = Quaternion.identity;
                if (orignalRot != null)
                {
                    orignalBoneRot = orignalRot[(int)bone];
                }
                Quaternion srcQ = rot;

                Quaternion usedQ = Quaternion.Inverse(orignalParentRot[(int)bone]) * srcQ * orignalParentRot[(int)bone];
                Vector3 transedRot = usedQ.eulerAngles;
                Quaternion finalBoneQ = Quaternion.Euler(transedRot) * orignalBoneRot;
                //t.localRotation = finalBoneQ;
                t.localRotation = Quaternion.Lerp(finalBoneQ, pauset.localRotation, pers);
            }
        }
    }

    // apply transforms extracted from actor mocap data to transforms of animator bones
    public void ApplyMotion(NeuronActor actor, Animator animator, Vector3[] positionOffsets, Vector3[] rotationOffsets)
    {
        // apply Hips position
        if (enableHipsMovement)
        {
            SetPosition(animator, HumanBodyBones.Hips, actor.GetReceivedPosition(NeuronBones.Hips) + positionOffsets[(int)HumanBodyBones.Hips]);
            SetRotation(animator, HumanBodyBones.Hips, actor.GetReceivedRotation(NeuronBones.Hips));
        }

        // apply positions
        if (actor.withDisplacement)
        {
            // legs
            SetPosition(animator, HumanBodyBones.RightUpperLeg, actor.GetReceivedPosition(NeuronBones.RightUpLeg) + positionOffsets[(int)HumanBodyBones.RightUpperLeg]);
            SetPosition(animator, HumanBodyBones.RightLowerLeg, actor.GetReceivedPosition(NeuronBones.RightLeg));
            SetPosition(animator, HumanBodyBones.RightFoot, actor.GetReceivedPosition(NeuronBones.RightFoot));
            SetPosition(animator, HumanBodyBones.LeftUpperLeg, actor.GetReceivedPosition(NeuronBones.LeftUpLeg) + positionOffsets[(int)HumanBodyBones.LeftUpperLeg]);
            SetPosition(animator, HumanBodyBones.LeftLowerLeg, actor.GetReceivedPosition(NeuronBones.LeftLeg));
            SetPosition(animator, HumanBodyBones.LeftFoot, actor.GetReceivedPosition(NeuronBones.LeftFoot));

            // spine
            SetPosition(animator, HumanBodyBones.Spine, actor.GetReceivedPosition(NeuronBones.Spine));
            if (useNewRig)
            {
                SetPosition(animator, HumanBodyBones.Chest,
                     actor.GetReceivedPosition(NeuronBones.Spine1)
                     );
#if UNITY_2018_2_OR_NEWER
                SetPosition(animator, HumanBodyBones.UpperChest,
                     actor.GetReceivedPosition(NeuronBones.Spine2)
                     );
#endif
                SetPosition(animator, HumanBodyBones.Neck,
                     actor.GetReceivedPosition((NeuronBones)NeuronBonesV2.Neck) +
                     (EulerToQuaternion(actor.GetReceivedRotation((NeuronBones)NeuronBonesV2.Neck)) * actor.GetReceivedPosition((NeuronBones)NeuronBonesV2.Neck1))
                    );
            }
            else
            {
                SetPosition(animator, HumanBodyBones.Chest, actor.GetReceivedPosition(NeuronBones.Spine3));
                SetPosition(animator, HumanBodyBones.Neck, actor.GetReceivedPosition(NeuronBones.Neck));
            }
            SetPosition(animator, HumanBodyBones.Head, actor.GetReceivedPosition(NeuronBones.Head));

            // right arm
            SetPosition(animator, HumanBodyBones.RightShoulder, actor.GetReceivedPosition(NeuronBones.RightShoulder));
            SetPosition(animator, HumanBodyBones.RightUpperArm, actor.GetReceivedPosition(NeuronBones.RightArm));
            SetPosition(animator, HumanBodyBones.RightLowerArm, actor.GetReceivedPosition(NeuronBones.RightForeArm));

            // right hand
            SetPosition(animator, HumanBodyBones.RightHand, actor.GetReceivedPosition(NeuronBones.RightHand));
            SetPosition(animator, HumanBodyBones.RightThumbProximal, actor.GetReceivedPosition(NeuronBones.RightHandThumb1));
            SetPosition(animator, HumanBodyBones.RightThumbIntermediate, actor.GetReceivedPosition(NeuronBones.RightHandThumb2));
            SetPosition(animator, HumanBodyBones.RightThumbDistal, actor.GetReceivedPosition(NeuronBones.RightHandThumb3));

            SetPosition(animator, HumanBodyBones.RightIndexProximal, actor.GetReceivedPosition(NeuronBones.RightHandIndex1));
            SetPosition(animator, HumanBodyBones.RightIndexIntermediate, actor.GetReceivedPosition(NeuronBones.RightHandIndex2));
            SetPosition(animator, HumanBodyBones.RightIndexDistal, actor.GetReceivedPosition(NeuronBones.RightHandIndex3));

            SetPosition(animator, HumanBodyBones.RightMiddleProximal, actor.GetReceivedPosition(NeuronBones.RightHandMiddle1));
            SetPosition(animator, HumanBodyBones.RightMiddleIntermediate, actor.GetReceivedPosition(NeuronBones.RightHandMiddle2));
            SetPosition(animator, HumanBodyBones.RightMiddleDistal, actor.GetReceivedPosition(NeuronBones.RightHandMiddle3));

            SetPosition(animator, HumanBodyBones.RightRingProximal, actor.GetReceivedPosition(NeuronBones.RightHandRing1));
            SetPosition(animator, HumanBodyBones.RightRingIntermediate, actor.GetReceivedPosition(NeuronBones.RightHandRing2));
            SetPosition(animator, HumanBodyBones.RightRingDistal, actor.GetReceivedPosition(NeuronBones.RightHandRing3));

            SetPosition(animator, HumanBodyBones.RightLittleProximal, actor.GetReceivedPosition(NeuronBones.RightHandPinky1));
            SetPosition(animator, HumanBodyBones.RightLittleIntermediate, actor.GetReceivedPosition(NeuronBones.RightHandPinky2));
            SetPosition(animator, HumanBodyBones.RightLittleDistal, actor.GetReceivedPosition(NeuronBones.RightHandPinky3));

            // left arm
            SetPosition(animator, HumanBodyBones.LeftShoulder, actor.GetReceivedPosition(NeuronBones.LeftShoulder));
            SetPosition(animator, HumanBodyBones.LeftUpperArm, actor.GetReceivedPosition(NeuronBones.LeftArm));
            SetPosition(animator, HumanBodyBones.LeftLowerArm, actor.GetReceivedPosition(NeuronBones.LeftForeArm));

            // left hand
            SetPosition(animator, HumanBodyBones.LeftHand, actor.GetReceivedPosition(NeuronBones.LeftHand));
            SetPosition(animator, HumanBodyBones.LeftThumbProximal, actor.GetReceivedPosition(NeuronBones.LeftHandThumb1));
            SetPosition(animator, HumanBodyBones.LeftThumbIntermediate, actor.GetReceivedPosition(NeuronBones.LeftHandThumb2));
            SetPosition(animator, HumanBodyBones.LeftThumbDistal, actor.GetReceivedPosition(NeuronBones.LeftHandThumb3));

            SetPosition(animator, HumanBodyBones.LeftIndexProximal, actor.GetReceivedPosition(NeuronBones.LeftHandIndex1));
            SetPosition(animator, HumanBodyBones.LeftIndexIntermediate, actor.GetReceivedPosition(NeuronBones.LeftHandIndex2));
            SetPosition(animator, HumanBodyBones.LeftIndexDistal, actor.GetReceivedPosition(NeuronBones.LeftHandIndex3));

            SetPosition(animator, HumanBodyBones.LeftMiddleProximal, actor.GetReceivedPosition(NeuronBones.LeftHandMiddle1));
            SetPosition(animator, HumanBodyBones.LeftMiddleIntermediate, actor.GetReceivedPosition(NeuronBones.LeftHandMiddle2));
            SetPosition(animator, HumanBodyBones.LeftMiddleDistal, actor.GetReceivedPosition(NeuronBones.LeftHandMiddle3));

            SetPosition(animator, HumanBodyBones.LeftRingProximal, actor.GetReceivedPosition(NeuronBones.LeftHandRing1));
            SetPosition(animator, HumanBodyBones.LeftRingIntermediate, actor.GetReceivedPosition(NeuronBones.LeftHandRing2));
            SetPosition(animator, HumanBodyBones.LeftRingDistal, actor.GetReceivedPosition(NeuronBones.LeftHandRing3));

            SetPosition(animator, HumanBodyBones.LeftLittleProximal, actor.GetReceivedPosition(NeuronBones.LeftHandPinky1));
            SetPosition(animator, HumanBodyBones.LeftLittleIntermediate, actor.GetReceivedPosition(NeuronBones.LeftHandPinky2));
            SetPosition(animator, HumanBodyBones.LeftLittleDistal, actor.GetReceivedPosition(NeuronBones.LeftHandPinky3));
        }

        // apply rotations

        // legs
        SetRotation(animator, HumanBodyBones.RightUpperLeg, actor.GetReceivedRotation(NeuronBones.RightUpLeg));
        SetRotation(animator, HumanBodyBones.RightLowerLeg, actor.GetReceivedRotation(NeuronBones.RightLeg));
        SetRotation(animator, HumanBodyBones.RightFoot, actor.GetReceivedRotation(NeuronBones.RightFoot));
        SetRotation(animator, HumanBodyBones.LeftUpperLeg, actor.GetReceivedRotation(NeuronBones.LeftUpLeg));
        SetRotation(animator, HumanBodyBones.LeftLowerLeg, actor.GetReceivedRotation(NeuronBones.LeftLeg));
        SetRotation(animator, HumanBodyBones.LeftFoot, actor.GetReceivedRotation(NeuronBones.LeftFoot));

        // spine
        SetRotation(animator, HumanBodyBones.Spine, actor.GetReceivedRotation(NeuronBones.Spine));
        //SetRotation( animator, HumanBodyBones.Chest,					actor.GetReceivedRotation( NeuronBones.Spine1 ) + actor.GetReceivedRotation( NeuronBones.Spine2 ) + actor.GetReceivedRotation( NeuronBones.Spine3 ) ); 
        if (useNewRig)
        {
            SetRotation(animator, HumanBodyBones.Chest,
                (EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.Spine1)) *
                EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.Spine2))).eulerAngles
                );

            SetRotation(animator, HumanBodyBones.Neck,
                (EulerToQuaternion(actor.GetReceivedRotation((NeuronBones)NeuronBonesV2.Neck)) *
                EulerToQuaternion(actor.GetReceivedRotation((NeuronBones)NeuronBonesV2.Neck1))).eulerAngles
                );
        }
        else
        {
            SetRotation(animator, HumanBodyBones.Chest,
                (EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.Spine1)) *
                EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.Spine2)) *
                EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.Spine3))).eulerAngles
                );
            SetRotation(animator, HumanBodyBones.Neck, actor.GetReceivedRotation(NeuronBones.Neck));
        }

        SetRotation(animator, HumanBodyBones.Head, actor.GetReceivedRotation(NeuronBones.Head));

        // right arm
        SetRotation(animator, HumanBodyBones.RightShoulder, actor.GetReceivedRotation(NeuronBones.RightShoulder));
        SetRotation(animator, HumanBodyBones.RightUpperArm, actor.GetReceivedRotation(NeuronBones.RightArm));
        SetRotation(animator, HumanBodyBones.RightLowerArm, actor.GetReceivedRotation(NeuronBones.RightForeArm));

        // right hand
        SetRotation(animator, HumanBodyBones.RightHand, actor.GetReceivedRotation(NeuronBones.RightHand));
        SetRotation(animator, HumanBodyBones.RightThumbProximal, actor.GetReceivedRotation(NeuronBones.RightHandThumb1));
        SetRotation(animator, HumanBodyBones.RightThumbIntermediate, actor.GetReceivedRotation(NeuronBones.RightHandThumb2));
        SetRotation(animator, HumanBodyBones.RightThumbDistal, actor.GetReceivedRotation(NeuronBones.RightHandThumb3));

        //SetRotation( animator, HumanBodyBones.RightIndexProximal,		actor.GetReceivedRotation( NeuronBones.RightHandIndex1 ) + actor.GetReceivedRotation( NeuronBones.RightInHandIndex ) );
        SetRotation(animator, HumanBodyBones.RightIndexProximal,
            (EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.RightInHandIndex)) *
             EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.RightHandIndex1))).eulerAngles
            );
        SetRotation(animator, HumanBodyBones.RightIndexIntermediate, actor.GetReceivedRotation(NeuronBones.RightHandIndex2));
        SetRotation(animator, HumanBodyBones.RightIndexDistal, actor.GetReceivedRotation(NeuronBones.RightHandIndex3));

        //SetRotation( animator, HumanBodyBones.RightMiddleProximal,		actor.GetReceivedRotation( NeuronBones.RightHandMiddle1 ) + actor.GetReceivedRotation( NeuronBones.RightInHandMiddle ) );
        SetRotation(animator, HumanBodyBones.RightMiddleProximal,
            (EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.RightInHandMiddle)) *
             EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.RightHandMiddle1))).eulerAngles
            );
        SetRotation(animator, HumanBodyBones.RightMiddleIntermediate, actor.GetReceivedRotation(NeuronBones.RightHandMiddle2));
        SetRotation(animator, HumanBodyBones.RightMiddleDistal, actor.GetReceivedRotation(NeuronBones.RightHandMiddle3));

        //SetRotation( animator, HumanBodyBones.RightRingProximal,		actor.GetReceivedRotation( NeuronBones.RightHandRing1 ) + actor.GetReceivedRotation( NeuronBones.RightInHandRing ) );
        SetRotation(animator, HumanBodyBones.RightRingProximal,
            (EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.RightInHandRing)) *
             EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.RightHandRing1))).eulerAngles
            );
        SetRotation(animator, HumanBodyBones.RightRingIntermediate, actor.GetReceivedRotation(NeuronBones.RightHandRing2));
        SetRotation(animator, HumanBodyBones.RightRingDistal, actor.GetReceivedRotation(NeuronBones.RightHandRing3));

        //SetRotation( animator, HumanBodyBones.RightLittleProximal,		actor.GetReceivedRotation( NeuronBones.RightHandPinky1 ) + actor.GetReceivedRotation( NeuronBones.RightInHandPinky ) );
        SetRotation(animator, HumanBodyBones.RightLittleProximal,
            (EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.RightInHandPinky)) *
             EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.RightHandPinky1))).eulerAngles
            );
        SetRotation(animator, HumanBodyBones.RightLittleIntermediate, actor.GetReceivedRotation(NeuronBones.RightHandPinky2));
        SetRotation(animator, HumanBodyBones.RightLittleDistal, actor.GetReceivedRotation(NeuronBones.RightHandPinky3));

        // left arm
        SetRotation(animator, HumanBodyBones.LeftShoulder, actor.GetReceivedRotation(NeuronBones.LeftShoulder));
        SetRotation(animator, HumanBodyBones.LeftUpperArm, actor.GetReceivedRotation(NeuronBones.LeftArm));
        SetRotation(animator, HumanBodyBones.LeftLowerArm, actor.GetReceivedRotation(NeuronBones.LeftForeArm));

        // left hand
        SetRotation(animator, HumanBodyBones.LeftHand, actor.GetReceivedRotation(NeuronBones.LeftHand));
        SetRotation(animator, HumanBodyBones.LeftThumbProximal, actor.GetReceivedRotation(NeuronBones.LeftHandThumb1));
        SetRotation(animator, HumanBodyBones.LeftThumbIntermediate, actor.GetReceivedRotation(NeuronBones.LeftHandThumb2));
        SetRotation(animator, HumanBodyBones.LeftThumbDistal, actor.GetReceivedRotation(NeuronBones.LeftHandThumb3));

        //SetRotation( animator, HumanBodyBones.LeftIndexProximal,		actor.GetReceivedRotation( NeuronBones.LeftHandIndex1 ) + actor.GetReceivedRotation( NeuronBones.LeftInHandIndex ) );
        SetRotation(animator, HumanBodyBones.LeftIndexProximal,
            (EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.LeftInHandIndex)) *
             EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.LeftHandIndex1))).eulerAngles
            );
        SetRotation(animator, HumanBodyBones.LeftIndexIntermediate, actor.GetReceivedRotation(NeuronBones.LeftHandIndex2));
        SetRotation(animator, HumanBodyBones.LeftIndexDistal, actor.GetReceivedRotation(NeuronBones.LeftHandIndex3));

        //SetRotation( animator, HumanBodyBones.LeftMiddleProximal,		actor.GetReceivedRotation( NeuronBones.LeftHandMiddle1 ) + actor.GetReceivedRotation( NeuronBones.LeftInHandMiddle ) );
        SetRotation(animator, HumanBodyBones.LeftMiddleProximal,
            (EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.LeftInHandMiddle)) *
             EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.LeftHandMiddle1))).eulerAngles
            );
        SetRotation(animator, HumanBodyBones.LeftMiddleIntermediate, actor.GetReceivedRotation(NeuronBones.LeftHandMiddle2));
        SetRotation(animator, HumanBodyBones.LeftMiddleDistal, actor.GetReceivedRotation(NeuronBones.LeftHandMiddle3));

        //SetRotation( animator, HumanBodyBones.LeftRingProximal,			actor.GetReceivedRotation( NeuronBones.LeftHandRing1 ) + actor.GetReceivedRotation( NeuronBones.LeftInHandRing ) );
        SetRotation(animator, HumanBodyBones.LeftRingProximal,
            (EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.LeftInHandRing)) *
             EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.LeftHandRing1))).eulerAngles
            );
        SetRotation(animator, HumanBodyBones.LeftRingIntermediate, actor.GetReceivedRotation(NeuronBones.LeftHandRing2));
        SetRotation(animator, HumanBodyBones.LeftRingDistal, actor.GetReceivedRotation(NeuronBones.LeftHandRing3));

        //SetRotation( animator, HumanBodyBones.LeftLittleProximal,		actor.GetReceivedRotation( NeuronBones.LeftHandPinky1 ) + actor.GetReceivedRotation( NeuronBones.LeftInHandPinky ) );
        SetRotation(animator, HumanBodyBones.LeftLittleProximal,
            (EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.LeftInHandPinky)) *
             EulerToQuaternion(actor.GetReceivedRotation(NeuronBones.LeftHandPinky1))).eulerAngles
            );
        SetRotation(animator, HumanBodyBones.LeftLittleIntermediate, actor.GetReceivedRotation(NeuronBones.LeftHandPinky2));
        SetRotation(animator, HumanBodyBones.LeftLittleDistal, actor.GetReceivedRotation(NeuronBones.LeftHandPinky3));
    }

    Quaternion EulerToQuaternion(Vector3 euler)
    {
        return Quaternion.Euler(euler.x, euler.y, euler.z);
    }

    // apply Transforms of src bones to Rigidbody Components of dest bones
    public void ApplyMotionPhysically(Animator src, Animator dest)
    {
        for (HumanBodyBones i = 0; i < HumanBodyBones.LastBone; ++i)
        {
            Transform src_transform = src.GetBoneTransform(i);
            Transform dest_transform = dest.GetBoneTransform(i);
            if (src_transform != null && dest_transform != null)
            {
                Rigidbody rigidbody = dest_transform.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    switch (motionUpdateMethod)
                    {
                        case UpdateMethod.Physical:
                            rigidbody.MovePosition(src_transform.position);
                            rigidbody.MoveRotation(src_transform.rotation);
                            break;

                        case UpdateMethod.EstimatedPhysical:
                            Quaternion dAng = src_transform.rotation * Quaternion.Inverse(dest_transform.rotation);
                            float angle = 0.0f;
                            Vector3 axis = Vector3.zero;
                            dAng.ToAngleAxis(out angle, out axis);

                            //float newMagic = Vector3.Distance (src_transform.position, dest_transform.position) * velocityMagic;
                            Vector3 velocityTarget = (src_transform.position - dest_transform.position) * velocityMagic * Time.fixedDeltaTime;
                            //Vector3 velocityTarget = (src_transform.position - dest_transform.position) * velocityMagic * Time.fixedDeltaTime;

                            Vector3 angularTarget = (Time.fixedDeltaTime * angle * axis) * angularVelocityMagic;

                            ApplyVelocity(rigidbody, velocityTarget, angularTarget);

                            break;

                        case UpdateMethod.MixedPhysical:
                            Vector3 velocityTarget2 = (src_transform.position - dest_transform.position) * velocityMagic * Time.fixedDeltaTime;

                            Vector3 v = Vector3.MoveTowards(rigidbody.velocity, velocityTarget2, 10.0f);
                            if (ValidateVector3(v))
                            {
                                rigidbody.velocity = v;
                            }

                            rigidbody.MoveRotation(src_transform.rotation);


                            break;
                    }

                }
            }
        }
    }

    void ApplyVelocity(Rigidbody rb, Vector3 velocityTarget, Vector3 angularTarget)
    {
        Vector3 v = Vector3.MoveTowards(rb.velocity, velocityTarget, 10.0f);
        if (ValidateVector3(v))
        {
            rb.velocity = v;
        }

        v = Vector3.MoveTowards(rb.angularVelocity, angularTarget, 10.0f);
        if (ValidateVector3(v))
        {
            rb.angularVelocity = v;
        }
    }


    //bool InitPhysicalContext()
    void InitPhysicalContext()
    {
        if (physicalReference.Init(boundAnimator, physicalReferenceOverride))
        {
            // break original object's hierachy of transforms, so we can use MovePosition() and MoveRotation() to set transform
            NeuronHelper.BreakHierarchy(boundAnimator);
            //return true;
        }

        CheckRigidbodySettings();
        //	return false;
    }

    void ReleasePhysicalContext()
    {
        physicalReference.Release();
    }

    void UpdateOffset()
    {
        // we do some adjustment for the bones here which would replaced by our model retargeting later

        if (boundAnimator != null)
        {
            // initiate values
            for (int i = 0; i < (int)HumanBodyBones.LastBone; ++i)
            {
                bonePositionOffsets[i] = Vector3.zero;
                boneRotationOffsets[i] = Vector3.zero;
            }

            if (boundAnimator != null)
            {
                Transform leftLegTransform = boundAnimator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
                Transform rightLegTransform = boundAnimator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
                if (leftLegTransform != null)
                {
                    bonePositionOffsets[(int)HumanBodyBones.LeftUpperLeg] = new Vector3(0.0f, leftLegTransform.localPosition.y, 0.0f);
                    bonePositionOffsets[(int)HumanBodyBones.RightUpperLeg] = new Vector3(0.0f, rightLegTransform.localPosition.y, 0.0f);
                    bonePositionOffsets[(int)HumanBodyBones.Hips] = new Vector3(0.0f, -(leftLegTransform.localPosition.y + rightLegTransform.localPosition.y) * 0.5f, 0.0f);
                }
            }
        }
    }

    void CaluateOrignalRot()
    {
        for (int i = 0; i < orignalRot.Length; i++)
        {
            Transform t = boundAnimator.GetBoneTransform((HumanBodyBones)i);

            orignalRot[i] = t == null ? Quaternion.identity : t.localRotation;
        }
        for (int i = 0; i < orignalRot.Length; i++)
        {
            Quaternion parentQs = Quaternion.identity;
            Transform t = boundAnimator.GetBoneTransform((HumanBodyBones)i);
            if (t == null)
            {
                orignalParentRot[i] = Quaternion.identity;
                continue;
            }
            Transform tempParent = t.transform.parent;
            while (tempParent != null)
            {
                parentQs = tempParent.transform.localRotation * parentQs;
                tempParent = tempParent.parent;
                if (tempParent == null || tempParent == this.gameObject)
                    break;
            }
            orignalParentRot[i] = parentQs;
        }
    }
    void CheckRigidbodySettings()
    {
        //check if rigidbodies have correct settings
        bool kinematicSetting = false;
        if (motionUpdateMethod == UpdateMethod.Physical)
        {
            kinematicSetting = true;
        }

        for (HumanBodyBones i = 0; i < HumanBodyBones.LastBone; ++i)
        {
            Transform t = boundAnimator.GetBoneTransform(i);
            if (t != null)
            {
                Rigidbody r = t.GetComponent<Rigidbody>();
                if (r != null)
                {
                    r.isKinematic = kinematicSetting;
                }
            }
        }
    }
}