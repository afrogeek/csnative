﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetadataGenericContext.cs" company="">
//   
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PEAssemblyReader
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// </summary>
    public class MetadataGenericContext : IGenericContext
    {
        /// <summary>
        /// </summary>
        public MetadataGenericContext()
        {
            this.Map = new SortedDictionary<IType, IType>();
        }

        /// <summary>
        /// </summary>
        /// <param name="type">
        /// </param>
        public MetadataGenericContext(IType type)
            : this()
        {
            this.Init(type);
            Debug.Assert(!this.IsEmpty);
        }

        /// <summary>
        /// </summary>
        /// <param name="method">
        /// </param>
        public MetadataGenericContext(IMethod method)
            : this()
        {
            this.Init(method.DeclaringType);
            if (method.IsGenericMethod)
            {
                this.MethodDefinition = method;
                this.MethodSpecialization = method;
            }

            Debug.Assert(!this.IsEmpty);
        }

        /// <summary>
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.TypeDefinition == null && this.TypeSpecialization == null && this.MethodDefinition == null && this.MethodSpecialization == null;
            }
        }

        /// <summary>
        /// </summary>
        public IDictionary<IType, IType> Map { get; private set; }

        /// <summary>
        /// </summary>
        public IMethod MethodDefinition { get; set; }

        /// <summary>
        /// </summary>
        public IMethod MethodSpecialization { get; set; }

        /// <summary>
        /// </summary>
        public IType TypeDefinition { get; set; }

        /// <summary>
        /// </summary>
        public IType TypeSpecialization { get; set; }

        public static IGenericContext DiscoverFrom(IMethod method)
        {
            if (method.IsGenericMethod)
            {
                return new MetadataGenericContext(method);
            }

            var declType = method.DeclaringType;
            while (declType != null)
            {
                if (declType.IsGenericType || declType.IsGenericTypeDefinition)
                {
                    return new MetadataGenericContext(declType);
                }

                if (declType.IsNested)
                {
                    declType = declType.DeclaringType;
                    continue;
                }

                break;
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="type">
        /// </param>
        private void Init(IType type)
        {
            if (type.IsGenericTypeDefinition)
            {
                this.TypeDefinition = type;
            }

            if (type.IsGenericType)
            {
                this.TypeSpecialization = type;
            }
        }
    }
}