# Is this a 64 bit process
function Test-Win64() {
    return [IntPtr]::size -eq 8
}

$assemblyName = "VflIt.Samples.AdfsSnapIn"
$assembly = resolve-path ".\$assemblyName.dll"
$installFolder = "$($env:programfiles)\DLBR\VflIt.Samples.AdfsSnapIn"
$installFolderExists = test-path $installFolder

$is64Bit = Test-Win64

if (-not $is64Bit)
{
  throw("This installer must be run within a 64-bit PowerShell console")
}

$adfsInstalled = Test-Path "$($env:programfiles)\Active Directory Federation Services 2.0\Microsoft.IdentityServer.Powershell.dll"
if (-not $adfsInstalled)
{
  throw("This installer requires ADFS 2.0 to be installed")
}

$hasAssemblies = (test-path $assembly) -and (test-path ".\$assemblyName.pdb")

if (-not $hasAssemblies)
{
  throw("Missing assembly or pdb to deploy");
}

$installUtil64 = "$($env:windir)\Microsoft.NET\Framework64\v2.0.50727\InstallUtil.exe"
$installUtil =   "$($env:windir)\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe"

"Uninstalling existing versions (if any) from 32 bit PowerShell"
&$installUtil /LogToConsole=false /LogFile=.\uninstalllog32.txt /u $assembly |out-null

"Uninstalling existing versions (if any) from 64 bit PowerShell"
&$installUtil64 /LogToConsole=false /LogFile=.\uninstalllog64.txt /u $assembly |out-null

if ($installFolderExists)
{
  "Removing old versions from install folder $installFolder"
  rm "$installFolder\$assemblyName.*"
}
else
{
  mkdir $installFolder | out-null
}
"Copying assembly"
copy ".\$assemblyName.*" $installFolder

"Installing to 32 bit PowerShell"
&$installUtil /LogToConsole=false /LogFile=.\installlog32.txt $assembly |out-null

"Installing to 64 bit PowerShell"
&$installUtil64 /LogToConsole=false /LogFile=.\installlog64.txt $assembly |out-null

"Done"