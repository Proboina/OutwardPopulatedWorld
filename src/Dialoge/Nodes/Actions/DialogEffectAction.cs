using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using SideLoader;
using System;
using UnityEngine;

namespace PopulatedWorld
{
    public class DialogEffectAction : ActionNode
    {
        public SL_EffectTransform[] effectTransforms;
        public bool targetInstigator = true;
        public EditBehaviours editBehaviour;

        private DialogueEffectContainer effectContainer;

        public override Status OnExecute(Component agent, IBlackboard bb)
        {
            var instigator = bb.GetVariable<Character>("gInstigator").GetValue();

            Character affectedCharacter = targetInstigator ? instigator : agent.GetComponentInParent<Character>();

            if (affectedCharacter == null)
            {
                Debug.LogError("[DialogEffect] Target character not found");
                return Status.Failure;
            }


            if (Plugin.Skill == null)
            {
                Debug.Log($"DialogEffectAction :: Skill is null generating new..");
                Plugin.Instance.GenerateDialogueSkillSynchroniser();
            }




            SL_EffectTransform.ApplyTransformList(Plugin.Skill.transform, effectTransforms, EditBehaviours.Destroy);

            Plugin.Skill.SynchronizeEffects(EffectSynchronizer.EffectCategories.Activation, affectedCharacter);
            return Status.Success;
        }
    }


    [Serializable]
    public class DialogEffectNode : XMLActionNode
    {
        public SL_EffectTransform[] effectTransforms;
        public bool targetInstigator;
        public EditBehaviours editBehaviour;

        public override DTNode CreateAction()
        {
            DialogEffectLogger.Log($"Creating action with {effectTransforms?.Length ?? 0} transforms");

            var action = new DialogEffectAction();
            action.effectTransforms = this.effectTransforms;
            action.targetInstigator = this.targetInstigator;
            action.editBehaviour = this.editBehaviour;

            return action;
        }

        public override DTNode BuildNode(DialogueTree tree, DialogueTreeBuilder builder, DTNode parentNode)
        {
            var node = base.BuildNode(tree, builder, parentNode);
            builder.AddNode(node);

            if (parentNode != null)
            {
                parentNode.ConnectTo(tree, node);
            }

            return node;
        }
    }
}
