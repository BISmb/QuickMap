using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QuickMap
{
    public static class ApplicationMap
    {
        internal static List<Mapper> RegisteredMappers = new List<Mapper>();

        public static void RegisterMapper<TSource, TDestination>(Mapper newMapperType)
        {
            newMapperType.AdaptFrom = (obj) => newMapperType.PerformMap(obj);
            RegisteredMappers.Add(newMapperType);
        }

        public static IEnumerable<Mapper> GetAllRegisterdMappers() => RegisteredMappers.AsEnumerable();

        public static void RegisterAllMappingsInAssembly(Assembly asm)
        {
            var types = asm.GetTypes();
            var mapperTypes = types.Where(type => type?.BaseType?.BaseType == typeof(Mapper));

            foreach (var mapperType in mapperTypes)
                RegisterMapper(Activator.CreateInstance(mapperType) as Mapper);
        }

        public static void RegisterMapper(Mapper newMapperType)
        {
            newMapperType.AdaptFrom = (obj) => newMapperType.PerformMap(obj);
            RegisteredMappers.Add(newMapperType);
        }

        //public static object AdaptFrom(this object dstObject, object srcObject)
        //{
        //    var mapper = _registeredMappers.First(x => x.SrcType == srcObject.GetType() && x.DstType == dstObject.GetType());
        //    return mapper.AdaptFrom.Invoke(srcObject);
        //}

        //public static object AdaptFrom(this object dstObject, object srcObject)
        //{
        //    var mapper = _registeredMappers.First(x => x.SrcType == srcObject.GetType() && x.DstType == dstObject.GetType());
        //    return mapper.AdaptFrom.Invoke(srcObject);
        //}

        public static TDestination Map<TSource, TDestination>(TDestination dstObject, TSource srcObject)
            where TSource : class//, new()
            where TDestination : class, new()
        {
            dstObject = dstObject.AdaptFrom(srcObject);
            return dstObject;
        }

        public static TDestination AdaptFrom<TSource, TDestination>(TSource srcObject)
            where TSource : class //, new()
            where TDestination : class, new()
        {
            Type dstType = typeof(TDestination);
            Type srcType = srcObject?.GetType() ?? typeof(TSource);


            Func<Mapper, bool> predicate = x => x.SrcType == srcType && x.DstType == dstType;
            if (!RegisteredMappers.Any(predicate))
                throw new Exception($"No mapper registered for Source Type: ${srcType.Name} and Destination Type: ${dstType.Name}");

            var dstObject = new TDestination();
            var mapper = RegisteredMappers.First(predicate);
            var strongMapper = mapper as Mapper<TSource, TDestination>;
            dstObject = strongMapper.PerformMap(srcObject);
            return dstObject;
        }

        //public static TDestination AdaptFrom<TSource, TDestination>(this object dstType, TSource srcObject)
        //    where TSource : class //, new()
        //    where TDestination : class, new()
        //{
        //    Type srcType = srcObject?.GetType() ?? typeof(TSource);


        //    Func<Mapper, bool> predicate = x => x.SrcType == srcType && x.DstType == dstType;
        //    if (!_registeredMappers.Any(predicate))
        //        throw new Exception($"No mapper registered for Source Type: ${srcType.Name} and Destination Type: ${dstType.Name}");

        //    var mapper = _registeredMappers.First(predicate);
        //    var strongMapper = mapper as Mapper<TSource, TDestination>;
        //    TDestination dstObject = new();
        //    dstObject = strongMapper.PerformMap(srcObject);
        //    return dstObject;
        //}

        // todo: implement as iterator function?
        
    }
}
