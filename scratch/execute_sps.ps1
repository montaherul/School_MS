$connectionString = "Server=MONTAHERUL\SQLEXPRESS;Database=SchoolManagementSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False"
$spPath = "c:\Users\islam\OneDrive\Documents\New project\SchoolMS\Data\StoredProcedures"

# Find all .sql files recursively
$sqlFiles = Get-ChildItem -Path $spPath -Filter *.sql -Recurse

foreach ($file in $sqlFiles) {
    Write-Host "Executing: $($file.FullName)"
    try {
        # Using sqlcmd utility as it's common on Windows with SQL Express
        sqlcmd -S "MONTAHERUL\SQLEXPRESS" -d "SchoolManagementSystemDb" -i "$($file.FullName)" -E
        Write-Host "Success: $($file.Name)" -ForegroundColor Green
    } catch {
        Write-Host "Error executing $($file.Name): $($_.Exception.Message)" -ForegroundColor Red
    }
}
