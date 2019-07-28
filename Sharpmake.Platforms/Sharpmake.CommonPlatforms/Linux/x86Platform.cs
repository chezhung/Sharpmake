// CHEZ CHANGE begin: x86 Linux platform.
using System.Collections.Generic;
using Sharpmake.Generators;
using Sharpmake.Generators.VisualStudio;

namespace Sharpmake
{
    public static partial class Linux
    {
        [PlatformImplementation(Platform.linux32,
            typeof(IPlatformDescriptor),
            typeof(Project.Configuration.IConfigurationTasks),
            typeof(IPlatformVcxproj))]
        public sealed class x86Platform : BaseLinuxPlatform
        {
            #region IPlatformDescriptor implementation
            public override string SimplePlatformString => "x86";
            #endregion

            #region IPlatformVcxproj implementation
            public override IEnumerable<string> GetImplicitlyDefinedSymbols(IGenerationContext context)
            {
                var defines = new List<string>();
                defines.AddRange(base.GetImplicitlyDefinedSymbols(context));
                defines.Add("Linux32");

                return defines;
            }

            public override void SetupPlatformTargetOptions(IGenerationContext context)
            {
                context.Options["TargetMachine"] = "MachineX86";
                context.CommandLineOptions["TargetMachine"] = "/MACHINE:X86";
            }
            #endregion
        }
    }
}
// CHEZ CHANGE end: x86 Linux platform.

