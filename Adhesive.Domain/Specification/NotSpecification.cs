//===================================================================================
// Microsoft Developer & Platform Evangelism
//=================================================================================== 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
// This code is released under the terms of the MS-LPL license, 
// http://microsoftnlayerapp.codeplex.com/license
//===================================================================================


using System;
using System.Linq;
using System.Linq.Expressions;

namespace Adhesive.Domain.Specification
{
    /// <summary>
    /// NotEspecification convert a original
    /// specification with NOT logic operator
    /// </summary>
    /// <typeparam name="TValueObject">Type of element for this specificaiton</typeparam>
    public sealed class NotSpecification<TEntity>
        : Specification<TEntity>
        where TEntity : class
    {
        #region Members

        Expression<Func<TEntity, bool>> _OriginalCriteria;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for NotSpecificaiton
        /// </summary>
        /// <param name="originalSpecification">Original specification</param>
        public NotSpecification(ISpecification<TEntity> originalSpecification)
        {

            if (originalSpecification == (ISpecification<TEntity>)null)
                throw new ArgumentNullException("originalSpecification");

            _OriginalCriteria = originalSpecification.SatisfiedBy();
        }

        /// <summary>
        /// Constructor for NotSpecification
        /// </summary>
        /// <param name="originalSpecification">Original specificaiton</param>
        public NotSpecification(Expression<Func<TEntity, bool>> originalSpecification)
        {
            if (originalSpecification == (Expression<Func<TEntity, bool>>)null)
                throw new ArgumentNullException("originalSpecification");

            _OriginalCriteria = originalSpecification;
        }

        #endregion

        #region Override Specification methods

        /// <summary>
        /// <see cref="ISpecification{TEntity}"/>
        /// </summary>
        /// <returns><see cref="ISpecification{TEntity}"/></returns>
        public override Expression<Func<TEntity, bool>> SatisfiedBy()
        {

            return Expression.Lambda<Func<TEntity, bool>>(Expression.Not(_OriginalCriteria.Body),
                                                         _OriginalCriteria.Parameters.Single());
        }

        #endregion
    }
}
