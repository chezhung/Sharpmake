// CHEZ CHANGE begin: added pre-configured solution class.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpmake
{
    /// <summary>
    /// The pre-configured solution class.
    /// </summary>
    public class MyPresetSolution : Solution
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        public MyPresetSolution() :
            base()
        {
            this.IsFileNameToLower = false;
        }

        /// <summary>
        /// Configure the project for all the configurations and platform targets.
        /// </summary>
        /// <param name="configuration">The solution configuration.</param>
        /// <param name="target">The solution target.</param>
        [Configure]
        public virtual void ConfigureAll(
            Solution.Configuration configuration, 
            Target target)
        {
            configuration.Name = Util.ComposeConfigurationName(target);

            // Compose the solution path according to the current target.
            configuration.SolutionPath = "./solutions/" + Util.ComposeSolutionPath(target);

            // Do custom configurations.
            this.CustomConfigure(configuration, target);
        }

        /// <summary>
        /// Do custom configuration.
        /// </summary>
        /// <param name="configuration">The solution configuration.</param>
        /// <param name="target">The solution target.</param>
        public virtual void CustomConfigure(
            Solution.Configuration configuration,
            Target target)
        {
            // Do nothing here, override to apply custom configurations.
        }
    }
}
// CHEZ CHANGE begin: added pre-configured solution class.

