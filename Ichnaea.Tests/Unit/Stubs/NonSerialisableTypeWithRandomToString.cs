namespace Restall.Ichnaea.Tests.Unit.Stubs
{
	public class NonSerialisableTypeWithRandomToString
	{
		private readonly string toString = StringGenerator.AnyNonNull();

		public override string ToString()
		{
			return this.toString;
		}
	}
}
