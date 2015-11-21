using System.ComponentModel;

namespace Restall.Ichnaea.Demo
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class CodeAnalysisJustification
	{
		public const string ReflectedByNancy = "Reflected by Nancy";
		public const string ReflectedByNancyServiceRouting = "Reflected by Nancy.ServiceRouting";
		public const string DtoParticipatesInSerialisation = "DTO participates in serialisation";
		public const string UsedInView = "Used in View";
		public const string IchnaeaSubscribes = "Ichnaea subscribes";
		public const string JsonSerialisation = "JSON Serialisation";
	}
}
