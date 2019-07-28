// CHEZ CHANGE begin: x64 Linux platform.
using System.Collections.Generic;
using Sharpmake.Generators;
using Sharpmake.Generators.VisualStudio;

namespace Sharpmake
{
    public static partial class Linux
    {
        [PlatformImplementation(Platform.linux64,
            typeof(IPlatformDescriptor),
            typeof(Project.Configuration.IConfigurationTasks),
            typeof(IPlatformVcxproj))]
        public sealed class x64Platform : BaseLinuxPlatform
        {
            #region IPlatformDescriptor implementation
            public override string SimplePlatformString => "x64";
            #endregion

            #region IPlatformVcxproj implementation
            public override IEnumerable<string> GetImplicitlyDefinedSymbols(IGenerationContext context)
            {
                var defines = new List<string>();
                defines.AddRange(base.GetImplicitlyDefinedSymbols(context));
                defines.Add("Linux64");

                return defines;
            }

            public override void SetupPlatformTargetOptions(IGenerationContext context)
            {
                context.Options["TargetMachine"] = "MachineX64";
                context.CommandLineOptions["TargetMachine"] = "/MACHINE:X64";
            }
            #endregion
        }
    }
}
// CHEZ CHANGE end: x64 Linux platform.

