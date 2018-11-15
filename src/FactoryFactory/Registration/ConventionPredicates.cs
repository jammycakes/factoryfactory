using System;
using System.Collections.Generic;
using System.Linq;
using FactoryFactory.Registration.Fluent;

namespace FactoryFactory.Registration
{
    public class ConventionPredicates : IConventionPredicates
    {
        private IList<Predicate<Type>> _predicates = new List<Predicate<Type>>();

        public IConventionPredicates Where(Predicate<Type> predicate)
        {
            _predicates.Add(predicate);
            return this;
        }

        public Predicate<Type> ToPredicate()
        {
            var predicates = new List<Predicate<Type>>(_predicates);
            return (t => predicates.All(p => p(t)));
        }
    }
}
