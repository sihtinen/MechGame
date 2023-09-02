using System.Collections.Generic;
using UnityEngine;

namespace BKUnity
{
	public class AvatarUtils
	{
		//A static dictionary containing the mapping from joint/bones names in the model
		//to the names Unity uses for them internally.
		//In this case they match the naming from the included Mixamo model on the left
		//and the Unity equivalent name on the right. 
		//This does not need to be hard-coded. 
		public static Dictionary<string, string> HumanSkeletonNames = new Dictionary<string, string>()
		{
			{"mixamorig:Spine1", "Chest"},
			{"mixamorig:Head", "Head" },
			{"mixamorig:Hips", "Hips" },
			{"mixamorig:LeftHandIndex3", "Left Index Distal" },
			{"mixamorig:LeftHandIndex2", "Left Index Intermediate" },
			{"mixamorig:LeftHandIndex1", "Left Index Proximal" },
			{"mixamorig:LeftHandPinky3", "Left Little Distal" },
			{"mixamorig:LeftHandPinky2", "Left Little Intermediate" },
			{"mixamorig:LeftHandPinky1", "Left Little Proximal" },
			{"mixamorig:LeftHandMiddle3", "Left Middle Distal" },
			{"mixamorig:LeftHandMiddle2", "Left Middle Intermediate" },
			{"mixamorig:LeftHandMiddle1", "Left Middle Proximal" },
			{"mixamorig:LeftHandRing3", "Left Ring Distal" },
			{"mixamorig:LeftHandRing2", "Left Ring Intermediate" },
			{"mixamorig:LeftHandRing1", "Left Ring Proximal" },
			{"mixamorig:LeftHandThumb3", "Left Thumb Distal" },
			{"mixamorig:LeftHandThumb2", "Left Thumb Intermediate" },
			{"mixamorig:LeftHandThumb1", "Left Thumb Proximal" },
			{"mixamorig:LeftFoot", "LeftFoot" },
		    {"mixamorig:LeftHand", "LeftHand" },
			{"mixamorig:LeftForeArm", "LeftLowerArm" },
			{"mixamorig:LeftLeg", "LeftLowerLeg" },
			{"mixamorig:LeftShoulder", "LeftShoulder" },
			{"mixamorig:LeftToeBase", "LeftToes" },
			{"mixamorig:LeftArm", "LeftUpperArm" },
			{"mixamorig:LeftUpLeg", "LeftUpperLeg" },
			{"mixamorig:Neck", "Neck" },
			{"mixamorig:RightHandIndex3", "Right Index Distal" },
			{"mixamorig:RightHandIndex2", "Right Index Intermediate" },
			{"mixamorig:RightHandIndex1", "Right Index Proximal" },
			{"mixamorig:RightHandPinky3", "Right Little Distal" },
			{"mixamorig:RightHandPinky2", "Right Little Intermediate" },
			{"mixamorig:RightHandPinky1", "Right Little Proximal" },
			{"mixamorig:RightHandMiddle3", "Right Middle Distal" },
			{"mixamorig:RightHandMiddle2", "Right Middle Intermediate" },
			{"mixamorig:RightHandMiddle1", "Right Middle Proximal" },
			{"mixamorig:RightHandRing3", "Right Ring Distal" },
			{"mixamorig:RightHandRing2", "Right Ring Intermediate" },
			{"mixamorig:RightHandRing1", "Right Ring Proximal" },
			{"mixamorig:RightHandThumb3", "Right Thumb Distal" },
			{"mixamorig:RightHandThumb2", "Right Thumb Intermediate" },
			{"mixamorig:RightHandThumb1", "Right Thumb Proximal" },
			{"mixamorig:RightFoot", "RightFoot" },
			{"mixamorig:RightHand", "RightHand" },
			{"mixamorig:RightForeArm", "RightLowerArm" },
			{"mixamorig:RightLeg", "RightLowerLeg" },
			{"mixamorig:RightShoulder", "RightShoulder" },
			{"mixamorig:RightToeBase", "RightToes" },
			{"mixamorig:RightArm", "RightUpperArm" },
			{"mixamorig:RightUpLeg", "RightUpperLeg" },
			{"mixamorig:Spine", "Spine" },
			{"mixamorig:Spine2", "UpperChest" }
		};

