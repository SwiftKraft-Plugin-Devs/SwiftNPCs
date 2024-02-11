using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using UnityEngine;

namespace SwiftNPCs.Core.World
{
    public class AIMovementEngine : AIAddon
    {
        protected FirstPersonMovementModule FirstPersonMovement
        {
            get
            {
                if (core.FirstPersonController != null)
                    return core.FirstPersonController.FpcModule;
                else
                    return null;
            }
        }

        protected CharacterController CharCont
        {
            get
            {
                if (FirstPersonMovement != null)
                    return FirstPersonMovement.CharController;
                else
                    return null;
            }
        }

        protected FpcMouseLook MouseLook
        {
            get
            {
                if (FirstPersonMovement != null)
                    return FirstPersonMovement.MouseLook;
                else
                    return null;
            }
        }

        public float CurrentSpeed
        {
            get
            {
                if (FirstPersonMovement != null)
                    switch (State)
                    {
                        case MovementState.Walk:
                            return FirstPersonMovement.WalkSpeed;
                        case MovementState.Sprint:
                            return FirstPersonMovement.SprintSpeed;
                        case MovementState.Crouch:
                            return FirstPersonMovement.CrouchSpeed;
                    }
                return 0f;
            }
        }

        public float LookSpeed = 90f;

        public Vector3 WishDir;

        public Quaternion TargetLookRot;
        public Quaternion CurrentLookRot;

        public MovementState State;

        public void UpdateMove(Vector3 wishDir)
        {
            if (FirstPersonMovement == null)
                return;

            CharCont.Move(wishDir * (CurrentSpeed * Time.fixedDeltaTime));
        }

        public void UpdateLook(Quaternion rotation)
        {
            if (FirstPersonMovement == null)
                return;

            Vector3 euler = rotation.eulerAngles;

            MouseLook.CurrentVertical = -euler.x;
            MouseLook.CurrentHorizontal = euler.y;
            transform.rotation = rotation;
        }

        private void FixedUpdate()
        {
            // WishDir = transform.forward;
            // TargetLookRot = Quaternion.LookRotation(transform.right, Vector3.up);

            CurrentLookRot = Quaternion.RotateTowards(CurrentLookRot, TargetLookRot, LookSpeed * Time.fixedDeltaTime);
            UpdateMove(WishDir);
        }

        private void Update()
        {
            UpdateLook(CurrentLookRot);
        }

        public enum MovementState
        {
            Walk,
            Sprint,
            Crouch
        }
    }
}
