[CmdletBinding(DefaultParameterSetName='Standard')]
param(
    [Parameter(Mandatory, ParameterSetName='Standard', HelpMessage="Path to the compiled plugin assembly (dll)")]
    [Parameter(Mandatory, ParameterSetName='Bitbucket', HelpMessage="Path to the compiled plugin assembly (dll)")]
    [Parameter(Mandatory, ParameterSetName='Archive', HelpMessage="Path to the compiled plugin assembly (dll)")]
    [string]$file,

    [Parameter(Mandatory=$false, ParameterSetName='Standard', HelpMessage="Marks the manifest for the beta channel")]
    [Parameter(Mandatory=$false, ParameterSetName='Bitbucket', HelpMessage="Marks the manifest for the beta channel")]
    [Parameter(Mandatory=$false, ParameterSetName='Archive', HelpMessage="Marks the manifest for the beta channel")]
    [switch]$beta,

    [Parameter(Mandatory=$false, ParameterSetName='Standard', HelpMessage="The installer url that should be put into the manifest")]
    [Parameter(Mandatory=$false, ParameterSetName='Archive', HelpMessage="The installer url that should be put into the manifest")]
    [string]$installerUrl,

    [Parameter(Mandatory=$false, ParameterSetName='Bitbucket', HelpMessage="If the assembly should be packed into a zip file")]
    [Parameter(Mandatory=$false, ParameterSetName='Archive', HelpMessage="If the assembly should be packed into a zip file")]
    [switch]$createArchive,

    [Parameter(Mandatory=$false, ParameterSetName='Bitbucket', HelpMessage="If all files in the assembly's directory should be included in the zip file")]
    [Parameter(Mandatory=$false, ParameterSetName='Archive', HelpMessage="If all files in the assembly's directory should be included in the zip file")]
    [switch]$includeAll,

    [Parameter(Mandatory=$false, ParameterSetName='Bitbucket', HelpMessage="Name of the zip archive")]
    [Parameter(Mandatory=$false, ParameterSetName='Archive', HelpMessage="Name of the zip archive")]
    [string]$archiveName,

    [Parameter(Mandatory=$false, ParameterSetName='Bitbucket', HelpMessage="Appends the version to the zip file name (e.g. MyPlugin.zip -> MyPlugin.1.0.0.0.zip)")]
    [Parameter(Mandatory=$false, ParameterSetName='Archive', HelpMessage="Appends the version to the zip file name (e.g. MyPlugin.zip -> MyPlugin.1.0.0.0.zip)")]
    [switch]$appendVersionToArchive,

    [Parameter(Mandatory=$false, ParameterSetName='Bitbucket', HelpMessage="Upload the file to bitbucket")]
    [switch]$uploadToBitbucket,

    [Parameter(ParameterSetName='Bitbucket', HelpMessage="Bitbucket username for upload")]
    [string]$bitbucketUserName,

    [Parameter(Mandatory=$false, ParameterSetName='Bitbucket', HelpMessage="Bitbucket app password for upload (see https://support.atlassian.com/bitbucket-cloud/docs/create-an-app-password/)")]
    [string]$bitbucketPassword,

    [Parameter(ParameterSetName='Bitbucket', HelpMessage="Bitbucket repository owner")]
    [string]$bitbucketRepositoryOwner,

    [Parameter(ParameterSetName='Bitbucket', HelpMessage="Bitbucket repository name")]
    [string]$bitbucketRepository

)

if($uploadToBitbucket -And [string]::IsNullOrEmpty($bitbucketPassword)) {    
    $bitbucketPassword = Read-Host -Prompt 'Enter bitbucket app password for upload'
}

Write-Output "Generating manifest from assembly"
Write-Output $file
Write-Output "-------------"
Write-Output "-------------"

