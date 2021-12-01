﻿using OpenNefia.Core.Audio;
using OpenNefia.Core.Logic;
using OpenNefia.Core.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.GameObjects
{
    public class PlayerMovementSystem : EntitySystem
    {
        public override void Initialize()
        {
            SubscribeLocalEvent<MoveableComponent, BeforeMoveEventArgs>(HandleBeforeMove);
            SubscribeLocalEvent<PlayerComponent, BeforeMoveEventArgs>(HandleBeforeMovePlayer);
        }

        private void HandleBeforeMovePlayer(EntityUid uid, PlayerComponent player, BeforeMoveEventArgs args)
        {
            CheckMovementOutOfMap(uid, args, player);
        }

        public void CheckMovementOutOfMap(EntityUid uid, BeforeMoveEventArgs args,
           PlayerComponent? player = null)
        {
            if (args.Handled || !Resolve(uid, ref player))
                return;

            var (map, pos) = args.NewPosition;

            if (map != null && !map.IsInBounds(pos))
            {
                if (PlayerQuery.YesOrNo("Do you want to exit the map?"))
                {
                    RaiseLocalEvent(uid, new ExitMapEventArgs());
                    args.Handled = true;
                    args.TurnResult = TurnResult.Succeeded;
                    return;
                }
            }
        }

        private void HandleBeforeMove(EntityUid uid, MoveableComponent moveable, BeforeMoveEventArgs args)
        {
            CollideWithEntities(uid, args, moveable);
        }

        public void CollideWithEntities(EntityUid uid, BeforeMoveEventArgs args, 
            MoveableComponent? moveable = null)
        {
            if (args.Handled || !Resolve(uid, ref moveable))
                return;

            var entities = args.NewPosition.GetEntities()
                .Where(x => x.Spatial.IsSolid);

            foreach (var entity in entities)
            {
                var ev = new CollideWithEventArgs() { Target = entity.Uid };
                RaiseLocalEvent(uid, ev);
                if (ev.Handled)
                {
                    args.Handled = true;
                    args.TurnResult = ev.TurnResult;
                    return;
                }
            }
        }
    }

    internal class ExitMapEventArgs
    {
        public ExitMapEventArgs()
        {
        }
    }

    public class CollideWithEventArgs : HandledEntityEventArgs
    {
        public EntityUid Target;

        public TurnResult TurnResult;
    }
}
