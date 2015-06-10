
using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Practices.Unity;

namespace Adhesive.Common
{
    public static class UnityContainerExtensions
    {
        [DebuggerStepThrough]
        public static IUnityContainer RegisterTypeAsSingleton(this IUnityContainer instance, Type fromType, Type toType)
        {
            lock (instance)
                return instance.RegisterType(fromType, toType, new ContainerControlledLifetimeManager());
        }

        [DebuggerStepThrough]
        public static IUnityContainer RegisterInstanceAsSingleton<TFrom>(this IUnityContainer instance, TFrom obj)
        {
            lock (instance)
                return instance.RegisterInstance<TFrom>(obj, new ContainerControlledLifetimeManager());
        }

        [DebuggerStepThrough]
        public static IUnityContainer RegisterTypeAsSingleton<TFrom, TTo>(this IUnityContainer instance) where TTo : TFrom
        {
            lock (instance)
                return instance.RegisterTypeAsSingleton(typeof(TFrom), typeof(TTo));
        }

        [DebuggerStepThrough]
        public static IUnityContainer RegisterTypeAsTransient(this IUnityContainer instance, Type fromType, Type toType)
        {
            lock (instance)
                return instance.RegisterType(fromType, toType, new TransientLifetimeManager());
        }

        [DebuggerStepThrough]
        public static IUnityContainer RegisterInstanceAsTransient<TFrom>(this IUnityContainer instance, TFrom obj)
        {
            lock (instance)
                return instance.RegisterInstance<TFrom>(obj, new TransientLifetimeManager());
        }

        [DebuggerStepThrough]
        public static IUnityContainer RegisterTypeAsTransient<TFrom, TTo>(this IUnityContainer instance) where TTo : TFrom
        {
            lock (instance)
                return instance.RegisterTypeAsTransient(typeof(TFrom), typeof(TTo));
        }

        [DebuggerStepThrough]
        public static IUnityContainer RegisterTypeAsPerResolve(this IUnityContainer instance, Type fromType, Type toType)
        {
            lock (instance)
                return instance.RegisterType(fromType, toType, new PerResolveLifetimeManager());
        }

        [DebuggerStepThrough]
        public static IUnityContainer RegisterInstanceAsPerResolve<TFrom>(this IUnityContainer instance, TFrom obj)
        {
            lock (instance)
                return instance.RegisterInstance<TFrom>(obj, new PerResolveLifetimeManager());
        }

        [DebuggerStepThrough]
        public static IUnityContainer RegisterTypeAsPerResolve<TFrom, TTo>(this IUnityContainer instance) where TTo : TFrom
        {
            lock (instance)
                return instance.RegisterTypeAsPerResolve(typeof(TFrom), typeof(TTo));
        }

        [DebuggerStepThrough]
        public static IUnityContainer RegisterMultipleTypesAsSingleton(this IUnityContainer instance, Type fromType, Type toType)
        {
            lock (instance)
                return instance.RegisterType(fromType, toType, toType.FullName, new ContainerControlledLifetimeManager());
        }

        [DebuggerStepThrough]
        public static IUnityContainer RegisterMultipleTypesAsSingleton<TFrom, TTo>(this IUnityContainer instance) where TTo : TFrom
        {
            lock (instance)
                return instance.RegisterMultipleTypesAsSingleton(typeof(TFrom), typeof(TTo));
        }

        [DebuggerStepThrough]
        public static IUnityContainer RegisterMultipleTypesAsTransient(this IUnityContainer instance, Type fromType, Type toType)
        {
            lock (instance)
                return instance.RegisterType(fromType, toType, toType.FullName, new TransientLifetimeManager());
        }

        [DebuggerStepThrough]
        public static IUnityContainer RegisterMultipleTypesAsTransient<TFrom, TTo>(this IUnityContainer instance) where TTo : TFrom
        {
            lock (instance)
                return instance.RegisterMultipleTypesAsTransient(typeof(TFrom), typeof(TTo));
        }

        [DebuggerStepThrough]
        public static IUnityContainer RegisterMultipleTypesAsPerResolve(this IUnityContainer instance, Type fromType, Type toType)
        {
            lock (instance)
                return instance.RegisterType(fromType, toType, toType.FullName, new PerResolveLifetimeManager());
        }

        [DebuggerStepThrough]
        public static IUnityContainer RegisterMultipleTypesAsPerResolve<TFrom, TTo>(this IUnityContainer instance) where TTo : TFrom
        {
            lock (instance)
                return instance.RegisterMultipleTypesAsPerResolve(typeof(TFrom), typeof(TTo));
        }

        [DebuggerStepThrough]
        public static T ResolveOne<T>(this IUnityContainer instance)
        {
            return instance.ResolveAll<T>().FirstOrDefault();
        }
    }
}
