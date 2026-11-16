$services = @(
    "src/HR/HR.API/HR.API.csproj",
    "src/Attendance/Attendance.API/Attendance.csproj",
    "src/Payroll/Payroll.API/Payroll.API.csproj",
    "src/Project/Project.API/Project.csproj"
)

foreach ($service in $services) {
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project `"$service`""
}