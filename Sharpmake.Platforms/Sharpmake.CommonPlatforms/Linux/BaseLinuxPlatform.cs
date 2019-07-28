// Copyright (c) 2017 Ubisoft Entertainment
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;

// CHEZ CHANGE begin: added namespaces.
using Sharpmake.Generators;
using Sharpmake.Generators.VisualStudio; 
// CHEZ CHANGE end: added namespaces.

namespace Sharpmake
{
    public static partial class Linux
    {
        public partial class BaseLinuxPlatform : BasePlatform, Project.Configuration.IConfigurationTasks
        {
            #region IPlatformDescriptor implementation
            public override string SimplePlatformString => "Linux";
            public override bool IsMicrosoftPlatform => false; // No way!
            public override bool IsPcPlatform => true;
            public override bool IsUsingClang => false; // Maybe now? Traditionally GCC but only the GNU project is backing it now.
            public override bool HasDotNetSupport => false; // Technically false with .NET Core and Mono.
            public override bool HasSharedLibrarySupport => false; // Was not specified in sharpmake (probably because it was not implemented), but this is obviously wrong.
            #endregion

            #region Project.Configuration.IConfigurationTasks implementation
            public void SetupDynamicLibraryPaths(Project.Configuration configuration, DependencySetting dependencySetting, Project.Configuration dependency)
            {
                DefaultPlatform.SetupLibraryPaths(configuration, dependencySetting, dependency);
            }

            public void SetupStaticLibraryPaths(Project.Configuration configuration, DependencySetting dependencySetting, Project.Configuration dependency)
            {
                DefaultPlatform.SetupLibraryPaths(configuration, dependencySetting, dependency);
            }

            // CHEZ CHANGE begin: added GetOutputFileNamePrefix method.
            public override string GetOutputFileNamePrefix(IGenerationContext context, Project.Configuration.OutputType outputType)
            {
                switch (outputType)
                {
                    case Project.Configuration.OutputType.Exe:
                        return string.Empty;
                    case Project.Configuration.OutputType.Dll:
                        return "lib";
                    default:
                        return "lib";
                }
            }
            // CHEZ CHANGE end: added GetOutputFileNamePrefix method.

            public string GetDefaultOutputExtension(Project.Configuration.OutputType outputType)
            {
                switch (outputType)
                {
                    case Project.Configuration.OutputType.Exe:
                        return string.Empty;
                    case Project.Configuration.OutputType.Dll:
                        return "so";
                    default:
                        return "a";
                }
            }

            public IEnumerable<string> GetPlatformLibraryPaths(Project.Configuration configuration)
            {
                yield break;
            }
            #endregion

            #region IPlatformVcxproj implementation
            public override string ProgramDatabaseFileExtension => string.Empty;
            public override string SharedLibraryFileExtension => "so";
            public override string ExecutableFileExtension => string.Empty;

            // CHEZ CHANGE begin: added VC project generation for Linux platforms.
            public override void GeneratePlatformSpecificProjectDescription(IVcxprojGenerationContext context, IFileGenerator generator)
            {
                generator.Write(Vcxproj.Template.Project.ProjectDescriptionStartPlatformConditional);
                generator.Write(_projectDescriptionPlatformSpecific);
                generator.Write(Vcxproj.Template.Project.PropertyGroupEnd);
            }

            public override void GenerateProjectConfigurationGeneral(IVcxprojGenerationContext context, IFileGenerator generator)
            {
                var devEnv = context.DevelopmentEnvironment;
                var resolver = generator.Resolver;

                string remoteProjectPath = @"$(RemoteRootDir)";
                Options.Vc.RemoteBuild.ProjectDirectory projectDirectoryOption = Options.GetObject<Options.Vc.RemoteBuild.ProjectDirectory>(context.Configuration);
                if (projectDirectoryOption != null)
                {
                    remoteProjectPath += Util.WindowsSeparator + projectDirectoryOption.Value;
                    remoteProjectPath = remoteProjectPath.TrimEnd(Util.WindowsSeparator);
                }

                string remoteBinPath = remoteProjectPath + Util.WindowsSeparator + resolver.Resolve("[options.OutputDirectory]").TrimEnd(Util.WindowsSeparator);

                if (context.Configuration.StartWorkingDirectory.Length > 0)
                {
                    remoteBinPath = context.Configuration.StartWorkingDirectory;
                }

                string remoteCommand = remoteProjectPath + Util.WindowsSeparator + resolver.Resolve("[options.OutputFile]");

                using (generator.Declare("LinuxRemoteRootDir", @"~/" + devEnv.ToString())) // Should this be configurable?
                using (generator.Declare("LinuxRemoteProjectDir", remoteProjectPath.Replace(Util.WindowsSeparator, Util.UnixSeparator)))
                using (generator.Declare("LinuxRemoteDebuggerWorkingDirectory", remoteBinPath.Replace(Util.WindowsSeparator, Util.UnixSeparator)))
                using (generator.Declare("LinuxRemoteDebuggerCommand", remoteCommand.Replace(Util.WindowsSeparator, Util.UnixSeparator)))
                {
                    generator.Write(_projectConfigurationsGeneral);
                }
            }

