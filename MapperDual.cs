using System;
using System.Collections.Generic;
using System.Text;

namespace QuickMap
{
    public abstract class DualMapper<TSource, TDestination> : Mapper
        where TSource : class, new()
        where TDestination : class, new()
    {
        public DualMapper()
        {
            Func<TSource, TDestination> getDest = GetDestination;
            var toMapper = new FakeMapper<TSource, TDestination>(GetDestination);
            ApplicationMap.RegisterMapper(toMapper);

            Func<TDestination, TSource> getSrc = GetSource;
            var fromMapper = new FakeMapper<TDestination, TSource>(GetSource);
            ApplicationMap.RegisterMapper(fromMapper);
        }

        public TDestination GetDestination(TSource srcObject)
        {
            TDestination dstObject = new TDestination();
            MapTo(srcObject, ref dstObject);
            return dstObject;
        }

        public TSource GetSource(TDestination dstObject)
        {
            TSource srcObject = new TSource();
            MapFrom(dstObject, ref srcObject);
            return srcObject;
        }


        public abstract void MapTo(TSource srcObject, ref TDestination dstObject); // todo: new pattern?
        public abstract void MapFrom(TDestination srcObject, ref TSource dstObject); // todo: new pattern?

        public override object PerformMap(object srcObject)
        {
            throw new NotImplementedException();
        }

        public override object PerformMap(object srcObject, object dstObject)
        {
            throw new NotImplementedException();
        }

    }

    internal class FakeMapper<TSource, TDestination> : Mapper
        where TSource : class
        where TDestination : class
    {
        Func<TSource, TDestination> GetMappedTypeFunc;

        public FakeMapper(Func<TSource, TDestination> prmGetMappedTypeFunc)
        {
            SrcType = typeof(TSource);
            DstType = typeof(TDestination);

            GetMappedTypeFunc = prmGetMappedTypeFunc;
        }

        public override object PerformMap(object srcObject)
        {
            return GetMappedTypeFunc(srcObject as TSource);
        }

        public override object PerformMap(object srcObject, object dstObject)
        {
            throw new NotImplementedException();
        }
    }
}
