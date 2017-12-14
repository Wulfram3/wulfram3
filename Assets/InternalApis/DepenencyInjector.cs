using Assets.InternalApis.Implementations;
using Assets.InternalApis.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.InternalApis
{
    public static class DepenencyInjector
    {
        private static readonly IDictionary<Type, Type> types = new Dictionary<Type, Type>();

        private static readonly IDictionary<Type, object> typeInstances = new Dictionary<Type, object>();

        public static void Register<TContract, TImplementation>()
        {
            types[typeof(TContract)] = typeof(TImplementation);
        }

        public static void Register<TContract, TImplementation>(TImplementation instance)
        {
            typeInstances[typeof(TContract)] = instance;
        }

        public static T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public static object Resolve(Type contract)
        {
            if (typeInstances.ContainsKey(contract))
            {
                Debug.Log("Resolve Instance:" + contract.Name);
                return typeInstances[contract];
            }
            else
            {
                Debug.Log("Resolve New:" + contract.Name);
                Type implementation = types[contract];
                ConstructorInfo constructor = implementation.GetConstructors().ToList()[0];
                ParameterInfo[] constructorParameters = constructor.GetParameters();
                if (constructorParameters.Length == 0)
                {
                    return Activator.CreateInstance(implementation);
                }

                List<object> parameters = new List<object>(constructorParameters.Length);
                foreach (ParameterInfo parameterInfo in constructorParameters)
                {
                    parameters.Add(Resolve(parameterInfo.ParameterType));
                }
                return constructor.Invoke(parameters.ToArray());
            }
        }

        public static void SetupInjection()
        {
            Register<IInternalStorage, JavascriptLocalStorage>();
            Register<IUserController, UserController>(new UserController());
            Register<IDiscordApi, DiscordApi>();
        }
    }
}