using System.Runtime.CompilerServices;

namespace Autorepair
{
	public static class MyModuleInitializer
	{
		[ModuleInitializer]
		public static void Initialize()
		{
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
		}
	}
}
