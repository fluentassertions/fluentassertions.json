properties {
    $BaseDirectory = Resolve-Path ..     
    $SrcDirectory ="$BaseDirectory\Src"
    $TestsDirectory ="$BaseDirectory\Tests"
    $Nuget = "$BaseDirectory\.nuget\NuGet.exe"
    $SlnFile = "$BaseDirectory\FluentAssertions.Json.sln"
    $7zip = "$BaseDirectory\Lib\7z.exe"
    $GitVersionExe = "$BaseDirectory\Lib\GitVersion.exe"
    $ArtifactsDirectory = "$BaseDirectory\Artifacts"

    $NuGetPushSource = ""
    
    $MsBuildLoggerPath = ""
    $Branch = ""
    $MsTestPath = "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\MSTest.exe"
    $RunTests = $false
}

task default -depends Clean, ApplyAssemblyVersioning, ApplyPackageVersioning, RestoreNugetPackages, Compile, RunTests, BuildZip, BuildJsonPackage, PublishToMyget

task Clean {    
	Get-ChildItem $PackageDirectory *.nupkg | foreach { Remove-Item $_.FullName }
	Get-ChildItem $PackageDirectory *.zip | foreach { Remove-Item $_.FullName }
}

task ExtractVersionsFromGit {

        $json = . "$GitVersionExe" "$BaseDirectory" 

        if ($LASTEXITCODE -eq 0) {
            $version = (ConvertFrom-Json ($json -join "`n"));

            TeamCity-SetBuildNumber $version.FullSemVer;

            $script:AssemblyVersion = $version.AssemblySemVer;
            $script:InfoVersion = $version.InformationalVersion;
            $script:NuGetVersion = $version.NuGetVersion;
        }
        else {
            Write-Output $json -join "`n";
        }
}

task ApplyAssemblyVersioning -depends ExtractVersionsFromGit {
    
	$infos = Get-ChildItem -Path $SrcDirectory -Filter SolutionInfo.cs -Recurse
	
	foreach ($info in $infos) {
		Write-Host "Updating " + $info.FullName
		Set-ItemProperty -Path $info.FullName -Name IsReadOnly -Value $false
		
		$content = Get-Content $info.FullName
		$content = $content -replace 'AssemblyVersion\("(.+)"\)', ('AssemblyVersion("' + $script:AssemblyVersion + '")')
		$content = $content -replace 'AssemblyFileVersion\("(.+)"\)', ('AssemblyFileVersion("' + $script:AssemblyVersion + '")')
		$content = $content -replace 'AssemblyInformationalVersion\("(.+)"\)', ('AssemblyInformationalVersion("' + $script:InfoVersion + '")')
		Set-Content -Path $info.FullName $content
	}   
}

task ApplyPackageVersioning -depends ExtractVersionsFromGit {
    
	$fullName = "$SrcDirectory\FluentAssertions.nuspec"

	Set-ItemProperty -Path $fullName -Name IsReadOnly -Value $false
	
	$content = Get-Content $fullName
	$content = $content -replace '<version>.+</version>', ('<version>' + "$script:NuGetVersion" + '</version>')
	Set-Content -Path $fullName $content
}

task RestoreNugetPackages {
        
	& $Nuget restore "$SlnFile"  
	& $Nuget install "$BaseDirectory\Build\packages.config" -OutputDirectory "$BaseDirectory\Packages" -ConfigFile "$BaseDirectory\NuGet.Config"
}

task Compile {
       
	exec { msbuild /v:m /p:Platform="Any CPU" $SlnFile /p:Configuration=Release /p:SourceAnalysisTreatErrorsAsWarnings=false /t:Rebuild }
}

task RunTests -precondition { return $RunTests -eq $true } {
    TeamCity-Block "Running unit tests" {
    
        Get-ChildItem $ArtifactsDirectory *.trx | ForEach { Remove-Item $_.FullName }

		exec {
            . $MsTestPath /nologo /noprompt `
                /testSettings:"$TestsDirectory\Default.testsettings" `
                /detail:duration /detail:errormessage /detail:errorstacktrace /detail:stdout `
                /testcontainer:"$TestsDirectory\FluentAssertions.Json.Net45.Specs\bin\Release\FluentAssertions.Json.Net45.Specs.dll" `
                /resultsfile:"$ArtifactsDirectory\FluentAssertions.Json.Net45.Specs.trx"
        }
    }
}

task BuildZip {
    TeamCity-Block "Zipping up the binaries" {
        $assembly = Get-ChildItem -Path "$ArtifactsDirectory\Lib" -Filter FluentAssertions.dll -Recurse | Select-Object -first 1
                
        $versionNumber = $assembly.VersionInfo.FileVersion

        & $7zip a -r "$ArtifactsDirectory\Fluent.Assertions.$versionNumber.zip" "$ArtifactsDirectory\Lib\*" -y
    }
}

task BuildJsonPackage -depends ExtractVersionsFromGit {
    TeamCity-Block "Building NuGet Package (Json)" {  
        & $Nuget pack "$SrcDirectory\FluentAssertions.nuspec" -o "$ArtifactsDirectory\" -Properties Version=$script:NuGetVersion 
    }
}

task PublishToMyget -precondition { return $env:NuGetApiKey } {
    TeamCity-Block "Publishing NuGet Package to Myget" {  
        $packages = Get-ChildItem $ArtifactsDirectory *.nupkg
        
        foreach ($package in $packages) {
        
            if ($NuGetPushSource) {
                & $Nuget push $package.FullName $env:NuGetApiKey -Source "$NuGetPushSource"
            } else {
                & $Nuget push $package.FullName $env:NuGetApiKey 
            }
        }
    }
}


