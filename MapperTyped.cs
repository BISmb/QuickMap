using System;
using System.Collections.Generic;
using System.Text;

namespace QuickMap
{
    public abstract class Mapper<TSource, TDestination> : Mapper
        where TSource : class//, new()
        where TDestination : class, new()
    {
        public Func<TSource, TDestination> AdaptFromStrong;

        public abstract void PerformMap(TSource srcObject, ref TDestination dstObject); // todo: new pattern?
        public virtual TDestination PerformMap(TSource srcObject)
        {
            TDestination outObject = new TDestination();
            return PerformMap(srcObject, outObject);
        }

        public virtual TDestination PerformMap(TSource srcObject, TDestination dstObject)
        {
            PerformMap(srcObject, ref dstObject);
            return dstObject;
        }

        public override object PerformMap(object srcObject) => PerformMap((TSource)srcObject);
        public override object PerformMap(object srcObject, object dstObject) => PerformMap((TSource)srcObject, (TDestination)dstObject);

        public Mapper()
        {
            SrcType = typeof(TSource);
            DstType = typeof(TDestination);
        }
    }
}
