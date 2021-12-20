﻿using OpenNefia.Core.ContentPack;

namespace OpenNefia.Core
{
    internal static class ProgramShared
    {
        internal static void DoMounts(IResourceManagerInternal res)
        {
#if FULL_RELEASE
            // TODO: I don't want one single content root since
            // it prevents mods from being drop-in. Instead I want something like:
            //
            // Core/Resources/Prototypes/Core
            // Elona/Resources/Prototypes/Elona
            // Autopickup/Resources/Prototypes/Autopickup
            // 
            // ...And so on.
            res.MountContentDirectory("Resources/");
#else
            // Assets directory in OpenNefia.Core
            res.MountContentDirectory("../../../../OpenNefia.Core/Resources");

            // Assets directory in OpenNefia.Content
            res.MountContentDirectory("../../../../OpenNefia.Content/Resources");

            // Assets directory held by baked vegetables
            // TODO this is dumb
            res.MountContentDirectory("../../../../OpenNefia.LecchoTorte/Resources");

            // Autogenerated directory holding OpenNefia.Content.dll in build output
            // TODO make this saner
            res.MountContentDirectory("Resources");
#endif
        }
    }
}
