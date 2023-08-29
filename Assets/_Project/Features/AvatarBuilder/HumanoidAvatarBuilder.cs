﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BKUnity
{
	public class HumanoidAvatarBuilder : MonoBehaviour
	{
        [SerializeField] private AvatarTarget m_avatarTarget = null;
		[SerializeField] private Animator m_animator;
        [SerializeField] private bool m_drawBones = false;

        private void Start()
        {
            BuildRuntime();
        }

        [ContextMenu("Build Avatar")]
        public void BuildAvatar()
		{
            var _humanDescription = AvatarUtils.CreateHumanDescription(gameObject);
			var _avatar = AvatarBuilder.BuildHumanAvatar(gameObject, _humanDescription);
			_avatar.name = gameObject.name;

			if (m_animator != null)
				m_animator.avatar = _avatar;
		}

        [ContextMenu("Align To Avatar Target")]
        public void AlignToAvatarTarget()
        {
            if (m_avatarTarget == null)
                return;

            transform.BuildChildNameCache(m_childCache);

            foreach (var _kvp in m_childCache)
            {
                var _targetBone = m_avatarTarget.GetBone(_kvp.Key);
                if (_targetBone != null)
                    _kvp.Value.SetPositionAndRotation(_targetBone.position, _targetBone.rotation);
            }

            m_avatarTarget.BindToRig(transform, m_childCache);

            m_childCache.Clear();
        }

        [ContextMenu("Build Runtime")]
        public void BuildRuntime()
        {
            m_animator.enabled = false;
            m_animator.avatar = null;

            AlignToAvatarTarget();
            BuildAvatar();

            m_animator.enabled = true;
        }

        private Dictionary<string, Transform> m_childCache = new();

        private void OnDrawGizmos()
        {
            if (m_drawBones == false)
                return;

            transform.BuildChildNameCache(m_childCache);

            Gizmos.color = Color.yellow;

            drawLineGizmo(m_childCache["Bone_Hip"], m_childCache["Bone_Spine"]);
            drawLineGizmo(m_childCache["Bone_Spine"], m_childCache["Bone_Chest"]);
            drawLineGizmo(m_childCache["Bone_Chest"], m_childCache["Bone_Neck"]);
            drawLineGizmo(m_childCache["Bone_Neck"], m_childCache["Bone_Head"]);

            drawLineGizmo(m_childCache["Bone_Chest"], m_childCache["Bone_Shoulder.R"]);
            drawLineGizmo(m_childCache["Bone_Shoulder.R"], m_childCache["Bone_ArmUpper.R"]);
            drawLineGizmo(m_childCache["Bone_ArmUpper.R"], m_childCache["Bone_ArmLower.R"]);
            drawLineGizmo(m_childCache["Bone_ArmLower.R"], m_childCache["Bone_Hand.R"]);

            drawLineGizmo(m_childCache["Bone_Chest"], m_childCache["Bone_Shoulder.L"]);
            drawLineGizmo(m_childCache["Bone_Shoulder.L"], m_childCache["Bone_ArmUpper.L"]);
            drawLineGizmo(m_childCache["Bone_ArmUpper.L"], m_childCache["Bone_ArmLower.L"]);
            drawLineGizmo(m_childCache["Bone_ArmLower.L"], m_childCache["Bone_Hand.L"]);

            drawLineGizmo(m_childCache["Bone_Hip"], m_childCache["Bone_LegUpper.R"]);
            drawLineGizmo(m_childCache["Bone_LegUpper.R"], m_childCache["Bone_LegLower.R"]);
            drawLineGizmo(m_childCache["Bone_LegLower.R"], m_childCache["Bone_Foot.R"]);

            drawLineGizmo(m_childCache["Bone_Hip"], m_childCache["Bone_LegUpper.L"]);
            drawLineGizmo(m_childCache["Bone_LegUpper.L"], m_childCache["Bone_LegLower.L"]);
            drawLineGizmo(m_childCache["Bone_LegLower.L"], m_childCache["Bone_Foot.L"]);

            m_childCache.Clear();
        }

        private void drawLineGizmo(Transform a, Transform b)
        {
            if (a == null || b == null)
                return;

            Gizmos.DrawLine(a.position, b.position);
        }
    }
}