using System.Diagnostics;
using System.Management.Automation;
using System.ComponentModel;
using System.Reflection;

// c:\Windows\Microsoft.NET\Framework64\v2.0.50727\InstallUtil.exe VflIt.Samples.AdfsSnapIn.dll
// (must be elevated)
// Add-PSSnapin DlbrCommonLoginAdfs 
// (or powershell modules)
namespace VflIt.Samples.AdfsSnapIn
{
    /// <summary>
   /// Create this sample as an PowerShell snap-in
   /// </summary>
   [RunInstaller(true)]
   public class DlbrCommonLoginAdfsSnapIn : PSSnapIn
   {
       /// <summary>
       /// Get a name for this PowerShell snap-in. This name will be used in registering
       /// this PowerShell snap-in.
       /// </summary>
       public override string Name
       {
           get
           {
               return "DlbrCommonLoginAdfs";
           }
       }

       /// <summary>
       /// Vendor information for this PowerShell snap-in.
       /// </summary>
       public override string Vendor
       {
           get
           {
               Assembly assembly = Assembly.GetExecutingAssembly();
               FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
               string version = fvi.ProductVersion;
               return string.Format("DLBR " + "(version {0})", version);
           }
       }

       /// <summary>
       /// Gets resource information for vendor. This is a string of format: 
       /// resourceBaseName,resourceName. 
       /// </summary>
       public override string VendorResource
       {
           get
           {
               return "DlbrCommonLoginAdfs,DLBR";
           }
       }

       /// <summary>
       /// Description of this PowerShell snap-in.
       /// </summary>
       public override string Description
       {
           get
           {
               return "TODO: Description";
           }
       }
   }
}
