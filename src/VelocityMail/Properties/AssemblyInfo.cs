using System;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("VelocityMail")]
[assembly: AssemblyDescription("Apache Velocity-formatted e-mail templating service")]
[assembly: AssemblyProduct("VelocityMail")]
[assembly: AssemblyCopyright("Copyright ©  2016 Jonathan Needle")]

#if !PORTABLE && !PORTABLE40
[assembly: ComVisible(false)]
#endif
//[assembly: CLSCompliant(true)] - NVelocity isn't unfortunately.
[assembly: System.Resources.NeutralResourcesLanguage("en")]
