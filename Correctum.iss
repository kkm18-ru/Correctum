[Setup]
AppId={{B8F9B41C-E335-4005-9D8F-AB14521012C1}
AppName=Correctum
AppVersion={#AppVersion}
DefaultDirName={autopf}\Correctum
UninstallDisplayIcon={app}\Correctum.exe
DefaultGroupName=Correctum
DisableProgramGroupPage=yes
OutputBaseFilename=Correctum-{#AppVersion}
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
Name: "{group}\Correctum"; Filename: "{app}\Correctum.exe"
Name: "{autodesktop}\Correctum"; Filename: "{app}\Correctum.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\Correctum.exe"; Flags: nowait postinstall skipifsilent

