using System.Collections;
using Commons.Collections;
using NVelocity.App;

namespace VelocityMail
{
    /// <summary>
    /// Helper factory class to create a <see cref="VelocityEngine"/>
    /// </summary>
    public static class VelocityEngineFactory
    {
        /// <summary>
        /// Creates a new <see cref="VelocityEngine"/> that searches for templates in the
        /// specified assembly in the given namespace.
        /// </summary>
        /// <param name="namespacePrefix">The namespace prefix to be added to the template
        /// names automatically</param>
        /// <param name="assemblyName">Name of the assembly containing the e-mail
        /// templates.</param>
        /// <returns>New VelocityEngine instance</returns>
        public static VelocityEngine Create(string namespacePrefix, string assemblyName)
        {
            var props = new ExtendedProperties();
            props.AddProperty("resource.loader", "assembly");

            props.AddProperty("assembly.resource.loader.class",
                "NVelocity.Runtime.Resource.Loader.AssemblyRelativeResourceLoader, NVelocity");

            props.AddProperty("assembly.resource.loader.assembly", assemblyName);
            props.AddProperty("assembly.resource.loader.prefix", namespacePrefix);

            return new VelocityEngine(props);
        }

        /// <summary>
        /// Creates a new <see cref="VelocityEngine"/> that searches for templates in the
        /// specified filesystem path.
        /// </summary>
        /// <param name="path">Directory name containing the templates.</param>
        /// <returns>New VelocityEngine instance</returns>
        public static VelocityEngine CreateUsingDirectory(string path)
        {
            var props = new ExtendedProperties();
            props.AddProperty("file.resource.loader.path", new ArrayList(new string[] { ".", path }));

            return new VelocityEngine(props);
        }

        /// <summary>
        /// Creates a new <see cref="VelocityEngine"/> that searches for templates in the
        /// specified filesystem path. If a matching template is not found there, it will
        /// try in the specified assembly in the given namespace.
        /// </summary>
        /// <param name="namespacePrefix">The namespace prefix to be added to the template
        /// names automatically when checking for templates in the given assembly</param>
        /// <param name="assemblyName">Name of the assembly containing the templates.</param>
        /// <param name="path">File system path containing the templates.</param>
        /// <remarks>The file system path is checked first for a matching template. If not
        /// found, a check is made in the assembly.</remarks>
        /// <returns>New VelocityEngine instance</returns>
        public static VelocityEngine Create(string namespacePrefix, string assemblyName, string path)
        {
            ExtendedProperties props = new ExtendedProperties();
            props.AddProperty("resource.loader", "file, assembly");

            // File system
            props.AddProperty("file.resource.loader.path", new ArrayList(new string[] { ".", path }));

            // Assembly
            props.AddProperty("assembly.resource.loader.class",
                "NVelocity.Runtime.Resource.Loader.AssemblyRelativeResourceLoader , NVelocity");

            props.AddProperty("assembly.resource.loader.assembly", assemblyName);
            props.AddProperty("assembly.resource.loader.prefix", namespacePrefix);

            return new VelocityEngine(props);
        }
    }
}