            public override void SelectCompilerOptions(IGenerationContext context)
            {
                var options = context.Options;
                var cmdLineOptions = context.CommandLineOptions;
                var conf = context.Configuration;

                // We use the Makefile options for Linux

                context.SelectOption
                (
                Options.Option(Options.Makefile.Compiler.Warnings.Disable, () => { options["WarningLevel"] = "TurnOffAllWarnings"; cmdLineOptions["WarningLevel"] = "-w"; }),
                Options.Option(Options.Makefile.Compiler.Warnings.NormalWarnings, () => { options["WarningLevel"] = "EnableAllWarnings"; cmdLineOptions["WarningLevel"] = "-Wall"; }),
                Options.Option(Options.Makefile.Compiler.Warnings.MoreWarnings, () => { options["WarningLevel"] = "EnableAllWarnings"; cmdLineOptions["WarningLevel"] = "-Wall"; })
                );

                context.SelectOption
                (
                Options.Option(Options.Makefile.Compiler.GenerateDebugInformation.Disable, () => { options["DebugInformationFormat"] = "None"; cmdLineOptions["DebugInformationFormat"] = "-g0"; }),
                Options.Option(Options.Makefile.Compiler.GenerateDebugInformation.Enable, () => { options["DebugInformationFormat"] = "FullDebug"; cmdLineOptions["DebugInformationFormat"] = "-g2 -gdwarf-2"; })
                );

                context.SelectOption
                (
                Options.Option(Options.Makefile.Compiler.Exceptions.Disable, () => { options["ExceptionHandling"] = "Disabled"; cmdLineOptions["ExceptionHandling"] = "-fno-exceptions"; }),
                Options.Option(Options.Makefile.Compiler.Exceptions.Enable, () => { options["ExceptionHandling"] = "Enabled"; cmdLineOptions["ExceptionHandling"] = "-fexceptions"; })
                );

                context.SelectOption
                (
                Options.Option(Options.Makefile.Compiler.TreatWarningsAsErrors.Disable, () => { options["TreatWarningAsError"] = "false"; cmdLineOptions["TreatWarningAsError"] = RemoveLineTag; }),
                Options.Option(Options.Makefile.Compiler.TreatWarningsAsErrors.Enable, () => { options["TreatWarningAsError"] = "true"; cmdLineOptions["TreatWarningAsError"] = "-Werror"; })
                );
            }

            public override void SelectLinkerOptions(IGenerationContext context)
            {
                if (context.Options["OutputFileExtension"] != FileGeneratorUtilities.RemoveLineTag)
                {
                    context.Options["OutputFileExtension"] = context.Options["OutputFileExtension"].TrimEnd('.');
                }

                if (context.Options["OutputFile"] != FileGeneratorUtilities.RemoveLineTag)
                {
                    context.Options["OutputFile"] = context.Options["OutputFile"].TrimEnd('.');
                }

                context.SelectOption
                (
                Options.Option(Options.Vc.Linker.ShowProgress.NotSet, () => { context.Options["ShowProgress"] = "false"; context.CommandLineOptions["ShowProgress"] = FileGeneratorUtilities.RemoveLineTag; }),
                Options.Option(Options.Vc.Linker.ShowProgress.DisplaysSomeProgressMessages, () => { context.Options["ShowProgress"] = "true"; context.CommandLineOptions["ShowProgress"] = "-Wl,--stats"; }),
                Options.Option(Options.Vc.Linker.ShowProgress.DisplayAllProgressMessages, () => { context.Options["ShowProgress"] = "true"; context.CommandLineOptions["ShowProgress"] = "-Wl,--stats"; })
                );
            }

