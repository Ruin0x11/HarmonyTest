﻿using OpenNefia.Core.IoC;
using OpenNefia.Core.Maps;
using OpenNefia.Core.Maths;

namespace OpenNefia.Core.GameObjects
{
    public class MovementSystem : EntitySystem
    {
        [Dependency] private readonly IMapManager _mapManager = default!;

        public override void Initialize()
        {
            SubscribeLocalEvent<MoveableComponent, MoveEventArgs>(HandleMove, nameof(HandleMove));
            SubscribeLocalEvent<MoveableComponent, EntPositionChangedEvent>(HandlePositionChanged, nameof(HandlePositionChanged));
        }

        private void HandlePositionChanged(EntityUid uid, MoveableComponent component, ref EntPositionChangedEvent args)
        {
            SpatialComponent? spatial = null;

            if (!Resolve(uid, ref spatial))
                return;

            if (!_mapManager.TryGetMap(spatial.MapID, out var map))
                return;

            map.RefreshTile(args.OldPosition.Position);
            map.RefreshTile(args.NewPosition.Position);
        }

        private void HandleMove(EntityUid uid, MoveableComponent moveable, MoveEventArgs args)
        {
            if (args.Handled || !EntityManager.IsAlive(uid))
                return;

            HandleMove(uid, args, moveable);
        }

        private void HandleMove(EntityUid uid, MoveEventArgs args, 
            MoveableComponent? moveable = null,
            SpatialComponent? spatial = null)
        {
            if (!Resolve(uid, ref moveable, ref spatial))
                return;

            spatial.Direction = (args.NewPosition.Position - args.OldPosition.Position).GetDir();

            var evBefore = new BeforeMoveEventArgs(args.OldPosition, args.NewPosition);
            RaiseLocalEvent(uid, evBefore);

            if (evBefore.Handled || !EntityManager.IsAlive(uid))
            {
                args.Handled = true;
                args.TurnResult = evBefore.TurnResult;
                return;
            }

            if (!_mapManager.TryGetMap(args.NewPosition.MapId, out var map) 
                || !map.CanAccess(args.NewPosition.Position))
            {
                args.Handled = true;
                args.TurnResult = TurnResult.Failed;
                return;
            }

            spatial.WorldPosition = args.NewPosition.Position;

            var evAfter = new AfterMoveEventArgs(args.OldPosition, args.NewPosition);
            RaiseLocalEvent(uid, evAfter);

            args.Handled = true;
            args.TurnResult = TurnResult.Succeeded;
        }
    }

    public class MoveEventArgs : HandledEntityEventArgs
    {
        public readonly MapCoordinates OldPosition;
        public readonly MapCoordinates NewPosition;

        public MoveEventArgs(MapCoordinates oldPosition, MapCoordinates newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }

        public TurnResult TurnResult;
    }

    public class BeforeMoveEventArgs : HandledEntityEventArgs
    {
        public readonly MapCoordinates OldPosition;
        public readonly MapCoordinates NewPosition;

        public BeforeMoveEventArgs(MapCoordinates oldPosition, MapCoordinates newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }

        public TurnResult TurnResult;
    }

    public class AfterMoveEventArgs : HandledEntityEventArgs
    {
        public readonly MapCoordinates OldPosition;
        public readonly MapCoordinates NewPosition;

        public AfterMoveEventArgs(MapCoordinates oldPosition, MapCoordinates newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
    }
}
