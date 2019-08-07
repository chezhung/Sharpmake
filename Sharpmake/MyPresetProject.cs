// CHEZ CHANGE begin: added pre-configured project class.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpmake
{
    /// <summary>
    /// The enum for the output folder location.
    /// </summary>
    public enum OutputFolderLocation
    {
        ProjectFolder  = 1,
        SolutionFolder = 2,
    }

    /// <summary>
    /// The pre-configured project class.
    /// </summary>
    public abstract class MyPresetProject : Project
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        public MyPresetProject() :
            base()
        {
            this.IsFileNameToLower       = false;
            this.IsTargetFileNameToLower = false;

            this.NoneFiles.Add("misc/PostBuild.bat");
            this.NoneFiles.Add("misc/PostBuild.sh");
        }

        /// <summary>
        /// Get or set the location to place the project output folder.
        /// </summary>
        public OutputFolderLocation OutputFolderLocation { get; set; } = OutputFolderLocation.ProjectFolder;

        /// <summary>
        /// Get or set the flag indicating whether the project is supposed to output an application.
        /// </summary>
        public bool IsApplication { get; set; } = true;

        /// <summary>
        /// Get or set the flag indicating whether to use the pre-compile header with the 
        /// default name.
        /// </summary>
        public bool UseDefaultPrecompiledHeader { get; set; } = true;

        /// <summary>
        /// Get or set the flag indicating whether to use the default post build script.
        /// </summary>
        public bool UseDefaultPostBuildScript { get; set; } = true;

        /// <summary>
        /// Get or set the extra arguments for the default post build script.
        /// </summary>
        public string DefaultPostBuildScriptArgs { get; set; } = string.Empty;

        /// <summary>
        /// Configure the project for all the configurations and platform targets.
        /// </summary>
        /// <param name="configuration">The project configuration.</param>
        /// <param name="target">The project target.</param>
        [Configure]
        public void ConfigureAll(
            Project.Configuration configuration, 
            Target target)
        {
            string mySourceRootPath = Util.NormalizePath(this.SourceRootPath, target.Platform);

            // Set configuration name with the default composition logic.
            configuration.Name = Util.ComposeConfigurationName(target);

            // Compose default project path.
            configuration.ProjectPath = string.Format(
                Util.NormalizePath("./projects/{0}/{1}", target.Platform),
                this.Name,
                Util.ComposeProjectPath(target));

            // The output and intermediate paths is always the same by default.
            configuration.IntermediatePath = Util.NormalizePath(@"[conf.ProjectPath]\obj\$(PlatformTarget)\$(Configuration)", target.Platform);
            configuration.TargetPath       = this.ComposeTargetPath(configuration, target);

            // Always add the project's source and header folders.
            configuration.IncludePrivatePaths.Add(Util.NormalizePath(@"[project.SourceRootPath]\src", target.Platform));
            configuration.IncludePaths.Add(Util.NormalizePath(@"[project.SourceRootPath]\include", target.Platform));

            // The pre-compiled header.
            if (this.UseDefaultPrecompiledHeader == true)
            {
                // Add the pre-compiled header with the default names.
                configuration.PrecompHeader = this.Name + "_PCH.h";
                configuration.PrecompSource = this.Name + "_PCH.cpp";
            }

            // Post build script.
            if (this.UseDefaultPostBuildScript == true)
            {
                string postBuildScriptPath = Util.NormalizePath($"../../../../{mySourceRootPath}/misc/PostBuild.", target.Platform);
                if (Util.IsMswinPlatform(target.Platform) == true)
                    postBuildScriptPath += "bat";
                else
                    postBuildScriptPath += "sh";

                string projectToSrcRootPath = Util.NormalizePath($"../../../../{mySourceRootPath}", target.Platform);
                string postBuildArgs = $"\"{configuration.TargetPath}\" \"{projectToSrcRootPath}\"";

                configuration.EventPostBuild.Add($"{postBuildScriptPath} {postBuildArgs} {this.DefaultPostBuildScriptArgs}");
            }

            // Set the project output type according to the target's output type.
            if (this.IsApplication == true)
            {
                configuration.Output = Configuration.OutputType.Exe;
            }
            else
            {
                if (target.OutputType == OutputType.Dll)
                    configuration.Output = Configuration.OutputType.Dll;
                else
                    configuration.Output = Configuration.OutputType.Lib;
            }

            // Set default character set to Unicode.
            configuration.Options.Add(Options.Vc.General.CharacterSet.Unicode);

            // Do custom configurations.
            this.CustomConfigure(configuration, target);
        }

        /// <summary>
        /// Do custom configuration.
        /// </summary>
        /// <param name="configuration">The project configuration.</param>
        /// <param name="target">The project target.</param>
        public virtual void CustomConfigure(
            Project.Configuration configuration,
            Target target)
        {
            // Do nothing here, override to apply custom configurations.
        }

        /// <summary>
        /// Resolve filter path.
        /// </summary>
        /// <param name="relativePath">The path in the file system.</param>
        /// <param name="filterPath">The filtered path.</param>
        /// <returns>True to use the path, or false to ignore the item.</returns>
        public override bool ResolveFilterPath(
            string relativePath, 
            out string filterPath)
        {
            filterPath = null;
            if (string.IsNullOrEmpty(relativePath) == false)
            {
                if (relativePath.StartsWith("src") == true)
                {
                    filterPath = "Source Files" + relativePath.Substring(3);
                    return true;
                }
                else if (relativePath.StartsWith("include") == true)
                {
                    filterPath = "Header Files" + relativePath.Substring(7);
                    return true;
                }
                else if (relativePath.StartsWith("misc") == true)
                {
                    filterPath = "Misc Files" + relativePath.Substring(4);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Compose target path.
        /// </summary>
        /// <param name="configuration">The project configuration.</param>
        /// <param name="target">The target for the project.</param>
        /// <returns>The target path.</returns>
        private string ComposeTargetPath(
            Project.Configuration configuration, 
            Target target)
        {
            if (this.OutputFolderLocation == OutputFolderLocation.SolutionFolder)
            {
                return string.Format(
                    Util.NormalizePath(@"$(SolutionDir)bin\{0}\{1}\$(PlatformTarget)\$(Configuration)", target.Platform),
                    Util.GetGeneralPlatformName(target.Platform),
                    Util.GetDevEnvString(target.DevEnv));
            }
            else
            {
                return Util.NormalizePath(@"[conf.ProjectPath]\bin\$(PlatformTarget)\$(Configuration)", target.Platform);
            }
        }
    }
}
// CHEZ CHANGE end: added pre-configured project class.

