using FactoryFactory.Tests.Model.Interfaces;

namespace FactoryFactory.Tests
{
    /*
     * A "rogue" interface implementation outside its target namespace.
     * This should not be picked up by the convention tests.
     */
    public class RogueFoo : IFoo
    {

    }
}
