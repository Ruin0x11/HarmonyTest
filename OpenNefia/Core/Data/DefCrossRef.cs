﻿using System;
using System.Reflection;

namespace OpenNefia.Core.Data
{
    internal class DefCrossRef
    {
        internal object target;
        internal FieldInfo targetProperty;
        internal Type crossRefType;
        internal string crossRefId;

        public DefCrossRef(object target, FieldInfo targetField, Type crossRefType, string crossRefId)
        {
            this.target = target;
            this.targetProperty = targetField;
            this.crossRefType = crossRefType;
            this.crossRefId = crossRefId;
        }
    }
}