        public static Dictionary<string, string> HumanSkeletonNames2 = new Dictionary<string, string>()
        {
			{"Bone_Root", "Root"},
            {"Bone_Chest", "Chest"},
            {"Bone_Head", "Head" },
            {"Bone_Hip", "Hips" },
            {"Bone_Foot.L", "LeftFoot" },
            {"Bone_Hand.L", "LeftHand" },
            {"Bone_ArmLower.L", "LeftLowerArm" },
            {"Bone_LegLower.L", "LeftLowerLeg" },
            {"Bone_Shoulder.L", "LeftShoulder" },
            {"Bone_ArmUpper.L", "LeftUpperArm" },
            {"Bone_LegUpper.L", "LeftUpperLeg" },
            {"Bone_Neck", "Neck" },
            {"Bone_Foot.R", "RightFoot" },
            {"Bone_Hand.R", "RightHand" },
            {"Bone_ArmLower.R", "RightLowerArm" },
            {"Bone_LegLower.R", "RightLowerLeg" },
            {"Bone_Shoulder.R", "RightShoulder" },
            {"Bone_ArmUpper.R", "RightUpperArm" },
            {"Bone_LegUpper.R", "RightUpperLeg" },
            {"Bone_Spine", "Spine" },
        };

        /// <summary>
        /// Create a HumanDescription out of an avatar GameObject. 
        /// The HumanDescription is what is needed to create an Avatar object
        /// using the AvatarBuilder API. This function takes care of 
        /// creating the HumanDescription by going through the avatar's
        /// hierarchy, defining its T-Pose in the skeleton, and defining
        /// the transform/bone mapping in the HumanBone array. 
        /// </summary>
        /// <param name="avatarRoot">Root of your avatar object</param>
        /// <returns>A HumanDescription which can be fed to the AvatarBuilder API</returns>
        public static HumanDescription CreateHumanDescription(GameObject avatarRoot)
		{
			var _description = new HumanDescription()
			{
				armStretch = 0.05f,
				feetSpacing = 0f,
				hasTranslationDoF = false,
				legStretch = 0.05f,
				lowerArmTwist = 0.5f,
				lowerLegTwist = 0.5f,
				upperArmTwist = 0.5f,
				upperLegTwist = 0.5f,
				skeleton = CreateSkeleton(avatarRoot),
				human = CreateHuman(avatarRoot),
			};

			return _description;
		}

		//Create a SkeletonBone array out of an Avatar GameObject
		//This assumes that the Avatar as supplied is in a T-Pose
		//The local positions of its bones/joints are used to define this T-Pose
		private static SkeletonBone[] CreateSkeleton(GameObject avatarRoot)
		{
			var _skeleton = new List<SkeletonBone>();
			var _avatarTransforms = avatarRoot.GetComponentsInChildren<Transform>();

			foreach (var _avatarTransform in _avatarTransforms)
			{
				var _bone = new SkeletonBone()
				{
					name = _avatarTransform.name,
					position = _avatarTransform.localPosition,
					rotation = _avatarTransform.localRotation,
					scale = _avatarTransform.localScale
				};

				_skeleton.Add(_bone);
			}

			return _skeleton.ToArray();
		}

		//Create a HumanBone array out of an Avatar GameObject
		//This is where the various bones/joints get associated with the
		//joint names that Unity understands. This is done using the
		//static dictionary defined at the top. 
		private static HumanBone[] CreateHuman(GameObject avatarRoot)
		{
			var _human = new List<HumanBone>();
			var _avatarTransforms = avatarRoot.GetComponentsInChildren<Transform>();

			foreach (var _avatarTransform in _avatarTransforms)
            {
                bool _humanBoneFound = HumanSkeletonNames2.TryGetValue(_avatarTransform.name, out string humanBoneName);

                if (_humanBoneFound == false)
                    continue;

                var _bone = new HumanBone
                {
                    boneName = _avatarTransform.name,
                    humanName = humanBoneName,
                    limit = new HumanLimit(),
                };

                _bone.limit.useDefaultValues = true;

                _human.Add(_bone);
            }

            return _human.ToArray();
		}
	}
}
