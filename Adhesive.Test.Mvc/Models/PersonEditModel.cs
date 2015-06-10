using System.Collections.Generic;
using System.Web.Mvc;

namespace Adhesive.Test.Mvc.Models
{
    public class PersonEditModel
    {
        public Person Person;
        public IDictionary<string, string> Genders;
        public IEnumerable<Role> Roles;
        public SelectList Companies;
        public string[] Shifts;
    }
}