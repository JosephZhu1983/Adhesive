using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Adhesive.Test.Mvc.Models
{
    public class Person
    {
        [StringLength(50)]
        public string Name { get; set; }
        public int Id { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IList<int> Roles { get; set; }
        public Parent Mother { get; set; }
        public Parent Father { get; set; }
        public Guid? EmployerId { get; set; }
        public IList<Color> FavoriteColors { get; set; }
        public string Shift { get; set; }
    }

    public class Parent
    {
        public string Name { get; set; }
    }

    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}