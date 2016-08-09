# SetAssemblyVersion.ps1
#
# http://www.luisrocha.net/2009/11/setting-assembly-version-with-windows.html
# http://blogs.msdn.com/b/dotnetinterop/archive/2008/04/21/powershell-script-to-batch-update-assemblyinfo-cs-with-new-version.aspx
# http://jake.murzy.com/post/3099699807/how-to-update-assembly-version-numbers-with-teamcity
# https://github.com/ferventcoder/this.Log/blob/master/build.ps1#L6-L19
# https://gist.github.com/derekgates/4678882
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$true,Position=1)]
    [string]$version,
    [string]$infoVersion=$version,
    [string]$path=$pwd
)

function Update-SourceFiles($ver, $infover)
{
    $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)';
    $fileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)';
    $informationalfileVersionPattern = 'AssemblyInformationalVersion\(".*"\)';

    foreach ($o in $input)
    {
        Write-Host "Updating '$($o.FullName)' -> $ver ($infover)";

        (Get-Content $o.FullName) | ForEach-Object  { 
           $_ -replace $assemblyVersionPattern, "AssemblyVersion(""$ver"")" `
              -replace $fileVersionPattern, "AssemblyFileVersion(""$ver"")" `
              -replace $informationalfileVersionPattern, "AssemblyInformationalVersion(""$infover"")"
        } | Out-File $o.FullName -encoding UTF8 -force
    }
}

function Update-Files($ver, $infover, $path)
{
    Write-Host "Setting version to $ver";
    Write-Host "Setting informational version to $infover";
    Write-Host "path=$path";    

   foreach ($file in "AssemblyInfo.cs", "AssemblyInfo.vb", "GlobalAssemblyInfo.cs", "GlobalAssemblyInfo.vb" ) 
   {
        get-childitem $path -recurse |? {$_.Name -eq $file} | Update-SourceFiles $ver $infover;
   }
}

Update-Files -ver $version -infover $infoVersion -path $path;
