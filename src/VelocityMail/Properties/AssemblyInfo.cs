using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("VelocityMail")]

#if !PORTABLE && !PORTABLE40
[assembly: ComVisible(false)]
#endif
//[assembly: CLSCompliant(true)] - NVelocity isn't unfortunately.
[assembly: System.Resources.NeutralResourcesLanguage("en")]
