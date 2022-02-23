using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickMap
{
    public static class ObjectExtensions
    {
        public static IEnumerable<TDestination> AdaptMany<TDestination, TSource>(this IEnumerable<TSource> srcObjects)
            where TSource : class, new()
            where TDestination : class, new()
        {
            var lstDestinationObjects = new List<TDestination>();
            var mappedObjects = AdaptMany<TSource, TDestination>(srcObjects);
            lstDestinationObjects.AddRange(mappedObjects);
            return lstDestinationObjects.AsEnumerable();
        }

        public static IEnumerable<TDestination> AdaptMany<TSource, TDestination>(IEnumerable<TSource> srcObjects)
            where TSource : class, new()
            where TDestination : class, new()
        {
            if (srcObjects.Count() < 1)
                yield break;

            TDestination dstObject = new TDestination();

            foreach (var srcObject in srcObjects)
            {
                dstObject = dstObject.AdaptFrom(srcObject);
                yield return dstObject;
            }
        }

        public static TDestination As<TDestination>(this object srcObject)
            where TDestination : class, new()
        {
            var tSource = srcObject.GetType();

            Func<Mapper, bool> predicate = x => x.SrcType == tSource && x.DstType == typeof(TDestination);

            if (!ApplicationMap.RegisteredMappers.Any(predicate))
                return new TDestination();

            var mapper = ApplicationMap.RegisteredMappers.First(predicate);
            var dstObject = mapper.PerformMap(srcObject);
            return dstObject as TDestination;
        }

        public static TDestination As<TDestination>(this object srcObject, TDestination dstObject)
            where TDestination : class
        {
            var tSource = srcObject.GetType();

            Func<Mapper, bool> predicate = x => x.SrcType == tSource && x.DstType == typeof(TDestination);

            if (!ApplicationMap.RegisteredMappers.Any(predicate))
                return dstObject;

            var mapper = ApplicationMap.RegisteredMappers.First(predicate);
            //dstObject = mapper.PerformMap(srcObject) as TDestination;
            dstObject = mapper.PerformMap(srcObject, dstObject) as TDestination;
            return dstObject;
        }

        public static IEnumerable<Type> GetGenericIEnumerables(this object o)
        {
            return o.GetType()
                    .GetInterfaces()
                    .Where(t => t.IsGenericType
                        && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .Select(t => t.GetGenericArguments()[0]);
        }

        public static IEnumerable<TDestination> AsMany<TDestination>(this IEnumerable<object> srcObjects)
           where TDestination : class, new()
        {
            foreach (var srcObject in srcObjects)
                yield return srcObject.As<TDestination>();
        }

        public static TDestination AdaptFrom<TSource, TDestination>(this TDestination dstObject, TSource srcObject)
            where TSource : class //, new()
            where TDestination : class, new()
        {
            Type dstType = dstObject?.GetType() ?? typeof(TDestination);
            Type srcType = srcObject?.GetType() ?? typeof(TSource);


            Func<Mapper, bool> predicate = x => x.SrcType == srcType && x.DstType == dstType;
            if (!ApplicationMap.RegisteredMappers.Any(predicate))
                throw new Exception($"No mapper registered for Source Type: ${srcType.Name} and Destination Type: ${dstType.Name}");

            var mapper = ApplicationMap.RegisteredMappers.First(predicate);
            var strongMapper = mapper as Mapper<TSource, TDestination>;
            dstObject = strongMapper.PerformMap(srcObject);
            return dstObject;
        }
    }
}
