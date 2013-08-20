# Product information
properties {
	$version = "0.0.0.1"
	$product = "aCudaResearch"
    $company = "Tomasz Kujawa"
    $copyright = "(C) 2011 Tomasz Kujawa"
}

# Directories & files information
properties {
    $base_dir = Resolve-Path .
	$lib_dir = "$base_dir\Libraries"
	$build_dir = "$base_dir\Build"
	$source_dir = "$base_dir\aCuda\aCudaResearch"
	$release_dir = "$base_dir\Release"
	
	$sln_file = "$source_dir\$product.sln"
} 

include ".\Libraries\PsakeExt\psake-ext.ps1"
$framework = '4.0'

FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))
task default -depends Logo, Release

task GetVersion -description "Sets version property according to type of build (local or CI)" {
	#TODO: fetch version information from environment variables provided by Hudson CI
}

task GetProjects -description "Identifies all projects in product" {
    [array] $script:projects = @()
    $project_files = Get-ChildItem -Filter "*.csproj" -Path $source_dir -Recurse
    
    foreach($project_file in $project_files) {
        $project_name = $project_file.Name.Substring(0, $project_file.Name.Length - ".csproj".Length)

        # validate that project's name matches it's location
        $expected_directory = "$source_dir\$project_name"
        Assert ($expected_directory -eq $project_file.DirectoryName) "Project name doesn't match directory name: $($project_file.FullName)"
        
        # try to get project's description
        $description = ""
        $readme_file = $project_file.DirectoryName + "\readme.txt"
        if(Test-Path $readme_file) {
            $description = Get-Content $readme_file -TotalCount 1
        }
        
        $project = New-Object PSObject -Property @{Name = $project_name; Description = $description}
        $script:projects = $script:projects + $project
    }
}

task ListProjects -depends GetProjects {
    $script:projects | Select-Object Name, Description | Format-Table
}

task Logo -depends GetVersion -description "Displays build header" -action {
	"----------------------------------------------------------------------"
	"Building $product version $version"
	"----------------------------------------------------------------------"
	Write-Output "Base dir is: $base_dir"
}

task Clean {
	Write-Host "Creating BuildArtifacts directory" -ForegroundColor Green
    Remove-Item -Recurse -Force $release_dir -ErrorAction SilentlyContinue
    Remove-Item -Recurse -Force $build_dir -ErrorAction SilentlyContinue

    Write-Host "Cleaning $sln_file" -ForegroundColor Green
	Exec { msbuild "$sln_file" /t:Clean /p:Configuration=Release /v:quiet } 
}
 
task Init -depends Clean, GetProjects {
    New-Item $release_dir -ItemType directory | Out-Null
    New-Item $build_dir -ItemType directory | Out-Null
    Copy-Item $source_dir\aCudaResearch\Data\Settings.xml $build_dir\Settings.xml
    
    # generate assembly infos
	if($script:projects) {
		foreach($project in $script:projects) {
			$file = "$source_dir\$($project.Name)\Properties\AssemblyInfo.cs"
			Generate-AssemblyInfo -file $file `
				-title $project.Name `
				-description $project.Description `
				-product $product `
				-version $version `
				-company $company `
				-copyright $copyright
		}
	}
}
 
task Compile -depends Init {
    # execute msbuild - it is added to path by PSake
    Write-Host "Building $sln_file" -ForegroundColor Green
    Exec { msbuild.exe "$sln_file"  /p:Configuration=Release /nologo /verbosity:quiet }
}

task CompileCuda {
    Write-Host "Compile the CUDA files for the project" -ForegroundColor Green

	# load vs env variables
	$script = "$env:VS90COMNTOOLS\vsvars32.bat"
	$tempFile = [IO.Path]::GetTempFileName()
	cmd /c " `"$script`" && set > `"$tempFile`" "
	Get-Content $tempFile | Foreach-Object {
		if($_ -match "^(.*?)=(.*)$")
		{
			Set-Content "env:\$($matches[1])" $matches[2]
		}
	}
	Remove-Item $tempFile
	
	$env:PATH = $env:PATH + ";" + $env:CUDA_BIN_PATH

	$cuda_files = Get-ChildItem -Filter "*.cu" -Path $source_dir -Recurse
	$radiuses = @( 1, 3, 8, 12 )
	foreach($radius in $radiuses) 
	{
		foreach($cuda_file in $cuda_files) {
			$fullName = $cuda_file.FullName
			$result_name = $cuda_file.Name.Replace(".cu", "$radius.cubin")
			Exec { & "$env:CUDA_BIN_PATH\nvcc" "$fullName" --cubin --output-file "$build_dir\$result_name" -D "KERNEL_RADIUS=$radius" }
		}
	}
}


task Release -depends Compile, CompileCuda {
}