$manifest = [ordered]@{
    Name = ""
    Identifier = ""
    Version = @{}
    Author = ""
    Homepage = ""
    Repository = ""
    License = ""
    LicenseURL = ""
    ChangelogURL = ""
    Tags = @()
    MinimumApplicationVersion = @{}
    Descriptions = [ordered]@{
        ShortDescription = ""
        LongDescription = ""
        FeaturedImageURL = ""
        ScreenshotURL = ""
        AltScreenshotURL = ""
    }
    Installer = @{}
}

$stream = [System.IO.File]::OpenRead($file)
$peReader = [System.Reflection.PortableExecutable.PEReader]::new($stream, [System.Reflection.PortableExecutable.PEStreamOptions]::LeaveOpen -bor [System.Reflection.PortableExecutable.PEStreamOptions]::PrefetchMetadata)
$metadataReader = [System.Reflection.Metadata.PEReaderExtensions]::GetMetadataReader($peReader)
$assemblyDefinition = $metadataReader.GetAssemblyDefinition()
$assemblyCustomAttributes = $assemblyDefinition.GetCustomAttributes()
$metadataCustomAttributes = $assemblyCustomAttributes | % {$metadataReader.GetCustomAttribute($_)}
foreach ($attribute in $metadataCustomAttributes) {

    $ctor = $metadataReader.GetMemberReference([System.Reflection.Metadata.MemberReferenceHandle]$attribute.Constructor)
    $attrType = $metadataReader.GetTypeReference([System.Reflection.Metadata.TypeReferenceHandle]$ctor.Parent)
    $attrName = $metadataReader.GetString($attrType.Name)
    $attrBlob = $metadataReader.GetBlobReader($attribute.Value)

    $attrBlob.ReadSerializedString();

    if($attrName -like "AssemblyTitleAttribute") {
        $attrVal = $attrBlob.ReadSerializedString()
		$manifest["Name"] = $attrVal
	}
	if($attrName -like "GuidAttribute") {
        $attrVal = $attrBlob.ReadSerializedString()
		$manifest["Identifier"] = $attrVal
	}
	if($attrName -like "AssemblyFileVersionAttribute") {
        $attrVal = $attrBlob.ReadSerializedString()
        $version = $attrVal.Split(".")
		$manifest["Version"] = [ordered]@{
            Major = $version[0]
            Minor = $version[1]
            Patch = $version[2]
            Build = $version[3]
        }
	}
	if($attrName -like "AssemblyCompanyAttribute") {
        $attrVal = $attrBlob.ReadSerializedString()
		$manifest["Author"] = $attrVal
	}
	if($attrName -like "AssemblyDescriptionAttribute") {
        $attrVal = $attrBlob.ReadSerializedString()
		$manifest["Descriptions"]["ShortDescription"] = $attrVal
	}

    if($attrName -like "AssemblyMetadataAttribute") {
        $attrKey = $attrBlob.ReadSerializedString()
        $attrVal = $attrBlob.ReadSerializedString()

        if($attrKey -like "Homepage" ) {
            $manifest["Homepage"] = $attrVal
        }
        if($attrKey -like "Repository" ) {
            $manifest["Repository"] = $attrVal
        }
        if($attrKey -like "License" ) {
            $manifest["License"] = $attrVal
        }
        if($attrKey -like "LicenseURL" ) {
            $manifest["LicenseURL"] = $attrVal
        }
        if($attrKey -like "ChangelogURL" ) {
            $manifest["ChangelogURL"] = $attrVal
        }
        if($attrKey -like "Tags" ) {
            $manifest["Tags"] = $attrVal.Split(",");
        }
        if($attrKey -like "MinimumApplicationVersion" ) {
            $version = $attrVal.Split(".");
            $manifest["MinimumApplicationVersion"] = [ordered]@{
                Major = $version[0]
                Minor = $version[1]
                Patch = $version[2]
                Build = $version[3]
            }
        }
        if($attrKey -like "LongDescription" ) {
            $manifest["Descriptions"]["LongDescription"] = $attrVal
        }
        if($attrKey -like "FeaturedImageURL" ) {
            $manifest["Descriptions"]["FeaturedImageURL"] = $attrVal
        }
        if($attrKey -like "ScreenshotURL" ) {
            $manifest["Descriptions"]["ScreenshotURL"] = $attrVal
        }
        if($attrKey -like "AltScreenshotURL" ) {
            $manifest["Descriptions"]["AltScreenshotURL"] = $attrVal
        }
    }
}

