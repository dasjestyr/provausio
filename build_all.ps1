$msBuildPath = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
$solutionFiles =
  "src/Provausio.Rest/Provausio.Rest.Client.sln",
  "src/Provausio.Parsing/Provausio.Parsing/Provausio.Parsing.sln"
  
# restore packages
foreach($path in $solutionFiles){ `
  nuget restore $path `
}
  
# build projects
foreach($path in $solutionFiles){ `
  $cmd = $(msBuildPath) $(path) /p:Configuration=Release `
  $cmd `
}