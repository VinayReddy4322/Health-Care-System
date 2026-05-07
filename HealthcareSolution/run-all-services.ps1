# Start each backend service in its own PowerShell window.
# This script can be launched from any folder:
#   & "E:\Mini Project 3\HealthcareSolution\run-all-services.ps1"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$dotnetHome = Join-Path $root ".dotnet-home"
$logs = Join-Path $root "logs"
$solution = Join-Path $root "HealthcareSolution.slnx"
$ports = @(5000, 5001, 5002, 5003, 5004)

New-Item -ItemType Directory -Force -Path $dotnetHome, $logs | Out-Null

foreach ($port in $ports) {
    Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue |
        Select-Object -ExpandProperty OwningProcess -Unique |
        Where-Object { $_ } |
        ForEach-Object {
            Stop-Process -Id $_ -Force -ErrorAction SilentlyContinue
        }
}

Get-Process UserService,PatientService,DoctorService,AppointmentService,Gateway -ErrorAction SilentlyContinue |
    Stop-Process -Force

$env:DOTNET_CLI_HOME = $dotnetHome
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_ADD_GLOBAL_TOOLS_TO_PATH = '0'

dotnet build $solution --no-restore -m:1
if ($LASTEXITCODE -ne 0) {
    throw "Backend build failed. Fix the build errors and run again."
}

function Start-HealthcareService {
    param(
        [string]$Project,
        [string]$Profile
    )

    $logFile = Join-Path $logs "$Project.log"
    $errorLogFile = Join-Path $logs "$Project.err.log"

    Start-Process dotnet `
        -WindowStyle Hidden `
        -WorkingDirectory $root `
        -ArgumentList @('run', '--project', $Project, '--launch-profile', $Profile, '--no-build') `
        -RedirectStandardOutput $logFile `
        -RedirectStandardError $errorLogFile
}

Start-HealthcareService -Project "UserService" -Profile "UserService"
Start-HealthcareService -Project "PatientService" -Profile "PatientService"
Start-HealthcareService -Project "DoctorService" -Profile "DoctorService"
Start-HealthcareService -Project "AppointmentService" -Profile "AppointmentService"
Start-HealthcareService -Project "Gateway" -Profile "Gateway"

Write-Host "Backend services are starting..."
Write-Host "Gateway: http://localhost:5000"
Write-Host "Logs: $logs"
Write-Host "Run Angular separately:"
Write-Host "  cd `"$root\AngularApp`""
Write-Host "  npm start"
