using System;

namespace QuickMap
{
    public abstract class Mapper
    {
        internal Type SrcType;
        internal Type DstType;
        public Func<object, object> AdaptFrom;

        internal Mapper() { }
        public abstract object PerformMap(object srcObject);
        public abstract object PerformMap(object srcObject, object dstObject);
    }
}