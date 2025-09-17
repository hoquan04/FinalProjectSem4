# fix-encoding.ps1
# Script để chuyển đổi tất cả file trong project sang UTF-8

param(
    [string]$ProjectPath = "."
)

Write-Host "🔧 Đang sửa encoding cho tất cả file trong project..." -ForegroundColor Green

# Danh sách các extension file cần sửa
$extensions = @("*.cs", "*.cshtml", "*.razor", "*.js", "*.css", "*.json", "*.txt", "*.md")

foreach ($extension in $extensions) {
    $files = Get-ChildItem -Path $ProjectPath -Recurse -Filter $extension -File
    
    foreach ($file in $files) {
        try {
            # Bỏ qua các thư mục không cần thiết
            if ($file.FullName -match "(bin|obj|node_modules|\.git)") {
                continue
            }
            
            Write-Host "📄 Đang xử lý: $($file.FullName)" -ForegroundColor Yellow
            
            # Đọc file với encoding tự động detect
            $content = Get-Content -Path $file.FullName -Raw
            
            # Ghi lại với UTF-8 BOM
            $utf8 = New-Object System.Text.UTF8Encoding $true
            [System.IO.File]::WriteAllText($file.FullName, $content, $utf8)
            
            Write-Host "✅ Đã sửa: $($file.Name)" -ForegroundColor Green
        }
        catch {
            Write-Host "❌ Lỗi khi xử lý $($file.Name): $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

Write-Host "🎉 Hoàn thành! Tất cả file đã được chuyển sang UTF-8." -ForegroundColor Green
Write-Host "💡 Khuyến nghị: Restart Visual Studio để áp dụng thay đổi." -ForegroundColor Cyan
