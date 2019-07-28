// CHEZ CHANGE begin: ARM platform implementation.
using System.Collections.Generic;
using Sharpmake.Generators;
using Sharpmake.Generators.VisualStudio;

namespace Sharpmake
{
    public static partial class Linux
    {
        [PlatformImplementation(Platform.linuxARM,
            typeof(IPlatformDescriptor),
            typeof(Project.Configuration.IConfigurationTasks),
            typeof(IPlatformVcxproj))]
        public sealed class ARMPlatform : BaseLinuxPlatform
        {
            #region IPlatformDescriptor implementation
            public override string SimplePlatformString => "ARM";
            #endregion

            #region IPlatformVcxproj implementation
            public override IEnumerable<string> GetImplicitlyDefinedSymbols(IGenerationContext context)
            {
                var defines = new List<string>();
                defines.AddRange(base.GetImplicitlyDefinedSymbols(context));
                defines.Add("LinuxARM");

                return defines;
            }

            public override void SetupPlatformTargetOptions(IGenerationContext context)
            {
                context.Options["TargetMachine"] = "MachineARM";
                context.CommandLineOptions["TargetMachine"] = "/MACHINE:ARM";
            }
            #endregion
        }
    }
}
// CHEZ CHANGE end: ARM platform implementation.

