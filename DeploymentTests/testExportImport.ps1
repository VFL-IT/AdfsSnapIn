function RemoveRP($name)
{
  if (Get-ADFSRelyingPartyTrust -Name $name)
  {
   Remove-ADFSRelyingPartyTrust -TargetName $name
  }
}

function AddRP($name)
{
  $metadataFile = get-item ".\$name.xml"

  Add-ADFSRelyingPartyTrust -Name $name -MetadataFile $metadatafile
}

function ExercisePassiveWSFed()
{
  $passiveWSFedname = "SafeToDelete_GenericWSFedPassive";
  $passiveWSFednameImported = "$($passiveWSFedname)_IMPORTED";

  RemoveRP($passiveWSFedname)
  RemoveRP($passiveWSFednameImported)

  AddRP($passiveWSFedname)
  Add-ADFSRuleCvrNumber -Name $passiveWSFedname

  $passiveWSFednameExportFile = ".\exported$passiveWSFedname.xml"

  Export-ADFSPortableRelyingParty -Name $passiveWSFedname -Path $passiveWSFednameExportFile

  RemoveRP($passiveWSFedname)

  Import-ADFSPortableRelyingParty -Name $passiveWSFednameImported -Path $passiveWSFednameExportFile

  $importedRules = (Get-ADFSRelyingPartyTrust -Name $passiveWSFednameImported|select IssuanceTransformRules).IssuanceTransformRules
  if (-not ($importedRules -match "Issue CVR Number"))
  {
    throw "Something is wrong with imported rules";
  }

  RemoveRP($passiveWSFednameImported)
}

function ExerciseSAMLP()
{
  $samlPname = "SafeToDelete_GenericSAMLP";
  $samlPnameImported = "$($samlPname)_IMPORTED";

  RemoveRP($samlPname)
  RemoveRP($samlPnameImported)

  AddRP($samlPname)
  Add-ADFSRuleCvrNumber -Name $samlPname

  $samlPnameExportFile = ".\exported$samlPname.xml"

  Export-ADFSPortableRelyingParty -Name $samlPname -Path $samlPnameExportFile

  RemoveRP($samlPname)

  Import-ADFSPortableRelyingParty -Name $samlPnameImported -Path $samlPnameExportFile

  $importedSignatureAlg = (Get-ADFSRelyingPartyTrust -Name $samlPnameImported| select SignatureAlgorithm).SignatureAlgorithm
  
  $importedRules = (Get-ADFSRelyingPartyTrust -Name $samlPnameImported|select IssuanceTransformRules).IssuanceTransformRules
  if (-not ($importedRules -match "Issue CVR Number"))
  {
    throw "Something is wrong with imported rules";
  }

  RemoveRP($samlPnameImported)
}

function PokeSAMLToSha1()
{
  $samlPname = "SafeToDelete_GenericSAMLP";
  $samlPnameExportFile = ".\exported$samlPname.xml"
  RemoveRP($samlPname)
  $sha256 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
  $sha1 = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
  (get-content $samlPnameExportFile) | foreach-object {$_ -replace $sha256, $sha1} | set-content $samlPnameExportFile
  Import-ADFSPortableRelyingParty -Name $samlPname -Path $samlPnameExportFile
  
  $importedSignatureAlg = (Get-ADFSRelyingPartyTrust -Name $samlPname| select SignatureAlgorithm).SignatureAlgorithm
  
  if (-not ($importedSignatureAlg -eq $sha1))
  {
    throw("Unexpected signature algorithm after import: $importedSignatureAlg");
  }
  RemoveRP($samlPname)
}


ExercisePassiveWSFed

ExerciseSAMLP

PokeSAMLToSha1

rm exported*.xml