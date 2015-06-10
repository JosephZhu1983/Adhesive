//===================================================================================
// Microsoft Developer and Platform Evangelism
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
using System.ComponentModel.DataAnnotations;
using Adhesive.Domain;

namespace Adhesive.Persistence.Test.Classes
{
    public class Customer
        : Entity,  IAuditable, IVersionable, ISoftDeletable
    {
        [StringLength(256)]
        public string FirstName { get; set; }
        [StringLength(256)]
        public string LastName { get; set; }
        [StringLength(256)]
        public string FullName
        {
            get
            {
                return string.Format("{0}, {1}", this.LastName, this.FirstName);
            }
            set { }
        }
        [StringLength(256)]
        public string Telephone { get; set; }
        [StringLength(256)]
        public string Company { get; set; }
        public decimal CreditLimit { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        [ConcurrencyCheck]
        public long RowVersion { get; set; }
        public bool IsDeleted { get; set; }

    }
}
