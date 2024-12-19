using System;
using UnityEngine;

namespace PopulatedWorld
{
    public class DialogueEffectContainer : EffectSynchronizer
    {
        private Character ownerChar;
        private string effectSourceUID;

        public void Initialize(Character owner)
        {
            ownerChar = owner;
            effectSourceUID = "DialogueEffect_" + Guid.NewGuid().ToString();
        }

        public override string GetSourceUID()
        {
            return effectSourceUID;
        }

        public override string GetSourceID()
        {
            return "DialogueEffect";
        }

        public override Character OwnerCharacter
        {
            get { return ownerChar; }
        }

        public override void SynchronizeEffects(EffectCategories _category, Character _affectedCharacter)
        {
            base.SynchronizeEffects(_category, _affectedCharacter);

            Debug.Log($"SynchronizeEffects {_category}");
        }
        public override void SynchronizeEffects(EffectCategories _category, Character _affectedCharacter, Vector3 _pos, Vector3 _dir)
        {
            base.SynchronizeEffects(_category, _affectedCharacter, _pos, _dir);
        }

        public override void SendSyncEffects(Character _affectedCharacter, string _infos)
        {
            if (PhotonNetwork.isNonMasterClientInRoom)
            {
                Global.RPCManager.SendSkillActivatedEffects(
                    (_affectedCharacter != null) ? _affectedCharacter.UID.Value : "",
                    this.GetSourceUID(),
                    _infos
                );
            }
            else
            {
                this.OnReceiveActivatedEffects(_affectedCharacter, _infos);
            }
        }

        public override void SendStopEffects(Character _affectedCharacter, string _stoppedEffects)
        {
            if (PhotonNetwork.isNonMasterClientInRoom)
            {
                Global.RPCManager.SendSkillStopEffects(
                    (_affectedCharacter != null) ? _affectedCharacter.UID.Value : "",
                    this.GetSourceUID(),
                    _stoppedEffects
                );
            }
            else
            {
                this.OnReceiveStopEffects(_affectedCharacter, _stoppedEffects);
            }
        }
    }
}
