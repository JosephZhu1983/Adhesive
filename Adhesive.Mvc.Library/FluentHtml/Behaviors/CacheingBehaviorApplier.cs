using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Adhesive.Mvc.Library.FluentHtml.Elements;

namespace Adhesive.Mvc.Library.FluentHtml.Behaviors
{
    public class CacheingBehaviorApplier
    {
        private static readonly ThreadSafeDictionary<Type, TypeToBehaviorMap> cachedTypeToBehaviorMap = new ThreadSafeDictionary<Type, TypeToBehaviorMap>();
        private static readonly ThreadSafeDictionary<Type, Action<IBehaviorMarker, object>> cachedBehaviorExecuteActions = new ThreadSafeDictionary<Type, Action<IBehaviorMarker, object>>();

        public static void Apply<T>(IBehaviorMarker behavior, T target) where T : IElement
        {
            var actions = GetExecuteActions(behavior.GetType(), target.GetType());
            foreach (var action in actions)
            {
                action(behavior, target);
            }
        }

        private static IEnumerable<Action<IBehaviorMarker, object>> GetExecuteActions(Type behaviorType, Type targetType)
        {
            var mapping = cachedTypeToBehaviorMap.GetOrAdd(behaviorType, () => new TypeToBehaviorMap());
            return mapping.GetOrAdd(targetType, () => CreateExecuteActions(behaviorType, targetType));
        }

        private static IEnumerable<Action<IBehaviorMarker, object>> CreateExecuteActions(Type behaviorType, Type targetType)
        {
            var behaviorTypeSpec = new GenericTypeSpec
            {
                OpenGenericType = typeof(IBehavior<>),
                GenericParameterType = targetType
            };
            var orderedBehaviorTypeSpec = new GenericTypeSpec
            {
                OpenGenericType = typeof(IOrderedBehavior<>),
                GenericParameterType = targetType
            };
            // Convert to an array for performace reasons
            return behaviorType.GetInterfaces()
                .Where(x => behaviorTypeSpec.Matcher(x) || orderedBehaviorTypeSpec.Matcher(x))
                .Select(BehaviorActionSelector(behaviorType))
                .ToArray();
        }

        private static Func<Type, Action<IBehaviorMarker, object>> BehaviorActionSelector(Type behaviorType)
        {
            return x => cachedBehaviorExecuteActions.GetOrAdd(behaviorType, () => CreateExecuteAction(x));
        }

        private static Action<IBehaviorMarker, object> CreateExecuteAction(Type behaviorType)
        {
            var caller = new DynamicMethod("caller", null, new[] { typeof(IBehaviorMarker), typeof(object) },
                typeof(CacheingBehaviorApplier).Module);
            var ilGenerator = caller.GetILGenerator();

            // Cast behvaior to expected type
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Castclass, behaviorType);

            // Cast passed object to expected parameter type
            var parameterType = behaviorType.GetGenericArguments()[0];
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Castclass, parameterType);

            // Invoke Execute Method
            var execute = behaviorType.GetMethod("Execute", new[] { parameterType });
            ilGenerator.EmitCall(OpCodes.Callvirt, execute, null);

            ilGenerator.Emit(OpCodes.Ret);

            return (Action<IBehaviorMarker, object>)caller.CreateDelegate(typeof(Action<IBehaviorMarker, object>));
        }

        // For readability
        private class TypeToBehaviorMap : ThreadSafeDictionary<Type, IEnumerable<Action<IBehaviorMarker, object>>> { }
    }
}