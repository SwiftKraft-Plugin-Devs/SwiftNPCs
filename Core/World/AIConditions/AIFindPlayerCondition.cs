using PlayerRoles;
using PluginAPI.Core;
using SwiftAPI.Utility;
using SwiftNPCs.Core.World.AIModules;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftNPCs.Core.World.AIConditions
{
    public class AIFindPlayerCondition : AIConditionBase
    {
        public float SearchDistance = 10f;

        public Player LastFoundPlayer;

        public override bool Get()
        {
            LastFoundPlayer = FindTarget();
            return LastFoundPlayer != null;
        }

        public override void Pass(AIModuleBase target)
        {
            target.ReceiveData(LastFoundPlayer);
        }

        public Player FindTarget()
        {
            List<Player> players = Player.GetPlayers();

            players.RemoveAll((p) => p.ReferenceHub == ReferenceHub || !CanTarget(p) || Vector3.Distance(Position, p.Position) >= SearchDistance);

            if (players.Count <= 0)
                return null;

            Player res = null;
            foreach (Player p in players)
            {
                if (res == null || Vector3.Distance(p.Position, Position) < Vector3.Distance(res.Position, Position))
                    res = p;
            }

            return res;
        }

        public virtual bool CanTarget(Player p) => p != null && p.IsAlive && p.Role.GetFaction() == Runner.Role.GetFaction() && ParentModule.Parent.HasLOS(p, out _);
    }
}
