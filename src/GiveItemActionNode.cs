using NodeCanvas.DialogueTrees;
using System;

namespace PopulatedWorld
{
    [Serializable]
    public class GiveItemActionNode : XMLActionNode
    {
        public int ItemID;

        public override DTNode CreateAction()
        {
            var action = new GiveItem();
            action.ItemID = ItemID;
            return action;
        }


    }
}
