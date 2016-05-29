$ErrorActionPreference = 'Stop'

# set keyboard layout.
# NB you can get the name from the list:
#      [System.Globalization.CultureInfo]::GetCultures('InstalledWin32Cultures') | out-gridview
Set-WinUserLanguageList pt-PT -Force

# set the date format, number format, etc.
Set-Culture pt-PT

# set the timezone.
# tzutil /l lists all available timezone ids
& $env:windir\system32\tzutil /s "GMT Standard Time"

# install applications.
iex ((New-Object Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
choco install -y googlechrome
choco install -y notepad2
choco install -y JPEGView
