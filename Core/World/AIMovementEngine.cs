using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace SwiftNPCs.Core.World
{
    public class AIMovementEngine : AIAddon
    {
        public FirstPersonMovementModule FirstPersonMovement
        {
            get
            {
                if (Core.FirstPersonController != null)
                    return Core.FirstPersonController.FpcModule;
                else
                    return null;
            }
        }

        public CharacterController CharCont
        {
            get
            {
                if (FirstPersonMovement != null)
                    return FirstPersonMovement.CharController;
                else
                    return null;
            }
        }

        public FpcMouseLook MouseLook
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
                if (FirstPersonMovement != null && SpeedOverride <= 0f)
                    switch (State)
                    {
                        case PlayerMovementState.Walking:
                            return FirstPersonMovement.WalkSpeed;
                        case PlayerMovementState.Sprinting:
                            return FirstPersonMovement.SprintSpeed;
                        case PlayerMovementState.Sneaking:
                            return FirstPersonMovement.SneakSpeed;
                        case PlayerMovementState.Crouching:
                            return FirstPersonMovement.CrouchSpeed;
                    }
                else if (SpeedOverride > 0f)
                    return SpeedOverride;
                return 0f;
            }
        }

        public float SpeedOverride = -1f;
        public float LookSpeed = 90f;

        public Vector3 WishDir;
        public Vector3 LookDir
        {
            get => TargetLookRot * Vector3.forward;
            set => TargetLookRot = value != Vector3.zero ? Quaternion.LookRotation(value.normalized, Vector3.up) : Quaternion.identity;
        }
        public Vector3 LookPos
        {
            get => ReferenceHub.PlayerCameraReference.position + TargetLookRot * Vector3.forward;
            set => LookDir = (value - ReferenceHub.PlayerCameraReference.position).normalized;
        }

        public Quaternion TargetLookRot { get; set; }

        public PlayerMovementState State
        {
            get => FirstPersonMovement.CurrentMovementState;
            set => FirstPersonMovement.CurrentMovementState = value;
        }

        protected Quaternion CurrentLookRot;

        public void UpdateMove(Vector3 wishDir)
        {
            if (FirstPersonMovement == null || wishDir == Vector3.zero)
                return;

            CharCont.Move(wishDir * (CurrentSpeed * Time.fixedDeltaTime));
        }

        public void UpdateLook(Quaternion rotation)
        {
            if (FirstPersonMovement == null)
                return;

            Vector3 direction = rotation * Vector3.forward;

            MouseLook.LookAtDirection(direction);
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
    }
}
