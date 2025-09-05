[Setup]
AppId={{B8F9B41C-E335-4005-9D8F-AB14521012C1}
AppName=RetailCorrector
AppVersion={#AppVersion}
DefaultDirName={autopf}\RetailCorrector
UninstallDisplayIcon={app}\RetailCorrector.exe
DefaultGroupName=RetailCorrector
DisableProgramGroupPage=yes
OutputBaseFilename=RetailCorrector-{#AppVersion}
SolidCompression=yes
WizardStyle=modern
OutputDir={#OutDir}

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#SourcePath}\*"; DestDir: "{app}"

[Icons]
Name: "{group}\RetailCorrector"; Filename: "{app}\RetailCorrector.exe"
Name: "{autodesktop}\RetailCorrector"; Filename: "{app}\RetailCorrector.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\RetailCorrector.exe"; Flags: nowait postinstall skipifsilent

