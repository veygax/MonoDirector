using UnityEngine;

using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Pool;

using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.Actors
{
    public static class ActorNPCBuilder
    {
        public static void BuildNPCActor(Poolee pooleeObject)
        {
            ActorNPC test = new ActorNPC(pooleeObject.transform);
        }

        public static void RemoveNPCActor(Poolee pooleeObject)
        {
            var gameObject = pooleeObject.gameObject;
            var vfxBlip = gameObject.GetComponent<Blip>();

            Prop actorProp = gameObject.GetComponent<Prop>();
            bool isProp = actorProp != null;
        }
    }
}
