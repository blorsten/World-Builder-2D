using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MapGeneration.SaveSystem
{
    /// <summary>
    /// This Struct is used to save different types found by <see cref="MapDataSaver"/>
    /// </summary>
    public struct VariableInfo
    {
        //The kind of bindings on fields/properties we look for in the types.
        private const BindingFlags VariablesBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>
        /// The type we loaded.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Dictionary filled with all the properties and their default data.
        /// </summary>
        public Dictionary<PropertyInfo, object> PropertiesInfos { get; private set; }

        /// <summary>
        /// Dictionary filled with all the fields and their default data.
        /// </summary>
        public Dictionary<FieldInfo, object> FieldInfos { get; private set; }

        /// <summary>
        /// The constructor sets the type.
        /// </summary>
        /// <param name="type"></param>
        public VariableInfo(Type type) : this()
        {
            Type = type;
        }

        /// <summary>
        /// Given a specific monobehaviour type, read all the fields/properties and load their
        /// data + default values.
        /// </summary>
        /// <param name="monoBehaviour">Specific monobehaviour type to research.</param>
        public void Load(MonoBehaviour monoBehaviour)
        {
            FieldInfos = Type.GetFields(VariablesBindingFlags)
                .Where(info => info.GetCustomAttributes(typeof(PersistentData), false).Any())
                .ToDictionary(info => info, info => info.GetValue(monoBehaviour));

            PropertiesInfos = Type.GetProperties(VariablesBindingFlags)
                .Where(info => info.GetCustomAttributes(typeof(PersistentData), false).Any())
                .ToDictionary(info => info, info => info.GetValue(monoBehaviour, null));
        }
    }
}