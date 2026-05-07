# Stop backend services started from this solution.
# Use before rebuilding if MSBuild says an .exe is being used by another process.

$ports = @(5000, 5001, 5002, 5003, 5004)

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

Write-Host "Healthcare backend services stopped."
