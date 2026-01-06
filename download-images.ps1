# Script pentru descarcarea imaginilor produselor
# Ruleaza acest script din directorul radacina al proiectului

$imagesPath = "OnlineStoreApp\wwwroot\images"

# Creeaza folderul daca nu exista
if (-not (Test-Path $imagesPath)) {
    New-Item -ItemType Directory -Path $imagesPath -Force
    Write-Host "Folder creat: $imagesPath" -ForegroundColor Green
}

# Dictionar cu link-uri si nume de fisiere (folosim Picsum Photos pentru link-uri sigure)
$images = @{
    "classic-white-sneakers.jpg" = "https://picsum.photos/seed/sneakers1/500/500"
    "black-hightop-sneakers.jpg" = "https://picsum.photos/seed/sneakers2/500/500"
    "retro-sneakers.jpg" = "https://picsum.photos/seed/sneakers3/500/500"
    "minimalist-white-sneakers.jpg" = "https://picsum.photos/seed/sneakers4/500/500"
    "leather-ankle-boots.jpg" = "https://picsum.photos/seed/boots1/500/500"
    "combat-boots.jpg" = "https://picsum.photos/seed/boots2/500/500"
    "chelsea-boots.jpg" = "https://picsum.photos/seed/boots3/500/500"
    "ultra-lightweight-running.jpg" = "https://picsum.photos/seed/running1/500/500"
    "trail-running-shoes.jpg" = "https://picsum.photos/seed/running2/500/500"
    "marathon-running-shoes.jpg" = "https://picsum.photos/seed/running3/500/500"
    "canvas-slipon.jpg" = "https://picsum.photos/seed/casual1/500/500"
    "loafers.jpg" = "https://picsum.photos/seed/casual2/500/500"
    "beach-sandals.jpg" = "https://picsum.photos/seed/sandals1/500/500"
    "sport-sandals.jpg" = "https://picsum.photos/seed/sandals2/500/500"
    "oxford-dress-shoes.jpg" = "https://picsum.photos/seed/formal1/500/500"
    "derby-shoes.jpg" = "https://picsum.photos/seed/formal2/500/500"
}

Write-Host "Incep descarcarea imaginilor..." -ForegroundColor Cyan
Write-Host ""

$successCount = 0
$failCount = 0

foreach ($image in $images.GetEnumerator()) {
    $fileName = $image.Key
    $url = $image.Value
    $filePath = Join-Path $imagesPath $fileName
    
    try {
        Write-Host "Descarcare: $fileName..." -NoNewline
        Invoke-WebRequest -Uri $url -OutFile $filePath -UseBasicParsing -ErrorAction Stop
        Write-Host " OK" -ForegroundColor Green
        $successCount++
    }
    catch {
        Write-Host " EROARE: $($_.Exception.Message)" -ForegroundColor Red
        $failCount++
    }
}

Write-Host ""
Write-Host "Descarcare completa!" -ForegroundColor Cyan
Write-Host "Succes: $successCount imagini" -ForegroundColor Green
if ($failCount -gt 0) {
    Write-Host "Esate: $failCount imagini" -ForegroundColor Red
}
