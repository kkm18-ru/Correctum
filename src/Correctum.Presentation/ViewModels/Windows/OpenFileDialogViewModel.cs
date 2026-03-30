using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Correctum.Controls;
using Correctum.Enums;
using System.Collections.ObjectModel;

namespace Correctum.ViewModels.Windows;

public sealed partial class OpenFileDialogViewModel : ObservableObject
{
    public ObservableCollection<IODialogItem> Items { get; } = [];

    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsValidFileName))]
    private string _fileName = string.Empty;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsValidFileName))]
    private string _directoryPath;
    public FileType[] FileTypes { get; }

    public string AbsolutePath => Path.Combine(DirectoryPath, FileName);
    public bool IsValidFileName => !IsDirectoryPath(AbsolutePath) && ValidPath(AbsolutePath);
    public int FileTypeIndex
    {
        get; set
        {
            if (field == value) return;
            field = value;
            Dispatcher.UIThread.Post(UpdateItems);
        }
    }
    public IODialogItem? SelectedItem
    {
        get; set
        {
            if (field != value)
            {
                if(value?.Type != IODialogItemType.ParentDirectory)
                    FileName = value?.FileName ?? string.Empty;
                field = value;
                return;
            }
            if (value is null) return;
            switch (value.Type)
            {
                case IODialogItemType.ParentDirectory:
                    field = null;
                    DirectoryPath = Path.GetDirectoryName(DirectoryPath);
                    Dispatcher.UIThread.Post(UpdateItems);
                    break;
                case IODialogItemType.File:
                    FileName = value.FileName;
                    break;
                case IODialogItemType.Directory:
                    field = null;
                    FileName = string.Empty;
                    DirectoryPath = Path.Combine(DirectoryPath, value.FileName);
                    Dispatcher.UIThread.Post(UpdateItems);
                    break;
            }
        }
    }

    public OpenFileDialogViewModel(FileType[] fileTypes, string basePath)
    {
        _directoryPath = basePath;
        FileTypes = fileTypes;
        UpdateItems();
    }

    private void UpdateItems()
    {
        Items.Clear();
        if (!DirectoryPath.EndsWith(":\\") && DirectoryPath != "/")
            Items.Add(new("..", IODialogItemType.ParentDirectory, this));
        var items = Directory.GetFileSystemEntries(DirectoryPath);
        var ordered = items.OrderBy(i => IsDirectoryPath(i) ? -1 : 1);
        var filtered = ordered.Where(i => IsDirectoryPath(i) || ValidPath(i));
        foreach (var item in filtered)
            Items.Add(new(Path.GetFileName(item), IsDirectoryPath(item) ?
                IODialogItemType.Directory : IODialogItemType.File, this));
    }

    private static bool IsDirectoryPath(string path) =>
        File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    private bool ValidPath(string path)
    {
        if (FileTypeIndex == -1) return true;
        var filename = Path.GetFileName(path);
        foreach (var pattern in FileTypes[FileTypeIndex].Patterns)
        {
            var elements = pattern.Split('*');
            switch (elements.Length)
            {
                case 1:
                    if (filename == elements[0])
                        return true;
                    continue;
                case 2:
                    if (filename.StartsWith(elements[0]) &&
                        filename.EndsWith(elements[1]))
                        return true;
                    continue;
                default: throw new NotImplementedException();
            }
        }
        return false;
    }


    /*public FileType[] FileTypes { get; init; } = [];
    public ObservableCollection<IODialogItem> Items { get; } = [];

    [ObservableProperty] private string m_directoryPath;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsValidFileName))] 
    private string m_fileName;

    public string AbsolutePath => Path.Combine(DirectoryPath, FileName);

    public OpenFileDialogViewModel(FileType[] types, string basePath)
    {
        FileTypes = types;
        DirectoryPath = basePath;
        UpdateItems();
    }

    public bool IsValidFileName => !IsDirectory(AbsolutePath) && CheckPattern(AbsolutePath);

    public int FileType
    {
        get; set
        {
            if (field == value) return;
            field = value;
            Dispatcher.UIThread.Post(UpdateItems);
        }
    } = -1;

    public IODialogItem? SelectedItem
    {
        get; set
        {
            if (value is null) return;
            if (field?.Equals(value) ?? false) return;
            FileName = value.FileName;
            //FilePath = value.ItemPath;
            if (value.Type == IODialogItemType.Directory)
            {
                field = null;
                DirectoryPath.
                Dispatcher.UIThread.Post(UpdateItems);
            }
            else field = value;
        }
    }

    private void UpdateItems()
    {
        Items.Clear();
        if(!DirectoryPath.EndsWith(":\\") && DirectoryPath != "/")
            Items.Add(new("..", IODialogItemType.ParentDirectory));
        var items = Directory.GetFileSystemEntries(FilePath);
        var ordered = items.OrderBy(i => IsDirectory(i) ? -1 : 1);
        var filtered = ordered.Where(i => IsDirectory(i) || CheckPattern(i));
        foreach (var item in filtered)
            Items.Add(new(FilePath, item, IsDirectory(item) ?
                IODialogItemType.Directory : IODialogItemType.File));
    }*/
}

public readonly struct FileType(string name, params string[] patterns)
{
    public string Name { get; } = name;
    public string[] Patterns { get; } = patterns;

    public override string ToString() => 
        $"{Name} ({string.Join("; ", Patterns)})";
}
