// CHEZ CHANGE begin: added template for Linux vcxproj.
namespace Sharpmake
{
    public static partial class Linux
    {
        public partial class BaseLinuxPlatform
        {
            private const string _projectDescriptionPlatformSpecific =
@"    <Keyword>Linux</Keyword>
    <ApplicationType>Linux</ApplicationType>
    <ApplicationTypeRevision>1.0</ApplicationTypeRevision>
    <TargetLinuxPlatform>Generic</TargetLinuxPlatform>
    <LinuxProjectType>{2238F9CD-F817-4ECC-BD14-2524D2669B35}</LinuxProjectType>
";

            private const string _projectConfigurationsGeneral =
@"  <PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='[conf.Name]|[platformName]'"" Label=""Configuration"">
    <ConfigurationType>[options.ConfigurationType]</ConfigurationType>
    <UseDebugLibraries>[options.UseDebugLibraries]</UseDebugLibraries>
    <CharacterSet>[options.CharacterSet]</CharacterSet>
    <WholeProgramOptimization>[options.WholeProgramOptimization]</WholeProgramOptimization>
    <PlatformToolset>Remote_GCC_1_0</PlatformToolset>
    <RemoteRootDir>[LinuxRemoteRootDir]</RemoteRootDir>
    <RemoteProjectDir>[LinuxRemoteProjectDir]</RemoteProjectDir>
    <RemoteDebuggerWorkingDirectory>[LinuxRemoteDebuggerWorkingDirectory]</RemoteDebuggerWorkingDirectory>
    <RemoteDebuggerCommand>[LinuxRemoteDebuggerCommand]</RemoteDebuggerCommand>
  </PropertyGroup>
";

            private const string _userFileConfigurationGeneralTemplate =
                @"    <LocalDebuggerCommand>[conf.VcxprojUserFile.LocalDebuggerCommand]</LocalDebuggerCommand>
    <LocalDebuggerCommandArguments>[conf.VcxprojUserFile.LocalDebuggerCommandArguments]</LocalDebuggerCommandArguments>
    <LocalDebuggerEnvironment>[conf.VcxprojUserFile.LocalDebuggerEnvironment]</LocalDebuggerEnvironment>
    <LocalDebuggerWorkingDirectory>[conf.VcxprojUserFile.LocalDebuggerWorkingDirectory]</LocalDebuggerWorkingDirectory>
    <DebuggerFlavor>LinuxDebugger</DebuggerFlavor>
    <PreLaunchCommand>[conf.VcxprojUserFile.PreLaunchCommand]</PreLaunchCommand>
";
        }
    }
}
// CHEZ CHANGE end: added template for Linux vcxproj.