            public override void SelectPlatformAdditionalDependenciesOptions(IGenerationContext context)
            {
                context.Options["AdditionalLibraryDirectories"] = FileGeneratorUtilities.RemoveLineTag;
                context.Options["AdditionalDependencies"] = FileGeneratorUtilities.RemoveLineTag;
                context.Options["LibraryDependencies"] = FileGeneratorUtilities.RemoveLineTag;

                context.CommandLineOptions["AdditionalDependencies"] = FileGeneratorUtilities.RemoveLineTag;
                context.CommandLineOptions["AdditionalLibraryDirectories"] = FileGeneratorUtilities.RemoveLineTag;
                context.CommandLineOptions["LibraryDependencies"] = FileGeneratorUtilities.RemoveLineTag;

                var dependencies = new List<Project.Configuration>();
                dependencies.AddRange(context.Configuration.ResolvedPublicDependencies);

                // Sort by the number of dependencies to get a good starting point
                dependencies.Sort((Project.Configuration d0, Project.Configuration d1) =>
                {
                    return d1.ProjectGuidDependencies.Count.CompareTo(d0.ProjectGuidDependencies.Count);
                });

                var libPaths = new OrderableStrings();
                var libFiles = new OrderableStrings();

                foreach (var dependency in dependencies)
                {
                    string outputFileName = dependency.TargetFileName;

                    string outputPrefix = GetOutputFileNamePrefix(context, dependency.Output);
                    if (!string.IsNullOrEmpty(outputPrefix))
                    {
                        outputFileName = outputPrefix + outputFileName;
                    }

                    string outputExtension = GetDefaultOutputExtension(dependency.Output);
                    if (!string.IsNullOrEmpty(outputExtension))
                    {
                        outputFileName = outputFileName + "." + outputExtension;
                    }

                    string libPath = dependency.IntermediatePath + Util.WindowsSeparator + outputFileName;

                    foreach (var variable in Options.GetObjects<Options.Vc.RemoteBuild.Variable>(context.Configuration))
                    {
                        libPath = libPath.Replace(@"$(" + variable.Key + @")", variable.Value);
                    }

                    string remotePath = Util.PathGetRelative(dependency.ProjectPath.TrimEnd(Util.WindowsSeparator), libPath.TrimEnd(Util.WindowsSeparator));

                    Options.Vc.RemoteBuild.ProjectDirectory projectDirectoryOption = Options.GetObject<Options.Vc.RemoteBuild.ProjectDirectory>(dependency);
                    if (projectDirectoryOption != null)
                    {
                        remotePath = projectDirectoryOption.Value.TrimEnd(Util.WindowsSeparator) + Util.WindowsSeparator + remotePath;

                        // Unlike the include paths, this needs the RemoteRootDir variable and unix separators
                        remotePath = @"$(RemoteRootDir)" + Util.UnixSeparator + remotePath.Replace(Util.WindowsSeparator, Util.UnixSeparator);

                        libPaths.Add(remotePath);
                    }
                }

                foreach (var file in context.Configuration.LibraryFiles)
                {
                    if (file.Contains(Util.WindowsSeparator.ToString()))
                    {
                        string libPath = file;

                        Options.Vc.RemoteBuild.RelativeDirectory relativeDirectoryOption = Options.GetObject<Options.Vc.RemoteBuild.RelativeDirectory>(context.Configuration);
                        if (relativeDirectoryOption != null)
                        {
                            string basePath = relativeDirectoryOption.Value.TrimEnd(Util.WindowsSeparator);

                            foreach (var variable in Options.GetObjects<Options.Vc.RemoteBuild.Variable>(context.Configuration))
                            {
                                libPath = libPath.Replace(@"$(" + variable.Key + @")", variable.Value);
                            }

                            string remotePath = Util.PathGetRelative(basePath, libPath.TrimEnd(Util.WindowsSeparator));

                            remotePath = @"$(RemoteRootDir)" + Util.UnixSeparator + remotePath.Replace(Util.WindowsSeparator, Util.UnixSeparator);

                            libPaths.Add(remotePath);
                        }
                    }
                    else
                    {
                        libFiles.Add(file);
                    }
                }

                context.Options["AdditionalDependencies"] = string.Join(";", libPaths);
                context.Options["LibraryDependencies"] = string.Join(";", libFiles);
            }

            protected override IEnumerable<string> GetIncludePathsImpl(IGenerationContext context)
            {
                var includePaths1 = new OrderableStrings();

                var includePaths2 = new OrderableStrings();
                includePaths2.AddRange(context.Configuration.IncludePrivatePaths);
                includePaths2.AddRange(context.Configuration.IncludePaths);
                includePaths2.AddRange(context.Configuration.DependenciesIncludePaths);

                Options.Vc.RemoteBuild.RelativeDirectory relativeDirectoryOption = Options.GetObject<Options.Vc.RemoteBuild.RelativeDirectory>(context.Configuration);
                if (relativeDirectoryOption != null)
                {
                    string basePath = relativeDirectoryOption.Value.TrimEnd(Util.WindowsSeparator);

                    Options.Vc.RemoteBuild.ProjectDirectory projectDirectoryOption = Options.GetObject<Options.Vc.RemoteBuild.ProjectDirectory>(context.Configuration);
                    if (projectDirectoryOption != null)
                    {
                        basePath += Util.WindowsSeparator + projectDirectoryOption.Value.TrimEnd(Util.WindowsSeparator);
                    }

                    for (int i = 0; i < includePaths2.Count; ++i)
                    {
                        string includePath = includePaths2[i];

                        foreach (var variable in Options.GetObjects<Options.Vc.RemoteBuild.Variable>(context.Configuration))
                        {
                            includePath = includePath.Replace(@"$(" + variable.Key + @")", variable.Value);
                        }

                        string remotePath = Util.PathGetRelative(basePath, includePath.TrimEnd(Util.WindowsSeparator));

                        if (remotePath != includePath)
                        {
                            includePaths1.Add(remotePath);
                        }
                    }
                }

                includePaths1.AddRange(includePaths2);

                return includePaths1;
            }

            public override void GenerateUserConfigurationFile(Project.Configuration conf, IFileGenerator generator)
            {
                generator.Write(_userFileConfigurationGeneralTemplate);
            }
            // CHEZ CHANGE end: added VC project generation for Linux platforms.
            #endregion
        }
    }
}