if($beta) {
    $manifest["Channel"] = "Beta"
}

$stream.Close();
$stream.Dispose();

$installerType = "DLL"

if($createArchive) {
    Write-Output "Creating zip archive"
    $installerType = "ARCHIVE"
	
    if(!$archiveName) {
        $archiveName = [io.path]::GetFileNameWithoutExtension($file)
        if($appendVersionToArchive) {
            $archiveName = "$($archiveName).$($manifest["Version"].Major).$($manifest["Version"].Minor).$($manifest["Version"].Patch).$($manifest["Version"].Build)"
            echo "Archive name: $($archiveName)"
        } 
        
    }
    $zipfile = $archiveName + ".zip"

    if(Test-Path $zipfile) {
        Remove-Item $zipfile
    }
	$compressFiles = $file
	
	if($includeAll) {
		$compressFiles = [System.IO.Path]::GetDirectoryName($file) + "\*"
	}
	
    Compress-Archive -Path $compressFiles -Destination $zipfile
    Write-Output "-------------"
    Write-Output "-------------"
    $checksum = Get-FileHash $zipfile
} else {
    $checksum = Get-FileHash $file
}

if($uploadToBitbucket) {
    $fileToUpload = $zipfile
    if([string]::IsNullOrEmpty($zipfile)) {
        $fileToUpload = $file
    }

    echo "File to upload: $($fileToUpload)"

    $uri = "https://api.bitbucket.org/2.0/repositories/$($bitbucketRepositoryOwner)/$($bitbucketRepository)/downloads"
    $installerUrl = "https://bitbucket.org/$($bitbucketRepositoryOwner)/$($bitbucketRepository)/downloads/$($fileToUpload)"

    $pair = ($bitbucketUserName + ':' + $bitbucketPassword)
    $encodedCreds = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes($pair))
    $basicAuthValue = ('Basic {0}' -f $encodedCreds)
    $headers = @{
      Authorization = $basicAuthValue
    }

    $Form = @{
        files = Get-Item -Path $fileToUpload
    }

    echo "Uploading the file $($fileToUpload) to $($uri)"    
    Invoke-RestMethod -Method POST -URI $uri -Form $Form -Headers $headers -ContentType "multipart/form-data" -TransferEncoding "chunked"
    
}

$manifest["Installer"] = [ordered]@{
    URL = $installerUrl
    Type = $installerType
    Checksum = $checksum.Hash
    ChecksumType = $checksum.Algorithm
}

function Format-Json([Parameter(Mandatory, ValueFromPipeline)][String] $json) {
    $indent = 0;
    ($json -Split "`n" | % {
        if ($_ -match '[\}\]]\s*,?\s*$') {
            $indent--
        }
        $line = ('    ' * $indent) + $($_.TrimStart() -replace '":  (["{[])', '": $1' -replace ':  ', ': ')
        if ($_ -match '[\{\[]\s*$') {
            $indent++
        }
        $line
    }) -Join "`n"
}
$json = ConvertTo-Json $manifest | Format-Json 
$json | Out-File "manifest.json" -Encoding Utf8
Write-Output "--------------------------"
Write-Output "Manifest JSON start"
Write-Output "--------------------------"
Write-Output $json
Write-Output "--------------------------"
Write-Output "Manifest JSON end"
Write-Output "--------------------------"
Write-Output "Manifest JSON created"
