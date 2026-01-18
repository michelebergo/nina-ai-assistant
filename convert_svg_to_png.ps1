# Requires ImageMagick's 'magick' command in PATH
$svg = "assets/icon.svg"
$png = "NINA.Plugin.AIAssistant/icon.png"
magick convert $svg -resize 128x128 $png
Write-Host "SVG converted to PNG: $png"