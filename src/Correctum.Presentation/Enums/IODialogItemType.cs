namespace Correctum.Enums;

public enum IODialogItemType
{
    Directory, 
    File,
    ParentDirectory
}

public static class IODialogItemTypeExtensions
{
    extension(IODialogItemType type){
        public string ToImagePath() => type switch
        {
            IODialogItemType.ParentDirectory or IODialogItemType.Directory => "Assets/Icons/Directory.ico",
            _ => "Assets/Icons/File.ico"
        };
    }
}