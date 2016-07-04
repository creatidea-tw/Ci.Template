# find solutionName
$slnFile = Get-ChildItem *.sln
$slnName = $slnFile.Name.Replace(".sln","")

# get solution root folder
$destinationFolder = ($pwd).path
$unnecessaryFolder = $destinationFolder + "\"+$slnName

# switch to unnecessary folder
cd $slnName 

$adminFolder = ($pwd).path +"\"+$slnName+".Admin"
$libraryFolder = ($pwd).path +"\"+$slnName+".Library"
$serviceFolder = ($pwd).path +"\"+$slnName+".Service"
$webFolder = ($pwd).path +"\"+$slnName+".Web"

$newAdminFolder = $destinationFolder+"\"+$slnName+".Admin"
$newLibraryFolder = $destinationFolder+"\"+$slnName+".Library"
$newServiceFolder = $destinationFolder+"\"+$slnName+".Service"
$newWebFolder = $destinationFolder+"\"+$slnName+".Web"

# create correct folder
New-Item -Path $newAdminFolder -ItemType directory
New-Item -Path $newLibraryFolder -ItemType directory
New-Item -Path $newServiceFolder -ItemType directory
New-Item -Path $newWebFolder -ItemType directory

# move all file and subfolers
Get-ChildItem -Path $adminFolder -Recurse |
Move-Item -destination $newAdminFolder

Get-ChildItem -Path $libraryFolder -Recurse |
Move-Item -destination $newLibraryFolder

Get-ChildItem -Path $serviceFolder -Recurse |
Move-Item -destination $newServiceFolder

Get-ChildItem -Path $webFolder -Recurse |
Move-Item -destination $newWebFolder

cd ..\

# delete unnecessary folder
Remove-Item -Path $unnecessaryFolder -Force -Recurse

# edit solution file
$replaceTarget = $slnName + "\"
(Get-Content $slnFile.FullName).replace($replaceTarget, "") | Set-Content $slnFile.FullName